using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{


    public class WorkTaskTriggerQueueContext<TDataModel> : BaseChannelWorkTask<TDataModel>
    //where TDataModel : IWorkTaskQueueDataModel
    {


        public DynamicAsyncQueueStateTypeEnum StateType { get; private set; } = DynamicAsyncQueueStateTypeEnum.UnDefine;

        public readonly ushort WorkTaskCount;

        private readonly List<BaseWorkTask> _WorkTaskQueueList = new List<BaseWorkTask>();

        private readonly Action<List<TDataModel>> _WorkTriggerAction;

        /// <summary>
        /// 触发事件的数据数量阀值,如: 每1000个数据量 触发一次事件
        /// </summary>
        public readonly ushort OnActionTriggerCount;
        /// <summary>
        ///  触发事件的闲置时间,如: 消息队列内数据只有100个数据不满足 数据数量触发阀值条件,并且 5分钟 内 数据数量依然不满足触发条件的时候,满足5分钟定时触发条件
        /// </summary>
        public readonly TimeSpan OnActionTriggerTimeSpan;



        public WorkTaskTriggerQueueContext(Action<List<TDataModel>> workTriggerAction, ushort workTaskCount, ushort actionTriggerCount, TimeSpan actionTriggerTimeSpan, int channelCapacityCount = 0, BoundedChannelFullMode channelFullMode = BoundedChannelFullMode.Wait)
            : base(null, _ => { }, null, 1, 3 * 1000, channelCapacityCount, channelFullMode)
        {

            if (workTriggerAction.IfIsNull())
            {
                throw new ArgumentNullException(nameof(workTriggerAction));
            }

            if (workTaskCount < 1)
            {
                workTaskCount = 1;
            }

            if (actionTriggerCount == 0)
            {
                actionTriggerCount = 1;
            }

            if (actionTriggerTimeSpan.TotalSeconds < 3)
            {
                actionTriggerTimeSpan = TimeSpan.FromSeconds(3);
            }

            OnActionTriggerCount = actionTriggerCount;
            WorkTaskCount = workTaskCount;
            _WorkTriggerAction = workTriggerAction;
            OnActionTriggerTimeSpan = actionTriggerTimeSpan;

            StateType = DynamicAsyncQueueStateTypeEnum.Stop;

        }



        protected virtual BaseWorkTask CreateWorkTaskQueue(Channel<TDataModel> channel, ushort workTaskIndex)
        {
            return new WorkTaskTriggerQueue<TDataModel>(channel, _WorkTriggerAction, OnActionTriggerCount, OnActionTriggerTimeSpan);
        }

        protected virtual List<Exception> CompleteChannel(Channel<TDataModel> channel)
        {
            var exceptions = new List<Exception>();

            if (channel.IfIsNull())
            {
                return exceptions;
            }

            try
            {
                channel.Writer.TryComplete();
            }
            catch (Exception ex)
            {
                exceptions.Add(new InvalidOperationException("WorkTaskTriggerQueueContext complete channel failed.", ex));
            }

            return exceptions;
        }

        protected virtual async Task<List<Exception>> CleanupWorkTaskQueueListAsync(IEnumerable<BaseWorkTask> workTaskQueueList)
        {
            var exceptions = new List<Exception>();

            foreach (var workTaskQueueModel in workTaskQueueList)
            {
                if (workTaskQueueModel.IfIsNull())
                {
                    continue;
                }

                try
                {
                    await workTaskQueueModel.StopAsync();
                }
                catch (Exception ex)
                {
                    exceptions.Add(new InvalidOperationException("WorkTaskTriggerQueueContext stop child work task failed.", ex));
                }

                try
                {
                    workTaskQueueModel.Dispose();
                }
                catch (Exception ex)
                {
                    exceptions.Add(new InvalidOperationException("WorkTaskTriggerQueueContext dispose child work task failed.", ex));
                }
            }

            return exceptions;
        }

        protected virtual async Task<List<Exception>> CleanupChannelAsync(Channel<TDataModel> channel)
        {
            var exceptions = new List<Exception>();

            if (channel.IfIsNull())
            {
                return exceptions;
            }

            try
            {
                DrainRemainingChannelData(channel);
            }
            catch (Exception ex)
            {
                exceptions.Add(new InvalidOperationException("WorkTaskTriggerQueueContext drain remaining channel data failed.", ex));
            }

            try
            {
                await channel.Reader.Completion;
            }
            catch (Exception ex)
            {
                exceptions.Add(new InvalidOperationException("WorkTaskTriggerQueueContext await channel completion failed.", ex));
            }

            return exceptions;
        }

        private static void ThrowCollectedExceptions(List<Exception> exceptions)
        {
            if (exceptions.IfIsNullOrEmpty())
            {
                return;
            }

            if (exceptions.Count == 1)
            {
                ExceptionDispatchInfo.Capture(exceptions[0]).Throw();
            }

            throw new AggregateException(exceptions);
        }

        protected override async Task OnStartAsync()
        {
            if (StateType == DynamicAsyncQueueStateTypeEnum.Stop)
            {
                var currentChannel = CreateChannel();
                var currentWorkTaskQueueList = new List<BaseWorkTask>();

                try
                {
                    for (ushort i = 0; i < WorkTaskCount; i++)
                    {
                        currentWorkTaskQueueList.Add(CreateWorkTaskQueue(currentChannel, i));
                    }

                    var list = currentWorkTaskQueueList.Select(o => o.StartAsync()).ToList();
                    await Task.WhenAll(list.ToArray());

                    _CurrentChannel = currentChannel;
                    _WorkTaskQueueList.AddRange(currentWorkTaskQueueList);
                    StateType = DynamicAsyncQueueStateTypeEnum.Start;
                }
                catch (Exception ex)
                {
                    var exceptions = new List<Exception> { ex };
                    exceptions.AddRange(CompleteChannel(currentChannel));
                    exceptions.AddRange(await CleanupWorkTaskQueueListAsync(currentWorkTaskQueueList));
                    exceptions.AddRange(await CleanupChannelAsync(currentChannel));

                    _WorkTaskQueueList.Clear();
                    _CurrentChannel = null;
                    StateType = DynamicAsyncQueueStateTypeEnum.Stop;

                    ThrowCollectedExceptions(exceptions);
                }

            }

            await Task.CompletedTask;

        }

        protected override async Task OnStopAsync()
        {
            if (StateType == DynamicAsyncQueueStateTypeEnum.Start)
            {
                StateType = DynamicAsyncQueueStateTypeEnum.Cancel;
                var currentChannel = _CurrentChannel;
                var currentWorkTaskQueueList = _WorkTaskQueueList.ToArray();
                var exceptions = new List<Exception>();

                try
                {
                    exceptions.AddRange(CompleteChannel(currentChannel));
                    exceptions.AddRange(await CleanupWorkTaskQueueListAsync(currentWorkTaskQueueList));
                    exceptions.AddRange(await CleanupChannelAsync(currentChannel));
                }
                finally
                {
                    _WorkTaskQueueList.Clear();

                    if (ReferenceEquals(_CurrentChannel, currentChannel))
                    {
                        _CurrentChannel = null;
                    }

                    StateType = DynamicAsyncQueueStateTypeEnum.Stop;
                }

                ThrowCollectedExceptions(exceptions);
            }

        }

        private static void DrainRemainingChannelData(Channel<TDataModel> channel)
        {
            if (channel.IfIsNull())
            {
                return;
            }

            while (channel.Reader.TryRead(out _))
            {
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
