using Lanymy.Common.Instruments.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.Helpers;

namespace Lanymy.Common.Instruments.Crawlers
{
    /// <summary>
    /// 示例资源爬虫，演示如何定时抓列表并并发处理详情页。
    /// </summary>
    public class LanymyResourceCrawler : BaseResourceCrawler<string, ResourceCrawlerDataModel>
    {
        /// <summary>
        /// 是否启用当前示例爬虫。
        /// </summary>
        public override bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 初始化示例资源爬虫。
        /// </summary>
        public LanymyResourceCrawler(string hostUrl, Action<TaskProgressModel> taskProgressAction, Action<List<ResourceCrawlerDataModel>> stopAndReadQueueAllDataAction, int workTaskTotalCount = 1, int taskDelayMilliseconds = 3 * 1000, int channelCapacityCount = 0) : base(hostUrl, taskProgressAction, stopAndReadQueueAllDataAction, workTaskTotalCount, taskDelayMilliseconds, channelCapacityCount)
        {
        }


        /// <summary>
        /// 获取要解析明细页集合
        /// </summary>
        /// <returns>是否中断此任务: True中断;False不中断继续执行下一次循环</returns>
        protected override AnalysisResourceListResult<string, ResourceCrawlerDataModel> OnAnalysisResourceList()
        {
            //解析一个http地址 获取 html
            //解析分页列表信息
            //此次要做分页分批次获取列表逻辑
            //每次获取事件会自动触发

            //例子

            //Thread.Sleep(TaskDelayMilliseconds);
            Thread.Sleep(15 * 1000);

            var analysisResourceListResult = new AnalysisResourceListResult<string, ResourceCrawlerDataModel>();

            // 示例里保持循环常驻，让下一次定时触发继续抓取下一批数据。
            analysisResourceListResult.IsBreak = false;
            analysisResourceListResult.AnalysisResourceList = new List<ResourceCrawlerDataModel>();
            for (int i = 0; i < 10; i++)
            {
                analysisResourceListResult.AnalysisResourceList.Add(new ResourceCrawlerDataModel
                {
                    //ID = i.ToString(),
                    ID = Guid.NewGuid().ToString(),
                    CreateDateTime = DateTime.Now,
                });
            }

            return analysisResourceListResult;

            ////如果要中断此循环定时器 则 返回 true
            //analysisResourceListResult.IsBreak = true;

            //return analysisResourceListResult;
        }

        protected override void OnAnalysisResourceDetail(ResourceCrawlerDataModel crawlerDataModel)
        {
            // 真实业务里这里通常会拉详情页、提取下载地址并把结果转交给下载爬虫。
        }

        protected override void OnStopAndReadQueueAllDataAction(List<ResourceCrawlerDataModel> queueAllDataList)
        {
            // 这里预留了“停机落盘未完成任务”的扩展点，示例中不做具体实现。
        }

        protected override async Task OnStartAsync()
        {
            await base.OnStartAsync();

            ////挂载之前的未处理数据集合
            //var fileFullPath = Path.Combine(PathHelper.GetCallDomainPath(), "1.txt");
            //if (File.Exists(fileFullPath))
            //{

            //    await Task.Run(async () =>
            //    {

            //        var list = JsonSerializeHelper.DeserializeFromJsonFile<List<ResourceCrawlerDataModel>>(fileFullPath);

            //        foreach (var webPageTaskData in list)
            //        {
            //            //await _CurrentWorkTaskQueue.AddToQueueAsync(webPageTaskData);
            //            await AddToQueueAsync(webPageTaskData);
            //        }

            //        list.Clear();

            //    });

            //}
        }

        protected override async Task OnDisposeAsync()
        {
            await Task.CompletedTask;
        }
    }
}
