using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.Models;

namespace Lanymy.Common.Instruments.Crawlers
{



    public abstract class BaseDownloadCrawler<TKey, TCrawlerDataModel> : BaseCrawler<TKey, TCrawlerDataModel>
        where TCrawlerDataModel : BaseCrawlerDataModel<TKey>
    {


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


        public async Task AddToDownloadAsync(TCrawlerDataModel crawlerDataModel)
        {

            if (IsRunning)
            {
                Interlocked.Increment(ref _CurrentTaskProgressTotalCount);
                await _CurrentWorkTaskQueue.AddToQueueAsync(crawlerDataModel);
            }

        }


        protected abstract void OnDownload(TCrawlerDataModel crawlerDataModel);


        protected virtual void OnWorkTaskQueue(TCrawlerDataModel crawlerDataModel)
        {
            Interlocked.Increment(ref _CurrentTaskProgressCompleteCount);
            OnDownload(crawlerDataModel);
        }




    }
}
