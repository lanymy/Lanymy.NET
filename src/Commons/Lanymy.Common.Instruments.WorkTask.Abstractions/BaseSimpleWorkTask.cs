﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{

    public abstract class BaseSimpleWorkTask : BaseWorkTask
    {

        protected Task _CurrentTask;
        private readonly Action<CancellationToken> _WorkAction;
        private readonly int _SleepIntervalMilliseconds;
        protected CancellationTokenSource _CurrentCancellationTokenSource;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="workAction"></param>
        /// <param name="sleepIntervalMilliseconds">小于等于0为执行间隔不休眠,大于0每次执行间隔休眠时间</param>
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




        protected virtual void OnWorkAction(CancellationToken token)
        {

            _WorkAction(token);

        }

        protected virtual void OnWorkError(CancellationToken token, Exception ex)
        {
        }

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
