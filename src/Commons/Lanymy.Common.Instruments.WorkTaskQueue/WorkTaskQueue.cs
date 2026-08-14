using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 面向外部使用的通用工作队列实现，支持同步或异步工作委托。
    /// </summary>
    public class WorkTaskQueue<TDataModel> : BaseWorkTaskQueue<TDataModel>
    //where TDataModel : IWorkTaskQueueDataModel
    {
        /// <summary>
        /// 使用已有通道和同步工作委托初始化队列。
        /// </summary>
        internal WorkTaskQueue(Channel<TDataModel> channel, Action<TDataModel> workAction, Action<List<TDataModel>> stopAndReadQueueAllDataAction, int workTaskTotalCount, int taskSleepMilliseconds, int channelCapacityCount, BoundedChannelFullMode channelFullMode)
            : base(channel, workAction, stopAndReadQueueAllDataAction, workTaskTotalCount, taskSleepMilliseconds, channelCapacityCount, channelFullMode)
        {
        }

        /// <summary>
        /// 使用同步工作委托初始化队列。
        /// </summary>
        public WorkTaskQueue(Action<TDataModel> workAction, Action<List<TDataModel>> stopAndReadQueueAllDataAction, int workTaskTotalCount = 1, int taskSleepMilliseconds = 3 * 1000, int channelCapacityCount = 0, BoundedChannelFullMode channelFullMode = BoundedChannelFullMode.Wait)
            : this((Channel<TDataModel>)null, workAction, stopAndReadQueueAllDataAction, workTaskTotalCount, taskSleepMilliseconds, channelCapacityCount, channelFullMode)
        {
        }

        /// <summary>
        /// 使用已有通道和异步工作委托初始化队列。
        /// </summary>
        internal WorkTaskQueue(Channel<TDataModel> channel, Func<TDataModel, Task> asyncWorkAction, Action<List<TDataModel>> stopAndReadQueueAllDataAction, int workTaskTotalCount, int taskSleepMilliseconds, int channelCapacityCount, BoundedChannelFullMode channelFullMode)
            : base(channel, asyncWorkAction, stopAndReadQueueAllDataAction, workTaskTotalCount, taskSleepMilliseconds, channelCapacityCount, channelFullMode)
        {

        }

        /// <summary>
        /// 使用异步工作委托初始化队列。
        /// </summary>
        public WorkTaskQueue(Func<TDataModel, Task> asyncWorkAction, Action<List<TDataModel>> stopAndReadQueueAllDataAction, int workTaskTotalCount = 1, int taskSleepMilliseconds = 3 * 1000, int channelCapacityCount = 0, BoundedChannelFullMode channelFullMode = BoundedChannelFullMode.Wait)
            : this((Channel<TDataModel>)null, asyncWorkAction, stopAndReadQueueAllDataAction, workTaskTotalCount, taskSleepMilliseconds, channelCapacityCount, channelFullMode)
        {
        }
    }
}
