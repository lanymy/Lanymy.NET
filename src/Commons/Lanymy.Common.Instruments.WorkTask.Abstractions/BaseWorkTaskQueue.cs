using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{


    public abstract class BaseWorkTaskQueue<TDataModel> : BaseChannelWorkTask<TDataModel>
        where TDataModel : IWorkTaskQueueDataModel
    {



        protected readonly List<Task> _CurrentWorkTaskList = new List<Task>();
        protected CancellationTokenSource _CurrentCancellationTokenSource;


        protected BaseWorkTaskQueue(Channel<TDataModel> channel, Action<TDataModel> workAction, int workTaskTotalCount = 1, int taskSleepMilliseconds = 3 * 1000, int channelCapacityCount = 0, BoundedChannelFullMode channelFullMode = BoundedChannelFullMode.Wait)
            : base(channel, workAction, workTaskTotalCount, taskSleepMilliseconds, channelCapacityCount, channelFullMode)
        {

        }


        //protected abstract Task OnAddToQueueAsync(TDataModel data);

        protected virtual void OnWorkAction(TDataModel dataModel)
        {
            _CurrentWorkAction(dataModel);
        }


        protected virtual async Task OnTaskAsync(object obj)
        {

            var token = (CancellationToken)obj;

            while (!token.IsCancellationRequested)
            {

                while (await _CurrentChannel.Reader.WaitToReadAsync(token))
                {

                    while (_CurrentChannel.Reader.TryRead(out var dataModel))
                    {
                        OnWorkAction(dataModel);
                    }

                }

            }

        }

        private void OnTask(object obj)
        {

            try
            {
                //OnTaskAsync(token).Wait(token);
                OnTaskAsync(obj).Wait();
            }
            catch (Exception e)
            {

                if (!e.Message.ToLower().Contains("task was canceled"))
                {
                    throw;
                }

            }


        }


        protected override async Task OnStartAsync()
        {

            if (!_IsInternalChannel)
            {
                _CurrentChannel = CreateChannel();
            }

            _CurrentCancellationTokenSource = new CancellationTokenSource();
            var token = _CurrentCancellationTokenSource.Token;

            for (var i = 0; i < WorkTaskTotalCount; i++)
            {

                var task = new Task(OnTask, token, token, TaskCreationOptions.LongRunning);
                //var task = new Task(async o => await OnTaskAsync(o), token, token, TaskCreationOptions.LongRunning);
                //var task = new Task(async () => await OnTaskAsync(token), TaskCreationOptions.LongRunning);
                task.Start();

                //var task = Task.Run(async () => await OnTaskAsync(token), token);
                //task.Start();

                _CurrentWorkTaskList.Add(task);

            }

            await Task.CompletedTask;

        }



        protected override async Task OnStopAsync()
        {


            if (!_IsInternalChannel)
            {

                _CurrentChannel.Writer.Complete();

            }



            if (_CurrentCancellationTokenSource.IfIsNullOrEmpty())
            {
                return;
            }


            _CurrentCancellationTokenSource.Cancel();


            foreach (var task in _CurrentWorkTaskList)
            {

                if (task.Status == TaskStatus.Running)
                {
                    task.Wait();
                }

                task.Dispose();

            }


            if (!_CurrentCancellationTokenSource.IfIsNullOrEmpty())
            {
                _CurrentCancellationTokenSource.Dispose();
                _CurrentCancellationTokenSource = null;
            }


            _CurrentWorkTaskList.Clear();



            if (_IsReadQueueAllData)
            {

                _IsReadQueueAllData = false;

                await foreach (var item in _CurrentChannel.Reader.ReadAllAsync())
                {
                    _CurrentReadQueueAllDataList.Add(item);
                }

            }

            if (!_IsInternalChannel)
            {

                await _CurrentChannel.Reader.Completion;
                _CurrentChannel = null;

            }


        }


        public override async Task<List<TDataModel>> StopAndReadQueueAllDataAsync()
        {

            _IsReadQueueAllData = true;

            await StopAsync();

            return _CurrentReadQueueAllDataList;

        }


        protected override async Task OnDisposeAsync()
        {

            if (!_CurrentReadQueueAllDataList.IfIsNullOrEmpty())
            {
                _CurrentReadQueueAllDataList.Clear();
                _CurrentReadQueueAllDataList = null;
            }

            await Task.CompletedTask;

        }



    }

}
