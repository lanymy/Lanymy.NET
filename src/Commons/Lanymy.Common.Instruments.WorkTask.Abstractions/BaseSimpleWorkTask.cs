﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 以“单个循环任务”方式持续执行工作的基础实现。
    /// </summary>
    public abstract class BaseSimpleWorkTask : BaseWorkTask
    {
        /// <summary>
        /// 当前后台循环任务。
        /// </summary>
        protected Task _CurrentTask;
        private readonly Action<CancellationToken> _WorkAction;
        private readonly int _SleepIntervalMilliseconds;
        protected CancellationTokenSource _CurrentCancellationTokenSource;

        /// <summary>
        /// 初始化简单循环任务。
        /// </summary>
        /// <param name="workAction">每次循环执行的工作委托。</param>
        /// <param name="sleepIntervalMilliseconds">小于等于 0 时每轮之间不休眠；大于 0 时按该间隔等待。</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected BaseSimpleWorkTask(Action<CancellationToken> workAction, int sleepIntervalMilliseconds = 0)
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
        /// 执行单轮工作逻辑。
        /// </summary>
        /// <param name="token">当前停止令牌。</param>
        protected virtual void OnWorkAction(CancellationToken token)
        {
            _WorkAction(token);
        }

        /// <summary>
        /// 处理单轮工作异常。
        /// </summary>
        /// <param name="token">当前停止令牌。</param>
        /// <param name="ex">捕获的异常。</param>
        protected virtual void OnWorkError(CancellationToken token, Exception ex)
        {
        }

        /// <summary>
        /// 在两轮工作之间执行等待。
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

        private void TryOnWorkError(CancellationToken token, Exception ex)
        {
            try
            {
                OnWorkError(token, ex);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 后台循环主逻辑。
        /// </summary>
        /// <param name="token">当前停止令牌。</param>
        private async Task OnTaskAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested && IsRunning)
                {
                    try
                    {
                        OnWorkAction(token);
                    }
                    catch (OperationCanceledException) when (token.IsCancellationRequested)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        TryOnWorkError(token, ex);
                    }

                    if (_SleepIntervalMilliseconds > 0)
                    {
                        // 把休眠放在循环尾部，保证每轮工作后再节流。
                        await DelayAsync(token);
                    }
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
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

            if (currentCancellationTokenSource.IfIsNull())
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
            catch (OperationCanceledException) when (currentCancellationTokenSource.IsCancellationRequested)
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
