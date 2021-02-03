using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{


    public class WorkTaskQueueContext<TDataModel> : IDisposable
               where TDataModel : class
    {


        private readonly ConcurrentQueue<TDataModel> _CurrentConcurrentQueue;



        public bool IsClearQueueWhenStop { get; } = false;

        public DynamicAsyncQueueStateTypeEnum DynamicAsyncQueueStateType { get; private set; } = DynamicAsyncQueueStateTypeEnum.UnDefine;

        public int WorkTaskTotalCount => _WorkTaskQueueList.Count;

        private readonly List<WorkTaskQueue<TDataModel>> _WorkTaskQueueList = new List<WorkTaskQueue<TDataModel>>();

        //private int _CurrentAddQueueIndex = 0;

        public ushort TaskCountPerWorkAction { get; }


        //public WorkTaskQueueContext(bool isClearQueueWhenStop, ushort taskCountPerWorkAction, params Action<TDataModel>[] workActions)
        public WorkTaskQueueContext(ConcurrentQueue<TDataModel> currentConcurrentQueue, bool isClearQueueWhenStop, ushort taskCountPerWorkAction, Action<TDataModel> workAction)
        {

            _CurrentConcurrentQueue = currentConcurrentQueue;

            if (_CurrentConcurrentQueue.IfIsNull())
            {
                _CurrentConcurrentQueue = new ConcurrentQueue<TDataModel>();
            }

            //至少 一个 WorkAction
            if (taskCountPerWorkAction == 0)
            {
                taskCountPerWorkAction = 1;
            }

            IsClearQueueWhenStop = isClearQueueWhenStop;
            TaskCountPerWorkAction = taskCountPerWorkAction;

            if (workAction.IfIsNullOrEmpty())
            {
                workAction = dm => { };
            }

            for (var i = 0; i < TaskCountPerWorkAction; i++)
            {

                _WorkTaskQueueList.Add(new WorkTaskQueue<TDataModel>(_CurrentConcurrentQueue, IsClearQueueWhenStop,
                    dm =>
                    {

                        workAction(dm);

                    }));

            }


            DynamicAsyncQueueStateType = DynamicAsyncQueueStateTypeEnum.Stop;


        }


        public void AddToQueue(TDataModel data)
        {

            if (DynamicAsyncQueueStateType == DynamicAsyncQueueStateTypeEnum.Start)
            {

                _CurrentConcurrentQueue.Enqueue(data);

            }

        }


        public async Task StartAsync()
        {

            if (DynamicAsyncQueueStateType == DynamicAsyncQueueStateTypeEnum.Stop)
            {

                var list = _WorkTaskQueueList.Select(o => o.StartAsync()).ToList();


                //await Task.WhenAll(list.ToArray());
                Task.WhenAll(list.ToArray()).Wait();


                DynamicAsyncQueueStateType = DynamicAsyncQueueStateTypeEnum.Start;

            }

            await Task.CompletedTask;

        }




        public async Task StopAsync()
        {

            if (DynamicAsyncQueueStateType == DynamicAsyncQueueStateTypeEnum.Start)
            {

                DynamicAsyncQueueStateType = DynamicAsyncQueueStateTypeEnum.Cancel;

                foreach (var workTaskQueueModel in _WorkTaskQueueList)
                {
                    //await workTaskQueueModel.StopAsync();
                    workTaskQueueModel.StopAsync().Wait();
                }


                DynamicAsyncQueueStateType = DynamicAsyncQueueStateTypeEnum.Stop;


            }

            await Task.CompletedTask;

        }


        public void Dispose()
        {

            StopAsync().Wait();

        }

    }
}
