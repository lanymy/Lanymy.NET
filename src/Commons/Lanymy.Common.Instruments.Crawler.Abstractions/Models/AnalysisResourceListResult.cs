using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Models
{
    /// <summary>
    /// 表示一次资源列表抓取的结果。
    /// </summary>
    /// <typeparam name="TKey">资源主键类型。</typeparam>
    /// <typeparam name="TCrawlerDataModel">资源数据模型类型。</typeparam>
    public class AnalysisResourceListResult<TKey, TCrawlerDataModel>
        where TCrawlerDataModel : BaseCrawlerDataModel<TKey>
    {
        /// <summary>
        /// 指示本轮抓取后是否中断后续定时轮询。
        /// </summary>
        public bool IsBreak { get; set; }

        /// <summary>
        /// 本轮抓取到、需要进入详情处理队列的资源集合。
        /// </summary>
        public List<TCrawlerDataModel> AnalysisResourceList { get; set; }
    }
}
