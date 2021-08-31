using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{


    public abstract class BaseTimerWorkTask : BaseWorkTask
    {

        protected readonly Func<TimerWorkTaskDataResult> _CurrentWorkFunc;
        public int TaskSleepMilliseconds { get; }
        protected CancellationTokenSource _CurrentCancellationTokenSource;
        protected Task _CurrentTask;


        protected BaseTimerWorkTask(Func<TimerWorkTaskDataResult> workFunc, int taskSleepMilliseconds = 3 * 1000)
        {

            _CurrentWorkFunc = workFunc;
            TaskSleepMilliseconds = taskSleepMilliseconds;

        }


        protected virtual TimerWorkTaskDataResult OnWorkFunc()
        {
            return _CurrentWorkFunc();
        }


        protected virtual void OnTask(object obj)
        {

            var token = (CancellationToken)obj;

            while (!token.IsCancellationRequested)
            {

                Task.Delay(TaskSleepMilliseconds).Wait();

                var timerWorkTaskDataResult = OnWorkFunc();

                if (!timerWorkTaskDataResult.IfIsNullOrEmpty() && timerWorkTaskDataResult.IsBreak)
                {
                    break;
                }

            }

        }


        protected override async Task OnStartAsync()
        {

            _CurrentCancellationTokenSource = new CancellationTokenSource();
            var token = _CurrentCancellationTokenSource.Token;

            _CurrentTask = new Task(OnTask, token, token, TaskCreationOptions.LongRunning);
            _CurrentTask.Start();

            await Task.CompletedTask;

        }



        protected override async Task OnStopAsync()
        {


            if (_CurrentCancellationTokenSource.IfIsNullOrEmpty())
            {
                return;
            }


            _CurrentCancellationTokenSource.Cancel();


            if (_CurrentTask.Status == TaskStatus.Running)
            {
                _CurrentTask.Wait();
            }

            _CurrentTask.Dispose();
            _CurrentTask = null;

            if (!_CurrentCancellationTokenSource.IfIsNullOrEmpty())
            {
                _CurrentCancellationTokenSource.Dispose();
                _CurrentCancellationTokenSource = null;
            }


        }



    }

}
