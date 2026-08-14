using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Models;

namespace Lanymy.Common.Instruments.Crawlers
{
    /// <summary>
    /// 提供“定时抓列表 + 并发跑详情”的资源型爬虫基类。
    /// </summary>
    public abstract class BaseResourceCrawler<TKey, TCrawlerDataModel> : BaseCrawler<TKey, TCrawlerDataModel>
        where TCrawlerDataModel : BaseCrawlerDataModel<TKey>
    {
        /// <summary>
        /// 指示当前爬虫是否启用；禁用时启动会直接退出。
        /// </summary>
        public abstract bool IsEnabled { get; set; }

        /// <summary>
        /// 当前资源站点入口地址。
        /// </summary>
        public string HostUrl { get; }

        /// <summary>
        /// 周期性抓取资源列表的定时任务。
        /// </summary>
        protected TimerWorkTask _CurrentAnalysisResourceListTimerWorkTask;

        /// <summary>
        /// 初始化资源爬虫。
        /// </summary>
        protected BaseResourceCrawler(string hostUrl, Action<TaskProgressModel> taskProgressAction, Action<List<TCrawlerDataModel>> stopAndReadQueueAllDataAction, int workTaskTotalCount, int taskDelayMilliseconds, int channelCapacityCount) : base(taskProgressAction, stopAndReadQueueAllDataAction, workTaskTotalCount, taskDelayMilliseconds, channelCapacityCount)
        {
            HostUrl = hostUrl;
        }

        protected override async Task OnStartAsync()
        {
            if (!IsEnabled)
            {
                IsRunning = false;
                return;
            }

            _CurrentAnalysisResourceListTimerWorkTask = new TimerWorkTask(OnAnalysisResourceListTimerWorkTask, TaskDelayMilliseconds);
            _CurrentWorkTaskQueue = new WorkTaskQueue<TCrawlerDataModel>(OnWorkTaskQueue, OnStopAndReadQueueAllDataAction, WorkTaskTotalCount, TaskDelayMilliseconds, ChannelCapacityCount);

            await _CurrentWorkTaskQueue.StartAsync();

            await _CurrentAnalysisResourceListTimerWorkTask.StartAsync();

            await base.OnStartAsync();
        }

        protected override async Task OnStopAsync()
        {
            await _CurrentAnalysisResourceListTimerWorkTask.StopAsync();
            await _CurrentWorkTaskQueue.StopAsync();

            _CurrentAnalysisResourceListTimerWorkTask.Dispose();
            _CurrentAnalysisResourceListTimerWorkTask = null;

            _CurrentWorkTaskQueue.Dispose();
            _CurrentWorkTaskQueue = null;

            await base.OnStopAsync();
        }

        /// <summary>
        /// 获取要解析明细页集合
        /// </summary>
        /// <returns>是否中断此任务: True中断;False不中断继续执行下一次循环</returns>
        protected abstract AnalysisResourceListResult<TKey, TCrawlerDataModel> OnAnalysisResourceList();

        /// <summary>
        /// 处理单条详情页资源。
        /// </summary>
        /// <param name="crawlerDataModel">当前详情页数据。</param>
        protected abstract void OnAnalysisResourceDetail(TCrawlerDataModel crawlerDataModel);

        /// <summary>
        /// 定时抓取资源列表并投递到详情处理队列。
        /// </summary>
        /// <returns>需要中断后续轮询时返回带 <c>IsBreak</c> 标记的结果。</returns>
        protected virtual TimerWorkTaskDataResult OnAnalysisResourceListTimerWorkTask()
        {
            var analysisResourceListResult = OnAnalysisResourceList();

            //if (analysisResourceListResult.IsBreak)
            //{

            //    //_CurrentAnalysisResourceListTimerWorkTask.StopAsync().Wait();
            //    return new TimerWorkTaskDataResult
            //    {
            //        IsBreak = true,
            //    };

            //}


            if (analysisResourceListResult.AnalysisResourceList.IfIsNull())
            {
                analysisResourceListResult.AnalysisResourceList = new List<TCrawlerDataModel>();
            }

            foreach (var crawlerDataModel in analysisResourceListResult.AnalysisResourceList)
            {
                // 这里走同步桥接是为了让定时器回调保持串行语义，同时把 AddToQueue 的根因异常包一层统一上抛。
                var addToQueueException = TaskHelper.TrySyncWait(() => AddToQueueAsync(crawlerDataModel));
                if (addToQueueException != null)
                {
                    throw new InvalidOperationException("Crawler add to queue failed.", addToQueueException);
                }
            }

            analysisResourceListResult.AnalysisResourceList.Clear();
            analysisResourceListResult.AnalysisResourceList = null;

            if (analysisResourceListResult.IsBreak)
            {

                //_CurrentAnalysisResourceListTimerWorkTask.StopAsync().Wait();
                return new TimerWorkTaskDataResult
                {
                    IsBreak = true,
                };

            }

            return null;
        }

        /// <summary>
        /// 向详情处理队列追加一条任务，并同步累计总任务数。
        /// </summary>
        /// <param name="crawlerDataModel">待处理的详情页数据。</param>
        protected virtual async Task AddToQueueAsync(TCrawlerDataModel crawlerDataModel)
        {
            Interlocked.Increment(ref _CurrentTaskProgressTotalCount);
            await _CurrentWorkTaskQueue.AddToQueueAsync(crawlerDataModel);
        }

        /// <summary>
        /// 详情处理队列消费入口，先累计完成数，再执行详情解析。
        /// </summary>
        /// <param name="crawlerDataModel">当前详情页数据。</param>
        protected virtual void OnWorkTaskQueue(TCrawlerDataModel crawlerDataModel)
        {
            Interlocked.Increment(ref _CurrentTaskProgressCompleteCount);
            OnAnalysisResourceDetail(crawlerDataModel);
        }
    }
}
