using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.Models;

namespace Lanymy.Common.Instruments.Crawlers
{
    /// <summary>
    /// 提供“外部投递下载任务 -> 内部并发处理”的下载型爬虫基类。
    /// </summary>
    public abstract class BaseDownloadCrawler<TKey, TCrawlerDataModel> : BaseCrawler<TKey, TCrawlerDataModel>
        where TCrawlerDataModel : BaseCrawlerDataModel<TKey>
    {
        /// <summary>
        /// 初始化下载爬虫。
        /// </summary>
        protected BaseDownloadCrawler(Action<TaskProgressModel> taskProgressAction, Action<List<TCrawlerDataModel>> stopAndReadQueueAllDataAction, int workTaskTotalCount, int taskDelayMilliseconds, int channelCapacityCount) : base(taskProgressAction, stopAndReadQueueAllDataAction, workTaskTotalCount, taskDelayMilliseconds, channelCapacityCount)
        {
        }

        protected override async Task OnStartAsync()
        {
            _CurrentWorkTaskQueue = new WorkTaskQueue<TCrawlerDataModel>(OnWorkTaskQueue, OnStopAndReadQueueAllDataAction, WorkTaskTotalCount, TaskDelayMilliseconds, ChannelCapacityCount);

            await _CurrentWorkTaskQueue.StartAsync();

            await base.OnStartAsync();
        }

        protected override async Task OnStopAsync()
        {
            await _CurrentWorkTaskQueue.StopAsync();

            _CurrentWorkTaskQueue.Dispose();
            _CurrentWorkTaskQueue = null;

            await base.OnStopAsync();
        }

        /// <summary>
        /// 向下载队列追加一条任务，并同步累计总任务数。
        /// </summary>
        /// <param name="crawlerDataModel">待下载的数据。</param>
        public async Task AddToDownloadAsync(TCrawlerDataModel crawlerDataModel)
        {
            if (IsRunning)
            {
                Interlocked.Increment(ref _CurrentTaskProgressTotalCount);
                await _CurrentWorkTaskQueue.AddToQueueAsync(crawlerDataModel);
            }
        }

        /// <summary>
        /// 执行具体下载逻辑。
        /// </summary>
        /// <param name="crawlerDataModel">当前下载任务。</param>
        protected abstract void OnDownload(TCrawlerDataModel crawlerDataModel);

        /// <summary>
        /// 工作队列消费入口，先累计完成数，再执行下载。
        /// </summary>
        /// <param name="crawlerDataModel">当前下载任务。</param>
        protected virtual void OnWorkTaskQueue(TCrawlerDataModel crawlerDataModel)
        {
            Interlocked.Increment(ref _CurrentTaskProgressCompleteCount);
            OnDownload(crawlerDataModel);
        }
    }
}
