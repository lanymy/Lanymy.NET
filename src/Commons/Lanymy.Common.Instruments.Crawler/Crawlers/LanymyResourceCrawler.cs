using Lanymy.Common.Instruments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Crawlers
{


    public class LanymyResourceCrawler : BaseResourceCrawler<string, ResourceCrawlerDataModel>
    {


        public override bool IsEnabled { get; set; } = true;


        public LanymyResourceCrawler(string hostUrl, Action<TaskProgressModel> taskProgressAction, int workTaskTotalCount = 1, int taskDelayMilliseconds = 3 * 1000, int channelCapacityCount = 0) : base(hostUrl, taskProgressAction, workTaskTotalCount, taskDelayMilliseconds, channelCapacityCount)
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

            Task.Delay(TaskDelayMilliseconds).Wait();

            var analysisResourceListResult = new AnalysisResourceListResult<string, ResourceCrawlerDataModel>();

            ////不中断定时器,等待下一次事件触发
            //analysisResourceListResult.IsBreak = false;
            //analysisResourceListResult.AnalysisResourceList = new List<ResourceCrawlerDataModel>();
            //for (int i = 0; i < 5; i++)
            //{
            //    analysisResourceListResult.AnalysisResourceList.Add(new ResourceCrawlerDataModel
            //    {
            //        ID = i.ToString(),
            //        CreateDateTime = DateTime.Now,
            //    });
            //}

            //return analysisResourceListResult;

            analysisResourceListResult.IsBreak = true;

            //如果要中断此循环定时器 则 返回 true
            return analysisResourceListResult;

        }


        protected override void OnAnalysisResourceDetail(ResourceCrawlerDataModel crawlerDataModel)
        {
            //处理明细页相关信息
            //crawlerDataModel.Url;
        }


        protected override async Task OnDisposeAsync()
        {
            await Task.CompletedTask;
        }


    }


}
