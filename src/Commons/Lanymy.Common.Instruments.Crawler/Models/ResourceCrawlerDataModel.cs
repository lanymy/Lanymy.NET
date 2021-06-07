using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Models
{

    public class ResourceCrawlerDataModel : BaseCrawlerDataModel<string>
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public string Tags { get; set; }

        public string DetailInfo { get; set; }
        public DateTime CreateDateTime { get; set; }
    }

}
