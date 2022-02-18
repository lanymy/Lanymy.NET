using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{



    public abstract class BaseChannelWorkTask<TDataModel> : BaseWorkTask
        where TDataModel : IWorkTaskQueueDataModel
    {

        protected Channel<TDataModel> _CurrentChannel;
        protected readonly Action<TDataModel> _CurrentWorkAction;

        protected List<TDataModel> _CurrentReadQueueAllDataList = new List<TDataModel>();
        protected bool _IsReadQueueAllData = false;

        public int ChannelCapacityCount { get; }

        public BoundedChannelFullMode ChannelFullMode { get; }
        public int TaskSleepMilliseconds { get; }
        public int WorkTaskTotalCount { get; }

        protected readonly bool _IsInternalChannel = false;


        protected BaseChannelWorkTask(Channel<TDataModel> channel, Action<TDataModel> workAction, int workTaskTotalCount, int taskSleepMilliseconds, int channelCapacityCount, BoundedChannelFullMode channelFullMode)
        {

            if (workAction.IfIsNull())
            {
                throw new ArgumentNullException(nameof(workAction));
            }

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

            ChannelCapacityCount = channelCapacityCount;
            ChannelFullMode = channelFullMode;
            TaskSleepMilliseconds = taskSleepMilliseconds;


            if (!channel.IfIsNull())
            {

                _IsInternalChannel = true;
                _CurrentChannel = channel;

            }



        }

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

        public virtual async Task AddToQueueAsync(TDataModel data)
        {

            if (IsRunning)
            {

                //while (!await _CurrentChannel.Writer.WaitToWriteAsync())
                //{

                //    await Task.Delay(TaskSleepMilliseconds);

                //}

                //await _CurrentChannel.Writer.WriteAsync(data);

                //await OnAddToQueueAsync(data);


                if (await _CurrentChannel.Writer.WaitToWriteAsync())
                {

                    await _CurrentChannel.Writer.WriteAsync(data);

                }


            }

        }


        //返回当前消息队列中的全部数据
        public abstract Task<List<TDataModel>> StopAndReadQueueAllDataAsync();


    }





}
