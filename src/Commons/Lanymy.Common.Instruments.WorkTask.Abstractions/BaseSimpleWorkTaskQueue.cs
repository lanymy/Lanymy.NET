using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 基于 <see cref="ConcurrentQueue{T}"/> 的简单单线程队列执行模型。
    /// </summary>
    public abstract class BaseSimpleWorkTaskQueue<TData> : BaseWorkTask
    //where TAddToQueueData : class
    //where TQueueData : class
    {
        /// <summary>
        /// 当前后台消费任务。
        /// </summary>
        protected Task _CurrentTask;
        protected CancellationTokenSource _CurrentCancellationTokenSource;
        protected readonly ConcurrentQueue<TData> _CurrentCacheConcurrentQueue = new ConcurrentQueue<TData>();
        private readonly Action<TData> _WorkAction;
        private readonly int _SleepIntervalMilliseconds;

        /// <summary>
        /// 初始化简单队列任务。
        /// </summary>
        /// <param name="workAction">每条数据的处理委托。</param>
        /// <param name="sleepIntervalMilliseconds">执行完当前消息队列后，到下一轮空轮询前的等待时间；小于等于 0 表示不等待。</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected BaseSimpleWorkTaskQueue(Action<TData> workAction, int sleepIntervalMilliseconds = 10)
        {

            if (workAction.IfIsNull())
            {
                throw new ArgumentNullException(nameof(workAction));
            }


            if (sleepIntervalMilliseconds < 0)
            {
                sleepIntervalMilliseconds = 0;
            }

            _SleepIntervalMilliseconds = sleepIntervalMilliseconds;
            _WorkAction = workAction;
        }

        /// <summary>
        /// 执行单条队列数据。
        /// </summary>
        /// <param name="data">当前数据。</param>
        protected virtual void OnWorkAction(TData data)
        {
            _WorkAction(data);
        }

        /// <summary>
        /// 处理单条数据的执行异常。
        /// </summary>
        /// <param name="data">出错的数据。</param>
        /// <param name="ex">捕获的异常。</param>
        protected virtual void OnWorkError(TData data, Exception ex)
        {
        }

        /// <summary>
        /// 在空轮询之间执行等待。
        /// </summary>
        /// <param name="token">当前停止令牌。</param>
        /// <returns>等待任务。</returns>
        protected virtual Task DelayAsync(CancellationToken token)
        {
            if (_SleepIntervalMilliseconds <= 0)
            {
                return Task.CompletedTask;
            }

            return Task.Delay(_SleepIntervalMilliseconds, token);
        }

        private void TryOnWorkError(TData data, Exception ex)
        {
            try
            {
                OnWorkError(data, ex);
            }
            catch
            {
            }
        }

        protected virtual void ClearCurrentCacheConcurrentQueue()
        {
#if NET48
            TData data;

            // .NET Framework 4.8 没有 ConcurrentQueue.Clear()，这里保留兼容实现。
            while (_CurrentCacheConcurrentQueue.TryDequeue(out data))
            {
            }
#else
            _CurrentCacheConcurrentQueue.Clear();
#endif
        }

        /// <summary>
        /// 消费队列的后台循环。
        /// </summary>
        /// <param name="token">当前停止令牌。</param>
        private async Task OnTaskAsync(CancellationToken token)
        {
            TData data;

            while (!token.IsCancellationRequested && IsRunning)
            {
                while (_CurrentCacheConcurrentQueue.TryDequeue(out data))
                {
                    try
                    {
                        OnWorkAction(data);
                    }
                    catch (Exception ex)
                    {
                        TryOnWorkError(data, ex);
                    }

                }

                if (_SleepIntervalMilliseconds > 0)
                {
                    await DelayAsync(token);
                }

            }

        }

        public virtual void AddToQueue(TData data)
        {
            if (IsRunning)
            {
                _CurrentCacheConcurrentQueue.Enqueue(data);
            }
        }

        protected override async Task OnStartAsync()
        {
            if (_CurrentCancellationTokenSource.IfIsNull())
            {
                _CurrentCancellationTokenSource = new CancellationTokenSource();
            }

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

            try
            {
                currentCancellationTokenSource?.Cancel();
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
            catch (OperationCanceledException) when (currentCancellationTokenSource != null && currentCancellationTokenSource.IsCancellationRequested)
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
                ClearCurrentCacheConcurrentQueue();
            }

            if (stopException != null)
            {
                throw stopException;
            }
        }
    }
}
