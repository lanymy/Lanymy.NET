using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Models
{


    public abstract class BaseCrawlerDataContext<TKey, TCrawlerDataModel> : BaseWorkTaskDataContext
        where TCrawlerDataModel : BaseCrawlerDataModel<TKey>
    {

        public ConcurrentQueue<TCrawlerDataModel> CurrentCrawlerDataQueue { get; set; } = new();


        public virtual void Add(TCrawlerDataModel crawlerDataModel)
        {
            CurrentCrawlerDataQueue.Enqueue(crawlerDataModel);
        }

        public virtual TCrawlerDataModel Get()
        {

            if (!CurrentCrawlerDataQueue.TryDequeue(out var result))
            {
                result = null;
            }

            return result;

        }

    }

}
