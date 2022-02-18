using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{


    public class WorkTaskTriggerQueueContext<TDataModel> : BaseChannelWorkTask<TDataModel>
        where TDataModel : IWorkTaskQueueDataModel
    {


        public DynamicAsyncQueueStateTypeEnum StateType { get; private set; } = DynamicAsyncQueueStateTypeEnum.UnDefine;

        public readonly ushort WorkTaskCount;

        private readonly List<WorkTaskTriggerQueue<TDataModel>> _WorkTaskQueueList = new List<WorkTaskTriggerQueue<TDataModel>>();

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
            : base(null, _ => { }, 1, 3 * 1000, channelCapacityCount, channelFullMode)
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



        protected override async Task OnStartAsync()
        {



            if (StateType == DynamicAsyncQueueStateTypeEnum.Stop)
            {


                _CurrentChannel = CreateChannel();

                for (var i = 0; i < WorkTaskCount; i++)
                {
                    _WorkTaskQueueList.Add(new WorkTaskTriggerQueue<TDataModel>(_CurrentChannel, _WorkTriggerAction, OnActionTriggerCount, OnActionTriggerTimeSpan));
                }

                var list = _WorkTaskQueueList.Select(o => o.StartAsync()).ToList();


                //await Task.WhenAll(list.ToArray());
                Task.WhenAll(list.ToArray()).Wait();


                StateType = DynamicAsyncQueueStateTypeEnum.Start;

            }

            await Task.CompletedTask;

        }

        protected override async Task OnStopAsync()
        {

            if (StateType == DynamicAsyncQueueStateTypeEnum.Start)
            {

                StateType = DynamicAsyncQueueStateTypeEnum.Cancel;

                _CurrentChannel.Writer.Complete();

                foreach (var workTaskQueueModel in _WorkTaskQueueList)
                {
                    //await workTaskQueueModel.StopAsync();
                    workTaskQueueModel.StopAsync().Wait();
                    workTaskQueueModel.Dispose();
                }

                if (_IsReadQueueAllData)
                {

                    _IsReadQueueAllData = false;

                    await foreach (var item in _CurrentChannel.Reader.ReadAllAsync())
                    {
                        _CurrentReadQueueAllDataList.Add(item);
                    }

                }

                await _CurrentChannel.Reader.Completion;

                _WorkTaskQueueList.Clear();

                _CurrentChannel = null;

                StateType = DynamicAsyncQueueStateTypeEnum.Stop;


            }

            //await Task.CompletedTask;

        }

        public override async Task<List<TDataModel>> StopAndReadQueueAllDataAsync()
        {

            _IsReadQueueAllData = true;

            await StopAsync();

            return _CurrentReadQueueAllDataList;

        }


        protected override async Task OnDisposeAsync()
        {

            if (!_CurrentReadQueueAllDataList.IfIsNullOrEmpty())
            {
                _CurrentReadQueueAllDataList.Clear();
                _CurrentReadQueueAllDataList = null;
            }

            await Task.CompletedTask;

        }



    }

}
