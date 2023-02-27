using System;
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


        private async Task OnTaskAsync(CancellationToken token)
        {

            try
            {
                while (!token.IsCancellationRequested && IsRunning)
                {

                    OnWorkAction(token);

                    if (_SleepIntervalMilliseconds > 0)
                    {
                        await Task.Delay(_SleepIntervalMilliseconds);
                    }

                }
            }
            catch
            {

            }

        }


        protected virtual async void OnTask(object obj)
        {

            try
            {
                var token = (CancellationToken)obj;
                await OnTaskAsync(token);
            }
            catch
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

            _CurrentTask = new Task(OnTask, token, token, TaskCreationOptions.LongRunning);
            _CurrentTask.Start();

            await Task.CompletedTask;

        }



        protected override async Task OnStopAsync()
        {

            if (_CurrentCancellationTokenSource.IfIsNull())
            {
                _CurrentCancellationTokenSource.Cancel();
            }

            _CurrentCancellationTokenSource.Dispose();
            _CurrentCancellationTokenSource = null;


            if (!_CurrentTask.IfIsNullOrEmpty())
            {

                if (_CurrentTask.Status == TaskStatus.Running)
                {
                    await _CurrentTask;
                }

                _CurrentTask.Dispose();
                _CurrentTask = null;

            }

        }


        protected override async Task OnDisposeAsync()
        {
            await StopAsync();
        }
    }

}
