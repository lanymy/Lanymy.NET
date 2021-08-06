using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Models
{



    public class AnalysisResourceListResult<TKey, TCrawlerDataModel>
        where TCrawlerDataModel : BaseCrawlerDataModel<TKey>
    {

        public bool IsBreak { get; set; }

        public List<TCrawlerDataModel> AnalysisResourceList { get; set; }

    }


}
