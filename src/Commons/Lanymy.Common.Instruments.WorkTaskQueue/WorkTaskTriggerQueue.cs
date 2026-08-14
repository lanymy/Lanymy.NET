using System;
using System.Collections.Generic;
using System.Threading.Channels;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 面向外部使用的批量触发工作队列实现。
    /// </summary>
    public class WorkTaskTriggerQueue<TDataModel> : BaseWorkTaskTriggerQueue<TDataModel>
    //where TDataModel : IWorkTaskQueueDataModel
    {
        /// <summary>
        /// 使用已有通道初始化触发队列。
        /// </summary>
        internal WorkTaskTriggerQueue(Channel<TDataModel> channel, Action<List<TDataModel>> workTriggerAction, ushort actionTriggerCount, TimeSpan actionTriggerTimeSpan, int taskSleepMilliseconds = 3000, int channelCapacityCount = 0, BoundedChannelFullMode channelFullMode = BoundedChannelFullMode.Wait)
            : base(channel, workTriggerAction, actionTriggerCount, actionTriggerTimeSpan, taskSleepMilliseconds, channelCapacityCount, channelFullMode)
        {
        }

        /// <summary>
        /// 初始化触发队列。
        /// </summary>
        /// <param name="workTriggerAction">满足触发条件时执行的批处理逻辑。</param>
        /// <param name="actionTriggerCount">按数量触发的阈值。</param>
        /// <param name="actionTriggerTimeSpan">按时间触发的阈值。</param>
        /// <param name="taskSleepMilliseconds">时间触发检查间隔。</param>
        /// <param name="channelCapacityCount">通道容量；小于等于 0 时使用无界通道。</param>
        /// <param name="channelFullMode">有界通道满载时的策略。</param>
        public WorkTaskTriggerQueue(Action<List<TDataModel>> workTriggerAction, ushort actionTriggerCount, TimeSpan actionTriggerTimeSpan, int taskSleepMilliseconds = 3000, int channelCapacityCount = 0, BoundedChannelFullMode channelFullMode = BoundedChannelFullMode.Wait)
            : this(null, workTriggerAction, actionTriggerCount, actionTriggerTimeSpan, taskSleepMilliseconds, channelCapacityCount, channelFullMode)
        {
        }
    }
}
