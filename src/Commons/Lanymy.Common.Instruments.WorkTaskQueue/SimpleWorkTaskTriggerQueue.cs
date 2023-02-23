using System;
using System.Collections.Generic;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{

    public class SimpleWorkTaskTriggerQueue<TData> : SimpleWorkTaskQueue<TData>
    {

        private readonly List<TData> _CurrentList = new List<TData>();
        private volatile int _IndexCount = 0;
        private readonly int _TriggerCount;

        private readonly Action<IEnumerable<TData>> _TriggerWorkAction;

        //public SimpleWorkTaskTriggerQueue(Action<TData> workAction, Action<IEnumerable<TData>> triggerWorkAction, int triggerCount = 3) : base(workAction)
        public SimpleWorkTaskTriggerQueue(Action<IEnumerable<TData>> triggerWorkAction, int triggerCount = 3, int sleepIntervalMilliseconds = 10) : base(_ => { }, sleepIntervalMilliseconds)
        {

            if (triggerWorkAction.IfIsNull())
            {
                throw new ArgumentNullException(nameof(triggerWorkAction));
            }

            _TriggerWorkAction = triggerWorkAction;

            triggerCount--;
            if (triggerCount < 0)
            {
                triggerCount = 0;
            }

            _TriggerCount = triggerCount;

        }


        protected override void OnWorkAction(TData data)
        {

            //base.OnWorkAction(data);

            //Interlocked.Increment(ref _IndexCount);
            _IndexCount++;

            _CurrentList.Add(data);

            if (_IndexCount > _TriggerCount)
            {

                _TriggerWorkAction(_CurrentList);

                _CurrentList.Clear();
                //Interlocked.Exchange(ref _IndexCount, 0);
                _IndexCount = 0;

            }

        }

    }


}
