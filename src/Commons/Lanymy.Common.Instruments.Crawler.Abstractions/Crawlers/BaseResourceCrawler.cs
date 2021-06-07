using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Models;

namespace Lanymy.Common.Instruments.Crawlers
{

    public abstract class BaseResourceCrawler<TKey, TCrawlerDataModel, TCrawlerDataContext> : BaseCrawler<TKey, TCrawlerDataModel, TCrawlerDataContext>
        where TCrawlerDataModel : BaseCrawlerDataModel<TKey>
        where TCrawlerDataContext : BaseCrawlerDataContext<TKey, TCrawlerDataModel>, new()
    {

        public abstract bool IsEnabled { get; set; }

        public string HostUrl { get; }

        private CancellationTokenSource _CurrentCancellationTokenSource;

        private Task _CurrentAnalysisResourceListTask;
        private Task _CurrentAnalysisResourceDetailTask;

        protected BaseResourceCrawler(string hostUrl)
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

            _CurrentCancellationTokenSource = new CancellationTokenSource();
            var token = _CurrentCancellationTokenSource.Token;

            _CurrentAnalysisResourceListTask = new Task(OnAnalysisResourceListTask, token, token, TaskCreationOptions.LongRunning);
            _CurrentAnalysisResourceListTask.Start();

            _CurrentAnalysisResourceDetailTask = new Task(OnAnalysisResourceDetailTask, token, token, TaskCreationOptions.LongRunning);
            _CurrentAnalysisResourceDetailTask.Start();


            await Task.CompletedTask;

        }


        protected override async Task OnStopAsync()
        {

            if (!_CurrentCancellationTokenSource.IfIsNullOrEmpty())
            {

                _CurrentCancellationTokenSource.Cancel();

                if (_CurrentAnalysisResourceDetailTask.Status == TaskStatus.Running)
                {
                    //await _CurrentAnalysisResourceDetailTask;
                    _CurrentAnalysisResourceDetailTask.Wait();
                }

                if (_CurrentAnalysisResourceListTask.Status == TaskStatus.Running)
                {
                    //await _CurrentAnalysisResourceListTask;
                    _CurrentAnalysisResourceListTask.Wait();

                }


            }


            if (!_CurrentAnalysisResourceDetailTask.IfIsNullOrEmpty())
            {
                _CurrentAnalysisResourceDetailTask.Dispose();
                _CurrentAnalysisResourceDetailTask = null;
            }


            if (!_CurrentAnalysisResourceListTask.IfIsNullOrEmpty())
            {
                _CurrentAnalysisResourceListTask.Dispose();
                _CurrentAnalysisResourceListTask = null;
            }


            if (!_CurrentCancellationTokenSource.IfIsNullOrEmpty())
            {
                _CurrentCancellationTokenSource.Dispose();
                _CurrentCancellationTokenSource = null;
            }

            await Task.CompletedTask;

        }



        /// <summary>
        /// 获取要解析明细页集合
        /// </summary>
        /// <param name="list">填充要解析明细页集合</param>
        /// <returns>是否中断此任务: True中断;False不中断继续执行下一次循环</returns>
        protected abstract bool OnAnalysisResourceList(List<TCrawlerDataModel> list);


        private void OnAnalysisResourceListTask(object obj)
        {


            var token = (CancellationToken)obj;
            var list = new List<TCrawlerDataModel>();

            while (!token.IsCancellationRequested)
            {

                Task.Delay(_TaskDelayMilliseconds).Wait();

                if (token.IsCancellationRequested) break;

                try
                {

                    list.Clear();

                    var isBreak = OnAnalysisResourceList(list);

                    foreach (var crawlerDataModel in list)
                    {
                        _CurrentCrawlerDataContext.Add(crawlerDataModel);
                    }

                    if (isBreak)
                    {
                        break;
                    }

                }
                catch
                {

                }

            }


        }


        protected abstract void OnAnalysisResourceDetail(TCrawlerDataModel crawlerDataModel);

        private void OnAnalysisResourceDetailTask(object obj)
        {


            var token = (CancellationToken)obj;

            while (!token.IsCancellationRequested)
            {

                Task.Delay(_TaskDelayMilliseconds).Wait();

                if (token.IsCancellationRequested) break;

                var crawlerDataModel = _CurrentCrawlerDataContext.Get();

                if (crawlerDataModel.IfIsNullOrEmpty()) continue;

                OnAnalysisResourceDetail(crawlerDataModel);

            }

        }






    }

}
