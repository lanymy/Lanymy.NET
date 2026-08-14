using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Models;

namespace Lanymy.Common.Instruments.Crawlers
{
    /// <summary>
    /// 定义爬虫任务的公共生命周期、进度统计和进度回调能力。
    /// </summary>
    public abstract class BaseCrawler<TKey, TCrawlerDataModel> : BaseWorkTask
        where TCrawlerDataModel : BaseCrawlerDataModel<TKey>
    {
        /// <summary>
        /// 当前爬虫实例标识，默认使用类型名。
        /// </summary>
        public string SpiderID => GetType().Name;

        /// <summary>
        /// 定时任务与工作队列之间共享的节流间隔。
        /// </summary>
        public int TaskDelayMilliseconds { get; }

        /// <summary>
        /// 工作队列并发工作数。
        /// </summary>
        public int WorkTaskTotalCount { get; }

        /// <summary>
        /// 内部通道容量；小于等于 0 时使用无界队列。
        /// </summary>
        public int ChannelCapacityCount { get; }


        //private readonly string _CurrentWorkTaskDataContextFileToken;
        //protected readonly string _CurrentWorkTaskDataRootDirectoryFullPath;
        //protected readonly LanymyIsolatedStorage _CurrentLanymyIsolatedStorage;

        protected TimerWorkTask _CurrentProgressTimerWorkTask;
        protected WorkTaskQueue<TCrawlerDataModel> _CurrentWorkTaskQueue;

        /// <summary>
        /// 复用的进度快照对象，定时回调时会刷新计数。
        /// </summary>
        protected readonly TaskProgressModel _CurrentTaskProgressModel = new TaskProgressModel();
        /// <summary>
        /// 总任务数
        /// </summary>
        protected int _CurrentTaskProgressTotalCount = 0;
        /// <summary>
        /// 已完成任务数
        /// </summary>
        protected int _CurrentTaskProgressCompleteCount = 0;

        protected readonly Action<TaskProgressModel> _CurrentTaskProgressAction;
        protected readonly Action<List<TCrawlerDataModel>> _CurrentStopAndReadQueueAllDataAction;

        /// <summary>
        /// 初始化爬虫基类。
        /// </summary>
        /// <param name="taskProgressAction">进度刷新回调。</param>
        /// <param name="stopAndReadQueueAllDataAction">停止时回传剩余队列数据的回调。</param>
        /// <param name="workTaskTotalCount">工作队列并发数。</param>
        /// <param name="taskDelayMilliseconds">轮询和节流间隔。</param>
        /// <param name="channelCapacityCount">内部通道容量。</param>
        protected BaseCrawler(Action<TaskProgressModel> taskProgressAction, Action<List<TCrawlerDataModel>> stopAndReadQueueAllDataAction, int workTaskTotalCount, int taskDelayMilliseconds, int channelCapacityCount)
        {

            //if (workTaskDataRootDirectoryFullPath.IfIsNullOrEmpty())
            //{
            //    workTaskDataRootDirectoryFullPath = string.Empty;
            //}

            //_CurrentWorkTaskDataRootDirectoryFullPath = workTaskDataRootDirectoryFullPath;

            _CurrentTaskProgressAction = taskProgressAction;
            _CurrentStopAndReadQueueAllDataAction = stopAndReadQueueAllDataAction;
            TaskDelayMilliseconds = taskDelayMilliseconds;
            WorkTaskTotalCount = workTaskTotalCount;
            ChannelCapacityCount = channelCapacityCount;

            //_CurrentWorkTaskDataContextFileToken = SpiderID + "_" + typeof(TCrawlerDataModel).Name;

            //_CurrentLanymyIsolatedStorage = new LanymyIsolatedStorage(_CurrentWorkTaskDataRootDirectoryFullPath);


        }

        /// <summary>
        /// 定时刷新当前任务进度。
        /// </summary>
        /// <returns>默认不打断定时任务。</returns>
        protected virtual TimerWorkTaskDataResult OnProgressTimerWorkTask()
        {
            _CurrentTaskProgressModel.TotalCount = _CurrentTaskProgressTotalCount;
            _CurrentTaskProgressModel.CompleteCount = _CurrentTaskProgressCompleteCount;

            _CurrentTaskProgressAction(_CurrentTaskProgressModel);

            return null;
        }

        /// <summary>
        /// 在停止阶段把尚未处理完的队列数据交给外部。
        /// </summary>
        /// <param name="queueAllDataList">停止时读取出的剩余数据。</param>
        protected virtual void OnStopAndReadQueueAllDataAction(List<TCrawlerDataModel> queueAllDataList)
        {
            if (!_CurrentStopAndReadQueueAllDataAction.IfIsNull())
            {
                _CurrentStopAndReadQueueAllDataAction(queueAllDataList);
            }
        }

        protected override async Task OnStartAsync()
        {
            _CurrentProgressTimerWorkTask = new TimerWorkTask(OnProgressTimerWorkTask, TaskDelayMilliseconds);
            await _CurrentProgressTimerWorkTask.StartAsync();
        }

        protected override async Task OnStopAsync()
        {
            await _CurrentProgressTimerWorkTask.StopAsync();
            _CurrentProgressTimerWorkTask.Dispose();
            _CurrentProgressTimerWorkTask = null;
        }
    }
}
