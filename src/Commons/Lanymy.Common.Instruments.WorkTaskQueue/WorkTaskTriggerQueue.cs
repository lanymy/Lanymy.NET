using System;
using System.Collections.Generic;
using System.Threading.Channels;

namespace Lanymy.Common.Instruments
{


    public class WorkTaskTriggerQueue<TDataModel> : BaseWorkTaskTriggerQueue<TDataModel>
    //where TDataModel : IWorkTaskQueueDataModel
    {


        internal WorkTaskTriggerQueue(Channel<TDataModel> channel, Action<List<TDataModel>> workTriggerAction, ushort actionTriggerCount, TimeSpan actionTriggerTimeSpan, int taskSleepMilliseconds = 3000, int channelCapacityCount = 0, BoundedChannelFullMode channelFullMode = BoundedChannelFullMode.Wait)
            : base(channel, workTriggerAction, actionTriggerCount, actionTriggerTimeSpan, taskSleepMilliseconds, channelCapacityCount, channelFullMode)
        {


        }


        public WorkTaskTriggerQueue(Action<List<TDataModel>> workTriggerAction, ushort actionTriggerCount, TimeSpan actionTriggerTimeSpan, int taskSleepMilliseconds = 3000, int channelCapacityCount = 0, BoundedChannelFullMode channelFullMode = BoundedChannelFullMode.Wait)
            : this(null, workTriggerAction, actionTriggerCount, actionTriggerTimeSpan, taskSleepMilliseconds, channelCapacityCount, channelFullMode)
        {



        }





    }
}
