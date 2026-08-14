using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Models
{
    /// <summary>
    /// 表示下载型爬虫的基础任务数据。
    /// </summary>
    public class BaseDownloadCrawlerDataModel : BaseCrawlerDataModel<Guid>
    {
        /// <summary>
        /// 资源类型，例如图片、视频等。
        /// </summary>
        public ResourceTypeEnum ResourceType { get; set; }

        /// <summary>
        /// 下载方式，例如直链或 m3u8。
        /// </summary>
        public ResourceDownloadTypeEnum ResourceDownloadType { get; set; }

        /// <summary>
        /// 下载地址
        /// </summary>
        public string DownloadUrl { get; set; }

        /// <summary>
        /// 任务创建时间。
        /// </summary>
        public DateTime CreateDateTime { get; set; }
    }
}
