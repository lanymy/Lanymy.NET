using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Models
{

    public abstract class BaseDownloadCrawlerDataModel : BaseCrawlerDataModel<Guid>
    {

        public ResourceTypeEnum ResourceType { get; set; }

        public ResourceDownloadTypeEnum ResourceDownloadType { get; set; }

        /// <summary>
        /// 下载地址
        /// </summary>
        public string DownloadUrl { get; set; }
        public DateTime CreateDateTime { get; set; }

    }

}
