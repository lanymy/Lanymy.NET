using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{


    public abstract class BaseSimpleWorkTaskQueue<TData> : BaseWorkTask
    //where TAddToQueueData : class
    //where TQueueData : class
    {

        protected Task _CurrentTask;
        protected readonly ConcurrentQueue<TData> _CurrentCacheConcurrentQueue = new ConcurrentQueue<TData>();
        private readonly Action<TData> _WorkAction;
        private readonly int _SleepIntervalMilliseconds;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workAction"></param>
        /// <param name="sleepIntervalMilliseconds">执行完当前消息队列所有信息后,到下一次循环的空闲等待时间; 小于等于0 为不等待</param>
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



        protected virtual void OnWorkAction(TData data)
        {
            _WorkAction(data);
        }


        private async Task OnTaskAsync()
        {

            TData data;

            while (IsRunning)
            {

                while (_CurrentCacheConcurrentQueue.TryDequeue(out data))
                {

                    OnWorkAction(data);

                }

                if (_SleepIntervalMilliseconds > 0)
                {
                    await Task.Delay(_SleepIntervalMilliseconds);
                }

            }

        }

        protected virtual async void OnTask()
        {

            await OnTaskAsync();

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
