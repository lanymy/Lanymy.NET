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


    public abstract class BaseResourceCrawler<TKey, TCrawlerDataModel> : BaseCrawler<TKey, TCrawlerDataModel>
        where TCrawlerDataModel : BaseCrawlerDataModel<TKey>
    {


        public abstract bool IsEnabled { get; set; }

        public string HostUrl { get; }

        protected TimerWorkTask _CurrentAnalysisResourceListTimerWorkTask;


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
        protected abstract void OnAnalysisResourceDetail(TCrawlerDataModel crawlerDataModel);


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
                Interlocked.Increment(ref _CurrentTaskProgressTotalCount);
                _CurrentWorkTaskQueue.AddToQueueAsync(crawlerDataModel).Wait();
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


        protected virtual void OnWorkTaskQueue(TCrawlerDataModel crawlerDataModel)
        {
            Interlocked.Increment(ref _CurrentTaskProgressCompleteCount);
            OnAnalysisResourceDetail(crawlerDataModel);
        }



    }


}
