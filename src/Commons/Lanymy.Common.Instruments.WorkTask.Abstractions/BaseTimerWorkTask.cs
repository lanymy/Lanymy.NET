using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 以固定间隔执行工作的定时任务基类。
    /// </summary>
    public abstract class BaseTimerWorkTask : BaseWorkTask
    {
        /// <summary>
        /// 当前定时任务的工作委托。
        /// </summary>
        protected readonly Func<TimerWorkTaskDataResult> _CurrentWorkFunc;
        public int TaskSleepMilliseconds { get; }
        protected CancellationTokenSource _CurrentCancellationTokenSource;
        protected Task _CurrentTask;

        /// <summary>
        /// 初始化定时任务。
        /// </summary>
        /// <param name="workFunc">每次 tick 执行的逻辑。</param>
        /// <param name="taskSleepMilliseconds">tick 间隔，默认 3 秒。</param>
        protected BaseTimerWorkTask(Func<TimerWorkTaskDataResult> workFunc, int taskSleepMilliseconds = 3 * 1000)
        {
            _CurrentWorkFunc = workFunc;
            TaskSleepMilliseconds = taskSleepMilliseconds;
        }

        /// <summary>
        /// 执行单次 tick 逻辑。
        /// </summary>
        /// <returns>当前 tick 的执行结果。</returns>
        protected virtual TimerWorkTaskDataResult OnWorkFunc()
        {
            return _CurrentWorkFunc();
        }

        /// <summary>
        /// 处理单次 tick 异常。
        /// </summary>
        /// <param name="ex">捕获的异常。</param>
        protected virtual void OnWorkError(Exception ex)
        {
        }

        private void TryOnWorkError(Exception ex)
        {
            try
            {
                OnWorkError(ex);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 定时任务主循环。
        /// </summary>
        /// <param name="token">当前停止令牌。</param>
        protected virtual async Task OnTaskAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    // 每轮先等待再执行，保证定时器语义是“固定间隔触发下一次”。
                    await Task.Delay(TaskSleepMilliseconds, token);

                    TimerWorkTaskDataResult timerWorkTaskDataResult;

                    try
                    {
                        timerWorkTaskDataResult = OnWorkFunc();
                    }
                    catch (OperationCanceledException) when (token.IsCancellationRequested)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        TryOnWorkError(ex);
                        continue;
                    }

                    if (!timerWorkTaskDataResult.IfIsNullOrEmpty() && timerWorkTaskDataResult.IsBreak)
                    {
                        // 允许工作函数主动要求退出定时循环。
                        break;
                    }
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
            }
        }

        protected override async Task OnStartAsync()
        {
            _CurrentCancellationTokenSource = new CancellationTokenSource();
            var token = _CurrentCancellationTokenSource.Token;

            _CurrentTask = Task.Factory.StartNew(
                () => OnTaskAsync(token),
                token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default).Unwrap();

            await Task.CompletedTask;
        }

        protected override async Task OnStopAsync()
        {
            Exception stopException = null;
            var currentTask = _CurrentTask;
            var currentCancellationTokenSource = _CurrentCancellationTokenSource;

            if (currentCancellationTokenSource.IfIsNullOrEmpty())
            {
                return;
            }

            try
            {
                currentCancellationTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                stopException = ex;
            }

            try
            {
                if (!currentTask.IfIsNullOrEmpty())
                {
                    await currentTask;
                }
            }
            catch (OperationCanceledException)
            {
                // ignored
            }
            catch (Exception ex)
            {
                stopException = ex;
            }
            finally
            {
                if (ReferenceEquals(_CurrentTask, currentTask))
                {
                    _CurrentTask = null;
                }

                currentTask?.Dispose();

                if (ReferenceEquals(_CurrentCancellationTokenSource, currentCancellationTokenSource))
                {
                    _CurrentCancellationTokenSource = null;
                }

                currentCancellationTokenSource?.Dispose();
            }

            if (stopException != null)
            {
                throw stopException;
            }
        }
    }
}
