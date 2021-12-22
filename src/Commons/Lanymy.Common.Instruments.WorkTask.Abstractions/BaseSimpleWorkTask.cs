using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{


    public abstract class BaseSimpleWorkTask<TData> : BaseWorkTask
    //where TAddToQueueData : class
    //where TQueueData : class
    {

        protected Task _CurrentTask;
        protected readonly ConcurrentQueue<TData> _CurrentCacheConcurrentQueue = new ConcurrentQueue<TData>();
        private readonly Action<TData> _WorkAction;

        protected BaseSimpleWorkTask(Action<TData> workAction)
        {

            if (workAction.IfIsNull())
            {
                throw new ArgumentNullException(nameof(workAction));
            }

            _WorkAction = workAction;

        }



        protected virtual void OnWorkAction(TData data)
        {
            _WorkAction(data);
        }


        protected virtual void OnTask()
        {

            TData data;

            while (IsRunning)
            {

                while (_CurrentCacheConcurrentQueue.TryDequeue(out data))
                {

                    OnWorkAction(data);

                }

                Task.Delay(10).Wait();

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

            _CurrentTask = new Task(OnTask, TaskCreationOptions.LongRunning);
            _CurrentTask.Start();

            await Task.CompletedTask;

        }

        protected override async Task OnStopAsync()
        {

            if (!_CurrentTask.IfIsNullOrEmpty())
            {

                if (_CurrentTask.Status == TaskStatus.Running)
                {
                    await _CurrentTask;
                }

                _CurrentTask.Dispose();
                _CurrentTask = null;

            }

            _CurrentCacheConcurrentQueue.Clear();

        }


        protected override async Task OnDisposeAsync()
        {
            await StopAsync();
        }
    }

}
