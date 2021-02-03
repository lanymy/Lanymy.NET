using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{

    public class WorkTaskQueue<TDataModel> : IDisposable
            where TDataModel : class
    {

        protected readonly object _Locker = new();

        private readonly ConcurrentQueue<TDataModel> _CurrentConcurrentQueue;

        private Task _CurrentTask;
        private CancellationTokenSource _CurrentCancellationTokenSource;

        private const int MAX_TASK_SLEEP_NUM = 3;
        private const int TASK_SLEEP_INCREMENT_INTERVAL_SECONDS = 2;

        private ushort _TaskSleepCount = 0;


        private bool _IsRunning = false;

        public bool IsRunning
        {
            get { return _IsRunning; }
            private set
            {

                if (_IsRunning == value) return;

                lock (_Locker)
                {
                    if (!_IsRunning.Equals(value))
                    {
                        _IsRunning = value;
                    }
                }

            }
        }

        public bool IsTaskSleeping { get; private set; } = false;


        protected Action<TDataModel> _WorkAction;

        public bool IsClearQueueWhenStop { get; } = false;


        public WorkTaskQueue(ConcurrentQueue<TDataModel> currentConcurrentQueue, bool isClearQueueWhenStop, Action<TDataModel> workAction)
        {

            _CurrentConcurrentQueue = currentConcurrentQueue.IfIsNull() ? new ConcurrentQueue<TDataModel>() : currentConcurrentQueue;
            _WorkAction = workAction;
            IsClearQueueWhenStop = isClearQueueWhenStop;

        }



        public virtual void AddToQueue(TDataModel data)
        {

            if (IsRunning)
            {

                _CurrentConcurrentQueue.Enqueue(data);

            }

        }


        protected virtual void OnWorkAction(TDataModel dataModel)
        {
            _WorkAction(dataModel);
        }


        protected virtual void OnFinishSleep()
        {

        }


        //private void OnTask()
        //{



        //    while (true)
        //    {

        //        TDataModel dataModel;

        //        if (_CurrentConcurrentQueue.TryDequeue(out dataModel))
        //        {

        //            if (_TaskSleepCount > 0)
        //            {
        //                _TaskSleepCount = 0;
        //            }

        //            //_WorkAction(dataModel);
        //            OnWorkAction(dataModel);

        //        }
        //        else
        //        {

        //            if (IsRunning)
        //            {

        //                if (_TaskSleepCount < MAX_TASK_SLEEP_NUM)
        //                {
        //                    IsTaskSleeping = true;
        //                    Task.Delay((_TaskSleepCount * TASK_SLEEP_INCREMENT_INTERVAL_SECONDS + 1) * 1000).Wait();
        //                    IsTaskSleeping = false;
        //                    _TaskSleepCount++;
        //                    continue;
        //                }

        //                OnFinishSleep();
        //                _TaskSleepCount = 0;

        //            }
        //            else
        //            {
        //                if (_CurrentConcurrentQueue.IsEmpty)
        //                {
        //                    break;
        //                }
        //            }


        //        }

        //    }

        //}


        private void OnTask(object obj)
        {

            var token = (CancellationToken)obj;

            while (!token.IsCancellationRequested)
            {

                TDataModel dataModel;

                if (_CurrentConcurrentQueue.TryDequeue(out dataModel))
                {

                    if (_TaskSleepCount > 0)
                    {
                        _TaskSleepCount = 0;
                    }

                    OnWorkAction(dataModel);

                }
                else
                {


                    if (_TaskSleepCount < MAX_TASK_SLEEP_NUM)
                    {
                        IsTaskSleeping = true;
                        Task.Delay((_TaskSleepCount * TASK_SLEEP_INCREMENT_INTERVAL_SECONDS + 1) * 1000).Wait();
                        IsTaskSleeping = false;
                        _TaskSleepCount++;
                        continue;
                    }

                    OnFinishSleep();
                    _TaskSleepCount = 0;


                }

            }

        }


        public async Task StartAsync()
        {

            if (IsRunning)
            {
                return;
            }

            //await StopTaskAsync();
            StopTaskAsync().Wait();

            StartTask();

            IsRunning = true;

            await Task.CompletedTask;

        }

        public async Task StopAsync()
        {

            if (IsRunning)
            {

                IsRunning = false;

                //await StopTaskAsync();
                StopTaskAsync().Wait();

            }

            await Task.CompletedTask;

        }



        private void StartTask()
        {

            _CurrentCancellationTokenSource = new CancellationTokenSource();
            var token = _CurrentCancellationTokenSource.Token;
            _CurrentTask = new Task(OnTask, token, token, TaskCreationOptions.LongRunning);
            _CurrentTask.Start();

        }



        private async Task StopTaskAsync()
        {


            if (IsClearQueueWhenStop && !_CurrentConcurrentQueue.IsEmpty)
            {
                _CurrentConcurrentQueue.Clear();
            }

            if (_CurrentTask.IfIsNullOrEmpty() || _CurrentCancellationTokenSource.IfIsNullOrEmpty())
            {
                return;
            }


            _CurrentCancellationTokenSource.Cancel();

            if (_CurrentTask.Status == TaskStatus.Running)
            {

                _CurrentTask.Wait();

            }


            if (!_CurrentTask.IfIsNullOrEmpty())
            {
                _CurrentTask.Dispose();
                _CurrentTask = null;
            }


            if (!_CurrentCancellationTokenSource.IfIsNullOrEmpty())
            {
                _CurrentCancellationTokenSource.Dispose();
                _CurrentCancellationTokenSource = null;
            }

            await Task.CompletedTask;

        }

        public void Dispose()
        {

            _CurrentConcurrentQueue.Clear();
            StopAsync().Wait();

        }

    }

}
