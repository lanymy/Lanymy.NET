using Lanymy.Common.ExtensionFunctions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 在普通队列消费基础上，增加“按数量 / 按时间”批量触发能力的任务队列。
    /// </summary>
    public abstract class BaseWorkTaskTriggerQueue<TDataModel> : BaseWorkTaskQueue<TDataModel>
    //where TDataModel : IWorkTaskQueueDataModel
    {
        /// <summary>
        /// 触发批处理前暂存的队列快照。
        /// </summary>
        protected readonly ConcurrentQueue<TDataModel> _CurrentCacheConcurrentQueue = new ConcurrentQueue<TDataModel>();
        protected Task _TimeTriggerTask;
        protected CancellationTokenSource _TimeTriggerTasktCancellationTokenSource;

        /// <summary>
        /// 指示当前是否正在执行触发回调，避免并发重入。
        /// </summary>
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

        /// <summary>
        /// 当前批量触发回调。
        /// </summary>
        protected readonly Action<List<TDataModel>> _CurrentWorkTaskTriggerQueueAction;


        protected BaseWorkTaskTriggerQueue(Channel<TDataModel> channel, Action<List<TDataModel>> workTriggerAction, ushort actionTriggerCount, TimeSpan actionTriggerTimeSpan, int taskSleepMilliseconds = 3000, int channelCapacityCount = 0, BoundedChannelFullMode channelFullMode = BoundedChannelFullMode.Wait)
            : base(channel, _ => { }, null, 1, taskSleepMilliseconds, channelCapacityCount, channelFullMode)
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
            if (_OnActionTriggerCountIndex < OnActionTriggerCount && (DateTime.Now - OnActionTriggerLastDateTime).TotalMilliseconds <= _OnActionTriggerMilliseconds)
            {
                return;
            }

            List<TDataModel> currentBatchDataList = null;
            var currentBatchCount = 0;
            var isTriggerActionSucceeded = false;

            lock (_Locker)
            {
                if (_IsWorkTriggerActionRun)
                {
                    return;
                }

                if (_OnActionTriggerCountIndex < OnActionTriggerCount && (DateTime.Now - OnActionTriggerLastDateTime).TotalMilliseconds <= _OnActionTriggerMilliseconds)
                {
                    return;
                }

                // 先做快照再执行回调，避免在回调执行期直接操作并发队列。
                currentBatchDataList = _CurrentCacheConcurrentQueue.ToList();
                currentBatchCount = currentBatchDataList.Count;
                _IsWorkTriggerActionRun = true;
            }

            try
            {
                if (currentBatchCount > 0)
                {
                    _CurrentWorkTaskTriggerQueueAction(currentBatchDataList);
                    isTriggerActionSucceeded = true;
                }
            }
            finally
            {
                lock (_Locker)
                {
                    if (isTriggerActionSucceeded && currentBatchCount > 0)
                    {
                        // 只有触发回调成功时才从缓存中剔除已处理批次，失败时保留现场等待下一次重试。
                        ClearTriggeredBatch(currentBatchCount);
                        var remainingTriggerCount = Math.Max(0, _OnActionTriggerCountIndex - currentBatchCount);
                        Interlocked.Exchange(ref _OnActionTriggerCountIndex, remainingTriggerCount);
                        OnActionTriggerLastDateTime = DateTime.Now;
                    }

                    _IsWorkTriggerActionRun = false;
                }
            }
        }

        private void ClearTriggeredBatch(int batchCount)
        {
#if NET48
            var dequeuedCount = 0;
            TDataModel item;
            while (dequeuedCount < batchCount && _CurrentCacheConcurrentQueue.TryDequeue(out item))
            {
                dequeuedCount++;
            }
#else
            var dequeuedCount = 0;
            while (dequeuedCount < batchCount && _CurrentCacheConcurrentQueue.TryDequeue(out _))
            {
                dequeuedCount++;
            }
#endif
        }

        private void FlushCachedDataOnStop()
        {
            if (_CurrentCacheConcurrentQueue.IsEmpty)
            {
                return;
            }

            // 通过抬高计数阈值强制执行一次最终触发，把停机前残留批次一起冲刷出去。
            var onActionTriggerCountIndex = _OnActionTriggerCountIndex + OnActionTriggerCount;
            Interlocked.Exchange(ref _OnActionTriggerCountIndex, onActionTriggerCountIndex);

            CheckOnActionTrigger();
        }


        protected override async Task OnStartAsync()
        {
            await base.OnStartAsync();

            _TimeTriggerTasktCancellationTokenSource = new CancellationTokenSource();
            var token = _TimeTriggerTasktCancellationTokenSource.Token;

            _TimeTriggerTask = Task.Factory.StartNew(
                () => OnTimeTriggerTaskAsync(token),
                token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default).Unwrap();

        }

        private async Task OnTimeTriggerTaskAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // 定时线程只负责检查是否达到触发条件，不直接参与消费主队列。
                CheckOnActionTrigger();

                await Task.Delay(TaskSleepMilliseconds, token);
            }
        }

        protected override async Task OnStopAsync()
        {
            Exception timeTriggerStopException = null;
            var currentTimeTriggerTask = _TimeTriggerTask;
            var currentTimeTriggerTaskCancellationTokenSource = _TimeTriggerTasktCancellationTokenSource;

            if (_TimeTriggerTasktCancellationTokenSource.IfIsNullOrEmpty())
            {
                try
                {
                    await base.OnStopAsync();
                }
                finally
                {
                    // 即使定时线程未启动成功，也要在 stop 末尾尽量冲刷掉缓存数据。
                    FlushCachedDataOnStop();
                }
                return;
            }


            try
            {
                currentTimeTriggerTaskCancellationTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                timeTriggerStopException = ex;
            }


            try
            {
                if (!currentTimeTriggerTask.IfIsNullOrEmpty())
                {
                    await currentTimeTriggerTask;
                }
            }
            catch (OperationCanceledException)
            {
                // ignored
            }
            catch (Exception ex)
            {
                timeTriggerStopException = ex;
            }
            finally
            {
                if (ReferenceEquals(_TimeTriggerTask, currentTimeTriggerTask))
                {
                    _TimeTriggerTask = null;
                }

                currentTimeTriggerTask?.Dispose();

                if (ReferenceEquals(_TimeTriggerTasktCancellationTokenSource, currentTimeTriggerTaskCancellationTokenSource))
                {
                    _TimeTriggerTasktCancellationTokenSource = null;
                }

                currentTimeTriggerTaskCancellationTokenSource?.Dispose();
            }

            try
            {
                await base.OnStopAsync();
            }
            finally
            {
                // worker 结束后再做最后一次批量触发，避免与正常消费并发交错。
                FlushCachedDataOnStop();
            }

            if (timeTriggerStopException != null)
            {
                throw timeTriggerStopException;
            }

        }



    }


}
