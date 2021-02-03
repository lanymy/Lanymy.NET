using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Lanymy.Common.Instruments
{

    public class WorkTaskTriggerQueue<TDataModel> : WorkTaskQueue<TDataModel>
            where TDataModel : class
    {


        //private object _Locker = new();

        private volatile bool _IsWorkTriggerActionRun = false;

        /// <summary>
        /// 触发方法的次数
        /// </summary>
        public ushort OnActionTriggerCount { get; }

        /// <summary>
        /// 触发方法的闲置时间
        /// </summary>
        public TimeSpan OnActionTriggerTimeSpan { get; set; }



        private uint _OnActionTriggerCountIndex = 0;
        private readonly uint _OnActionTriggerMilliseconds;


        private Action<List<TDataModel>> _WorkTriggerAction;

        public DateTime OnActionTriggerLastDateTime { get; private set; } = DateTime.Now;

        private ConcurrentQueue<TDataModel> _CurrentCacheConcurrentQueue = new();


        //public WorkTaskTriggerQueue(ConcurrentQueue<TDataModel> currentConcurrentQueue, bool isClearQueueWhenStop, ushort actionTriggerCount, TimeSpan actionTriggerTimeSpan, Action workTriggerAction, Action<TDataModel> workAction) : base(currentConcurrentQueue, isClearQueueWhenStop, workAction)
        public WorkTaskTriggerQueue(ConcurrentQueue<TDataModel> currentConcurrentQueue, bool isClearQueueWhenStop, ushort actionTriggerCount, TimeSpan actionTriggerTimeSpan, Action<List<TDataModel>> workTriggerAction) : base(currentConcurrentQueue, isClearQueueWhenStop, null)
        {

            if (actionTriggerCount == 0)
            {
                actionTriggerCount = 1;
            }

            if (actionTriggerTimeSpan.TotalSeconds < 3)
            {
                actionTriggerTimeSpan = TimeSpan.FromSeconds(3);
            }


            OnActionTriggerCount = actionTriggerCount;
            OnActionTriggerTimeSpan = actionTriggerTimeSpan;
            _OnActionTriggerMilliseconds = (uint)OnActionTriggerTimeSpan.TotalMilliseconds;
            _WorkTriggerAction = workTriggerAction;

        }


        private void CheckOnActionTrigger()
        {

            if (_OnActionTriggerCountIndex > OnActionTriggerCount || (DateTime.Now - OnActionTriggerLastDateTime).TotalMilliseconds > _OnActionTriggerMilliseconds)
            {

                _IsWorkTriggerActionRun = true;

                lock (_Locker)
                {


                    _WorkTriggerAction(_CurrentCacheConcurrentQueue.ToList());

                    _CurrentCacheConcurrentQueue.Clear();
                    Interlocked.Exchange(ref _OnActionTriggerCountIndex, 0);
                    OnActionTriggerLastDateTime = DateTime.Now;

                    _IsWorkTriggerActionRun = false;

                }

            }

        }

        protected override void OnWorkAction(TDataModel dataModel)
        {

            if (_IsWorkTriggerActionRun)
            {
                lock (_Locker)
                {
                    //base.OnWorkAction(dataModel);
                    _CurrentCacheConcurrentQueue.Enqueue(dataModel);
                }
            }
            else
            {
                //base.OnWorkAction(dataModel);
                _CurrentCacheConcurrentQueue.Enqueue(dataModel);
            }

            Interlocked.Increment(ref _OnActionTriggerCountIndex);

            CheckOnActionTrigger();

        }


        protected override void OnFinishSleep()
        {

            CheckOnActionTrigger();

        }

    }



}
