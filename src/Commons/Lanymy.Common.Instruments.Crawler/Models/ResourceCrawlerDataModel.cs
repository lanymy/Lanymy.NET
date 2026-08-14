using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Models
{
    /// <summary>
    /// 表示资源列表和详情页之间流转的基础资源数据。
    /// </summary>
    public class ResourceCrawlerDataModel : BaseCrawlerDataModel<string>
    {
        /// <summary>
        /// 资源标题。
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 资源详情地址。
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 资源标签文本。
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 资源详情描述。
        /// </summary>
        public string DetailInfo { get; set; }

        /// <summary>
        /// 资源采集时间。
        /// </summary>
        public DateTime CreateDateTime { get; set; }
    }
}
