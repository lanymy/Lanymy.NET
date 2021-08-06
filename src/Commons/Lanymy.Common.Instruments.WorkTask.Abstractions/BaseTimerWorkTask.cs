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

        protected readonly Action _CurrentWorkAction;
        public int TaskSleepMilliseconds { get; }
        protected CancellationTokenSource _CurrentCancellationTokenSource;
        protected Task _CurrentTask;


        protected BaseTimerWorkTask(Action workAction, int taskSleepMilliseconds = 3 * 1000)
        {

            _CurrentWorkAction = workAction;
            TaskSleepMilliseconds = taskSleepMilliseconds;

        }


        protected virtual void OnWorkAction()
        {
            _CurrentWorkAction();
        }

        protected virtual void OnTask(object obj)
        {

            var token = (CancellationToken)obj;

            while (!token.IsCancellationRequested)
            {

                Task.Delay(TaskSleepMilliseconds).Wait();

                OnWorkAction();
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
