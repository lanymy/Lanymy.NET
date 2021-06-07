using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Models
{


    /// <summary>
    /// 爬虫数据基类
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    public abstract class BaseCrawlerDataModel<TKey>
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public TKey ID { get; set; }

    }

}
