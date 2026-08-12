using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{


    public abstract class BaseWorkTaskQueue<TDataModel> : BaseChannelWorkTask<TDataModel>
    //where TDataModel : IWorkTaskQueueDataModel
    {



        protected readonly List<Task> _CurrentWorkTaskList = new List<Task>();
        protected CancellationTokenSource _CurrentCancellationTokenSource;


        protected BaseWorkTaskQueue(Channel<TDataModel> channel, Action<TDataModel> workAction, Action<List<TDataModel>> stopAndReadQueueAllDataAction = null, int workTaskTotalCount = 1, int taskSleepMilliseconds = 3 * 1000, int channelCapacityCount = 0, BoundedChannelFullMode channelFullMode = BoundedChannelFullMode.Wait)
            : base(channel, workAction, stopAndReadQueueAllDataAction, workTaskTotalCount, taskSleepMilliseconds, channelCapacityCount, channelFullMode)
        {

        }

        protected BaseWorkTaskQueue(Channel<TDataModel> channel, Func<TDataModel, Task> asyncWorkAction, Action<List<TDataModel>> stopAndReadQueueAllDataAction = null, int workTaskTotalCount = 1, int taskSleepMilliseconds = 3 * 1000, int channelCapacityCount = 0, BoundedChannelFullMode channelFullMode = BoundedChannelFullMode.Wait)
            : base(channel, asyncWorkAction, stopAndReadQueueAllDataAction, workTaskTotalCount, taskSleepMilliseconds, channelCapacityCount, channelFullMode)
        {

        }


        //protected abstract Task OnAddToQueueAsync(TDataModel data);

        protected virtual void OnWorkAction(TDataModel dataModel)
        {
            _CurrentWorkAction(dataModel);
        }

        protected virtual async Task OnWorkActionAsync(TDataModel dataModel)
        {
            if (!_CurrentAsyncWorkAction.IfIsNull())
            {
                await _CurrentAsyncWorkAction(dataModel);
                return;
            }

            OnWorkAction(dataModel);
        }

        protected virtual void OnWorkError(TDataModel dataModel, Exception ex)
        {
        }

        private void TryOnWorkError(TDataModel dataModel, Exception ex)
        {
            try
            {
                OnWorkError(dataModel, ex);
            }
            catch
            {
            }
        }


        protected virtual async Task OnTaskAsync(CancellationToken token)
        {

            try
            {
                while (!token.IsCancellationRequested)
                {

                    while (await _CurrentChannel.Reader.WaitToReadAsync(token))
                    {

                        while (IsRunning && _CurrentChannel.Reader.TryRead(out var dataModel))
                        {
                            try
                            {
                                await OnWorkActionAsync(dataModel);
                            }
                            catch (OperationCanceledException) when (token.IsCancellationRequested)
                            {
                                return;
                            }
                            catch (Exception ex)
                            {
                                TryOnWorkError(dataModel, ex);
                            }
                        }

                    }

                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
            }
            catch
            {

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
                var task = Task.Factory.StartNew(
                    () => OnTaskAsync(token),
                    token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default).Unwrap();

                _CurrentWorkTaskList.Add(task);

            }

            await Task.CompletedTask;

        }



        protected override async Task OnStopAsync()
        {
            var exceptions = new List<Exception>();
            var currentChannel = _CurrentChannel;
            var currentCancellationTokenSource = _CurrentCancellationTokenSource;
            var currentWorkTaskList = _CurrentWorkTaskList.ToArray();

            if (!_IsInternalChannel && !currentChannel.IfIsNull())
            {
                currentChannel.Writer.TryComplete();

            }



            try
            {
                if (!currentCancellationTokenSource.IfIsNullOrEmpty())
                {
                    currentCancellationTokenSource.Cancel();
                }
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            try
            {
                if (currentWorkTaskList.Length > 0)
                {
                    await Task.WhenAll(currentWorkTaskList);
                }
            }
            catch (OperationCanceledException)
            {
                // ignored
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
            finally
            {
                foreach (var task in currentWorkTaskList)
                {
                    task.Dispose();
                }

                if (ReferenceEquals(_CurrentCancellationTokenSource, currentCancellationTokenSource))
                {
                    _CurrentCancellationTokenSource = null;
                }

                currentCancellationTokenSource?.Dispose();
                _CurrentWorkTaskList.Clear();
            }

            try
            {
                await OnStopAndReadQueueAllDataActionAsync();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            if (!_IsInternalChannel && !currentChannel.IfIsNull())
            {
                try
                {
                    await currentChannel.Reader.Completion;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
                finally
                {
                    if (ReferenceEquals(_CurrentChannel, currentChannel))
                    {
                        _CurrentChannel = null;
                    }
                }
            }

            if (exceptions.Count == 1)
            {
                throw exceptions[0];
            }

            if (exceptions.Count > 1)
            {
                throw new AggregateException(exceptions);
            }


        }


        //public override async Task<List<TDataModel>> StopAndReadQueueAllDataAsync()
        //{

        //    _IsReadQueueAllData = true;

        //    await StopAsync();

        //    return _CurrentReadQueueAllDataList;

        //}


        protected override async Task OnDisposeAsync()
        {

            //if (!_CurrentReadQueueAllDataList.IfIsNullOrEmpty())
            //{
            //    _CurrentReadQueueAllDataList.Clear();
            //    _CurrentReadQueueAllDataList = null;
            //}

            await Task.CompletedTask;

        }



    }

}
