using System;
using System.Threading.Channels;

namespace Lanymy.Common.Instruments
{


    public class WorkTaskQueue<TDataModel> : BaseWorkTaskQueue<TDataModel>
        where TDataModel : IWorkTaskQueueDataModel
    {





        internal WorkTaskQueue(Channel<TDataModel> channel, Action<TDataModel> workAction, int workTaskTotalCount, int taskSleepMilliseconds, int channelCapacityCount, BoundedChannelFullMode channelFullMode)
            : base(channel, workAction, workTaskTotalCount, taskSleepMilliseconds, channelCapacityCount, channelFullMode)
        {



        }

        public WorkTaskQueue(Action<TDataModel> workAction, int workTaskTotalCount = 1, int taskSleepMilliseconds = 3 * 1000, int channelCapacityCount = 0, BoundedChannelFullMode channelFullMode = BoundedChannelFullMode.Wait)
            : this(null, workAction, workTaskTotalCount, taskSleepMilliseconds, channelCapacityCount, channelFullMode)
        {


        }




    }

}
