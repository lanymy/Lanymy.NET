using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.Models;

namespace Lanymy.Common.Instruments.Crawlers
{


    public abstract class BaseDownloadCrawler<TKey, TCrawlerDataModel, TCrawlerDataContext> : BaseCrawler<TKey, TCrawlerDataModel, TCrawlerDataContext>
        where TCrawlerDataModel : BaseCrawlerDataModel<TKey>
        where TCrawlerDataContext : BaseCrawlerDataContext<TKey, TCrawlerDataModel>, new()
    {

        protected readonly WorkTaskQueueContext<TCrawlerDataModel> _CurrentWorkTaskQueueContext;

        /// <summary>
        /// 同时下载数 默认值 3
        /// </summary>
        /// <param name="taskCountDownload"></param>
        protected BaseDownloadCrawler(ushort taskCountDownload = 3)
        {

            _CurrentWorkTaskQueueContext = new WorkTaskQueueContext<TCrawlerDataModel>(_CurrentCrawlerDataContext.CurrentCrawlerDataQueue, false, taskCountDownload, OnDownloadTask);

        }


        protected abstract void OnDownload(TCrawlerDataModel crawlerDataModel);

        private void OnDownloadTask(TCrawlerDataModel crawlerDataModel)
        {
            OnDownload(crawlerDataModel);
        }


        protected override async Task OnStartAsync()
        {

            //await _CurrentWorkTaskQueueContext.StartAsync();
            _CurrentWorkTaskQueueContext.StartAsync().Wait();

            await Task.CompletedTask;

        }


        protected override async Task OnStopAsync()
        {

            //await _CurrentWorkTaskQueueContext.StopAsync();
            _CurrentWorkTaskQueueContext.StopAsync().Wait();

            await Task.CompletedTask;

        }

        public virtual void AddToDownloadQueue(TCrawlerDataModel crawlerDataModel)
        {

            _CurrentCrawlerDataContext.Add(crawlerDataModel);
            //_CurrentCrawlerDataContext.CurrentCrawlerDataQueue.Enqueue(crawlerDataModel);

        }


    }



}
