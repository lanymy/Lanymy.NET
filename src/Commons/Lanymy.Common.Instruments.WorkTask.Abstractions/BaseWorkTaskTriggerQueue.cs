using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{


    public abstract class BaseWorkTaskTriggerQueue<TDataModel> : BaseWorkTaskQueue<TDataModel>
        where TDataModel : IWorkTaskQueueDataModel
    {


        protected readonly ConcurrentQueue<TDataModel> _CurrentCacheConcurrentQueue = new ConcurrentQueue<TDataModel>();
        protected Task _TimeTriggerTask;
        protected CancellationTokenSource _TimeTriggerTasktCancellationTokenSource;


        protected volatile bool _IsWorkTriggerActionRun = false;

        /// <summary>
        /// 触发事件的数据数量阀值,如: 每1000个数据量 触发一次事件
        /// </summary>
        public ushort OnActionTriggerCount { get; private set; }
        //protected uint _OnActionTriggerCountIndex = 0;
        protected int _OnActionTriggerCountIndex = 0;

        /// <summary>
        ///  触发事件的闲置时间,如: 消息队列内数据只有100个数据不满足 数据数量触发阀值条件,并且 5分钟 内 数据数量依然不满足触发条件的时候,满足5分钟定时触发条件
        /// </summary>
        public TimeSpan OnActionTriggerTimeSpan { get; private set; }

        protected readonly uint _OnActionTriggerMilliseconds;
        public DateTime OnActionTriggerLastDateTime { get; private set; } = DateTime.Now;


        protected readonly Action<List<TDataModel>> _CurrentWorkTaskTriggerQueueAction;


        protected BaseWorkTaskTriggerQueue(Channel<TDataModel> channel, Action<List<TDataModel>> workTriggerAction, ushort actionTriggerCount, TimeSpan actionTriggerTimeSpan, int taskSleepMilliseconds = 3000, int channelCapacityCount = 0, BoundedChannelFullMode channelFullMode = BoundedChannelFullMode.Wait)
            : base(channel, _ => { }, 1, taskSleepMilliseconds, channelCapacityCount, channelFullMode)
        {

            if (workTriggerAction.IfIsNull())
            {
                throw new ArgumentNullException(nameof(workTriggerAction));
            }

            if (actionTriggerCount == 0)
            {
                actionTriggerCount = 1;
            }

            //if (actionTriggerTimeSpan.TotalSeconds < 3)
            //{
            //    actionTriggerTimeSpan = TimeSpan.FromSeconds(3);
            //}
            if (actionTriggerTimeSpan.TotalSeconds < 1)
            {
                actionTriggerTimeSpan = TimeSpan.FromSeconds(1);
            }

            OnActionTriggerCount = actionTriggerCount;
            OnActionTriggerTimeSpan = actionTriggerTimeSpan;
            _OnActionTriggerMilliseconds = (uint)OnActionTriggerTimeSpan.TotalMilliseconds;
            _CurrentWorkTaskTriggerQueueAction = workTriggerAction;

        }



        protected override void OnWorkAction(TDataModel dataModel)
        {

            if (_IsWorkTriggerActionRun)
            {
                lock (_Locker)
                {
                    _CurrentCacheConcurrentQueue.Enqueue(dataModel);
                }
            }
            else
            {
                _CurrentCacheConcurrentQueue.Enqueue(dataModel);
            }

            Interlocked.Increment(ref _OnActionTriggerCountIndex);

            CheckOnActionTrigger();

        }


        private void CheckOnActionTrigger()
        {

            if (_OnActionTriggerCountIndex >= OnActionTriggerCount || (DateTime.Now - OnActionTriggerLastDateTime).TotalMilliseconds > _OnActionTriggerMilliseconds)
            {


                lock (_Locker)
                {

                    _IsWorkTriggerActionRun = true;

                    if (!_CurrentCacheConcurrentQueue.IsEmpty)
                    {
                        _CurrentWorkTaskTriggerQueueAction(_CurrentCacheConcurrentQueue.ToList());
                    }

                    _CurrentCacheConcurrentQueue.Clear();
                    Interlocked.Exchange(ref _OnActionTriggerCountIndex, 0);
                    OnActionTriggerLastDateTime = DateTime.Now;

                    _IsWorkTriggerActionRun = false;

                }

            }

        }


        protected override async Task OnStartAsync()
        {
            await base.OnStartAsync();


            _TimeTriggerTasktCancellationTokenSource = new CancellationTokenSource();
            var token = _TimeTriggerTasktCancellationTokenSource.Token;

            var _TimeTriggerTask = new Task(OnTimeTriggerTask, token, token, TaskCreationOptions.LongRunning);
            _TimeTriggerTask.Start();

        }

        private void OnTimeTriggerTask(object obj)
        {
            var token = (CancellationToken)obj;

            while (!token.IsCancellationRequested)
            {

                CheckOnActionTrigger();

                Task.Delay(TaskSleepMilliseconds).Wait();

            }
        }

        protected override async Task OnStopAsync()
        {

            await base.OnStopAsync();

            var onActionTriggerCountIndex = _OnActionTriggerCountIndex + OnActionTriggerCount;
            Interlocked.Exchange(ref _OnActionTriggerCountIndex, onActionTriggerCountIndex);

            if (_TimeTriggerTasktCancellationTokenSource.IfIsNullOrEmpty())
            {
                return;
            }


            _TimeTriggerTasktCancellationTokenSource.Cancel();


            if (_TimeTriggerTask.Status == TaskStatus.Running)
            {
                _TimeTriggerTask.Wait();
            }

            _TimeTriggerTask.Dispose();



            if (!_TimeTriggerTasktCancellationTokenSource.IfIsNullOrEmpty())
            {
                _TimeTriggerTasktCancellationTokenSource.Dispose();
                _TimeTriggerTasktCancellationTokenSource = null;
            }

        }



    }


}
