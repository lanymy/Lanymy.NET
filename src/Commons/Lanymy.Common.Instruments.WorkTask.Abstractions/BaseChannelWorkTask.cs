using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{



    public abstract class BaseChannelWorkTask<TDataModel> : BaseWorkTask//, IChannelWorkTask<TDataModel>
        where TDataModel : IWorkTaskQueueDataModel
    {

        protected Channel<TDataModel> _CurrentChannel;
        protected readonly Action<TDataModel> _CurrentWorkAction;
        protected readonly Action<List<TDataModel>> _CurrentStopAndReadQueueAllDataAction;

        //protected List<TDataModel> _CurrentReadQueueAllDataList = new List<TDataModel>();
        //protected bool _IsReadQueueAllData = false;

        public int ChannelCapacityCount { get; }

        public BoundedChannelFullMode ChannelFullMode { get; }
        public int TaskSleepMilliseconds { get; }
        public int WorkTaskTotalCount { get; }

        protected readonly bool _IsInternalChannel = false;


        protected BaseChannelWorkTask(Channel<TDataModel> channel, Action<TDataModel> workAction, Action<List<TDataModel>> stopAndReadQueueAllDataAction, int workTaskTotalCount, int taskSleepMilliseconds, int channelCapacityCount, BoundedChannelFullMode channelFullMode)
        {

            if (workAction.IfIsNull())
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


        protected virtual async Task<List<TDataModel>> ReadQueueAllDataAsync()
        {

            var list = new List<TDataModel>();

            await foreach (var item in _CurrentChannel.Reader.ReadAllAsync())
            {
                list.Add(item);
            }

            return list;

        }


        protected virtual async Task OnStopAndReadQueueAllDataActionAsync()
        {

            if (!_CurrentStopAndReadQueueAllDataAction.IfIsNull())
            {

                var list = await ReadQueueAllDataAsync();
                _CurrentStopAndReadQueueAllDataAction(list);

                list.Clear();

            }

        }


        ///// <summary>
        ///// 停止执行任务,并返回当前消息队列中的全部数据
        ///// </summary>
        ///// <returns></returns>
        //public abstract Task<List<TDataModel>> StopAndReadQueueAllDataAsync();


    }





}
