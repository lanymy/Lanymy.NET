using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CrawlerDataContextModels;
using Lanymy.Common.Instruments.Models;

namespace Lanymy.Common.Instruments.Crawlers
{


    public class LanymyResourceCrawler : BaseResourceCrawler<string, ResourceCrawlerDataModel, ResourceCrawlerDataContext>
    {

        public override bool IsEnabled { get; set; } = true;

        public LanymyResourceCrawler(string hostUrl) : base(hostUrl)
        {

        }


        /// <summary>
        /// 获取要解析明细页集合
        /// </summary>
        /// <param name="list">填充要解析明细页集合</param>
        /// <returns>是否中断此任务: True中断;False不中断继续执行下一次循环</returns>
        protected override bool OnAnalysisResourceList(List<ResourceCrawlerDataModel> list)
        {

            //解析一个http地址 获取 html 
            //解析分页列表信息
            //此次要做分页分批次获取列表逻辑
            //每次获取事件会自动触发

            //例子
            for (int i = 0; i < 5; i++)
            {
                list.Add(new ResourceCrawlerDataModel
                {
                    ID = i.ToString(),
                    CreateDateTime = DateTime.Now,
                });
            }

            //不中断定时器,等待下一次事件触发
            return false;

            //如果要中断此循环定时器 则 返回 true
            //return true;

        }


        protected override void OnAnalysisResourceDetail(ResourceCrawlerDataModel crawlerDataModel)
        {

            //处理明细页相关信息
            //crawlerDataModel.Url;

        }


    }

}
