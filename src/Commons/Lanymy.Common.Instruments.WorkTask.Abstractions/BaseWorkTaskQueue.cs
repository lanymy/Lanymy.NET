using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 提供基于 Channel 的多工作线程队列执行模型。
    /// </summary>
    public abstract class BaseWorkTaskQueue<TDataModel> : BaseChannelWorkTask<TDataModel>
    //where TDataModel : IWorkTaskQueueDataModel
    {
        /// <summary>
        /// 当前启动的工作任务集合。
        /// </summary>
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
        /// <summary>
        /// 执行单条工作项的同步逻辑。
        /// </summary>
        /// <param name="dataModel">当前工作项。</param>
        protected virtual void OnWorkAction(TDataModel dataModel)
        {
            _CurrentWorkAction(dataModel);
        }

        /// <summary>
        /// 执行单条工作项的异步逻辑；若未提供异步委托则回退到同步委托。
        /// </summary>
        /// <param name="dataModel">当前工作项。</param>
        protected virtual async Task OnWorkActionAsync(TDataModel dataModel)
        {
            if (!_CurrentAsyncWorkAction.IfIsNull())
            {
                await _CurrentAsyncWorkAction(dataModel);
                return;
            }

            OnWorkAction(dataModel);
        }

        /// <summary>
        /// 处理单条工作项异常。
        /// </summary>
        /// <param name="dataModel">出错的工作项。</param>
        /// <param name="ex">捕获的异常。</param>
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

        /// <summary>
        /// 工作线程主循环。
        /// </summary>
        /// <param name="token">停止令牌。</param>
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
                // 每个 worker 都独立从同一个 channel 消费，形成并发处理模型。
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
                // 内部创建的 channel 由本类负责 complete，这样 worker 能尽快退出 WaitToReadAsync。
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
                // worker 全部退出后再读尾，避免和消费线程并发争抢剩余数据。
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

        protected override async Task OnDisposeAsync()
        {
            await Task.CompletedTask;
        }
    }
}
