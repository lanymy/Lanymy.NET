using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 基于 <see cref="Channel{T}"/> 的工作任务基类，负责队列创建、写入和停机读尾。
    /// </summary>
    public abstract class BaseChannelWorkTask<TDataModel> : BaseWorkTask//, IChannelWorkTask<TDataModel>
                                                                        //where TDataModel : IWorkTaskQueueDataModel
    {
        /// <summary>
        /// 当前使用的通道实例。
        /// </summary>
        protected Channel<TDataModel> _CurrentChannel;
        protected readonly Func<TDataModel, Task> _CurrentAsyncWorkAction;
        protected readonly Action<List<TDataModel>> _CurrentStopAndReadQueueAllDataAction;
        protected readonly Action<TDataModel> _CurrentWorkAction;

        /// <summary>
        /// 绑定通道容量；小于等于 0 时使用无界通道。
        /// </summary>
        public int ChannelCapacityCount { get; }

        public BoundedChannelFullMode ChannelFullMode { get; }
        public int TaskSleepMilliseconds { get; }
        public int WorkTaskTotalCount { get; }

        /// <summary>
        /// 指示当前通道是否由外部注入。
        /// </summary>
        protected readonly bool _IsInternalChannel = false;


        protected BaseChannelWorkTask(Channel<TDataModel> channel, Action<TDataModel> workAction, Action<List<TDataModel>> stopAndReadQueueAllDataAction, int workTaskTotalCount, int taskSleepMilliseconds, int channelCapacityCount, BoundedChannelFullMode channelFullMode)
            : this(channel, workAction, null, stopAndReadQueueAllDataAction, workTaskTotalCount, taskSleepMilliseconds, channelCapacityCount, channelFullMode)
        {

        }

        protected BaseChannelWorkTask(Channel<TDataModel> channel, Func<TDataModel, Task> asyncWorkAction, Action<List<TDataModel>> stopAndReadQueueAllDataAction, int workTaskTotalCount, int taskSleepMilliseconds, int channelCapacityCount, BoundedChannelFullMode channelFullMode)
            : this(channel, null, asyncWorkAction, stopAndReadQueueAllDataAction, workTaskTotalCount, taskSleepMilliseconds, channelCapacityCount, channelFullMode)
        {

        }

        private BaseChannelWorkTask(Channel<TDataModel> channel, Action<TDataModel> workAction, Func<TDataModel, Task> asyncWorkAction, Action<List<TDataModel>> stopAndReadQueueAllDataAction, int workTaskTotalCount, int taskSleepMilliseconds, int channelCapacityCount, BoundedChannelFullMode channelFullMode)
        {
            if (workAction.IfIsNull() && asyncWorkAction.IfIsNull())
            {
                throw new ArgumentNullException(nameof(workAction));
            }

            _CurrentStopAndReadQueueAllDataAction = stopAndReadQueueAllDataAction;

            if (workTaskTotalCount < 1)
            {
                workTaskTotalCount = 1;
            }

            if (taskSleepMilliseconds <= 0)
            {
                taskSleepMilliseconds = 3 * 1000;
            }

            if (channelCapacityCount < 0)
            {
                channelCapacityCount = 0;
            }

            WorkTaskTotalCount = workTaskTotalCount;
            _CurrentWorkAction = workAction;
            _CurrentAsyncWorkAction = asyncWorkAction;

            ChannelCapacityCount = channelCapacityCount;
            ChannelFullMode = channelFullMode;
            TaskSleepMilliseconds = taskSleepMilliseconds;


            if (!channel.IfIsNull())
            {
                // 外部传入 channel 时，由调用方负责它的生命周期。
                _IsInternalChannel = true;
                _CurrentChannel = channel;
            }
        }

        /// <summary>
        /// 根据容量配置创建内部通道。
        /// </summary>
        /// <returns>新建的通道实例。</returns>
        protected virtual Channel<TDataModel> CreateChannel()
        {
            var channel = ChannelCapacityCount <= 0
                ? Channel.CreateUnbounded<TDataModel>()
                : Channel.CreateBounded<TDataModel>(new BoundedChannelOptions(ChannelCapacityCount)
                {
                    FullMode = ChannelFullMode,
                });

            return channel;
        }

        /// <summary>
        /// 向当前通道追加一条数据。
        /// </summary>
        /// <param name="data">待写入的数据。</param>
        public virtual async Task AddToQueueAsync(TDataModel data)
        {
            if (!IsRunning)
            {
                return;
            }

            var currentChannel = _CurrentChannel;
            if (currentChannel.IfIsNull())
            {
                return;
            }

            if (!await currentChannel.Writer.WaitToWriteAsync())
            {
                return;
            }

            // WaitToWriteAsync 返回后再做一次快照确认，避免 stop / channel 切换窗口把数据写进旧通道。
            if (!IsRunning || !ReferenceEquals(_CurrentChannel, currentChannel))
            {
                return;
            }

            try
            {
                await currentChannel.Writer.WriteAsync(data);
            }
            catch (ChannelClosedException) when (!IsRunning || !ReferenceEquals(_CurrentChannel, currentChannel))
            {
                // ignored
            }
        }

        /// <summary>
        /// 读取停机时通道中尚未消费完的全部数据。
        /// </summary>
        /// <returns>剩余数据列表。</returns>
        protected virtual async Task<List<TDataModel>> ReadQueueAllDataAsync()
        {
            var list = new List<TDataModel>();
            var currentChannel = _CurrentChannel;
            if (currentChannel.IfIsNull())
            {
                return list;
            }

            if (_IsInternalChannel)
            {
                // 外部通道不能被本类消费 Completion，因此这里只做非阻塞快照读取。
                while (currentChannel.Reader.TryRead(out var item))
                {
                    list.Add(item);
                }

                return list;
            }

            await foreach (var item in currentChannel.Reader.ReadAllAsync())
            {
                list.Add(item);
            }

            return list;

        }

        /// <summary>
        /// 在停止阶段执行剩余数据回调。
        /// </summary>
        protected virtual async Task OnStopAndReadQueueAllDataActionAsync()
        {
            if (!_CurrentStopAndReadQueueAllDataAction.IfIsNull())
            {
                var list = await ReadQueueAllDataAsync();
                _CurrentStopAndReadQueueAllDataAction(list);
                list.Clear();
            }
        }
    }
}
