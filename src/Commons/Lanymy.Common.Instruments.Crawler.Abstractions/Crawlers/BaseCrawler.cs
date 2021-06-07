using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Models;

namespace Lanymy.Common.Instruments.Crawlers
{

    public abstract class BaseCrawler<TKey, TCrawlerDataModel, TCrawlerDataContext> : BaseWorkTask
        where TCrawlerDataModel : BaseCrawlerDataModel<TKey>
        where TCrawlerDataContext : BaseCrawlerDataContext<TKey, TCrawlerDataModel>, new()
    {

        public string SpiderID => GetType().Name;

        //protected readonly int _TaskDelayMilliseconds = 5 * 1000;
        protected readonly int _TaskDelayMilliseconds;

        protected TCrawlerDataContext _CurrentCrawlerDataContext;

        //protected readonly string _CurrentWorkTaskDataContextFileFullPath;

        private readonly string _CurrentWorkTaskDataContextFileToken;

        protected readonly string _CurrentWorkTaskDataRootDirectoryFullPath;

        private readonly LanymyIsolatedStorage _CurrentLanymyIsolatedStorage;

        protected BaseCrawler(string workTaskDataRootDirectoryFullPath = null, int taskDelayMilliseconds = 5 * 1000)
        {

            if (workTaskDataRootDirectoryFullPath.IfIsNullOrEmpty())
            {
                workTaskDataRootDirectoryFullPath = string.Empty;
            }

            _CurrentWorkTaskDataRootDirectoryFullPath = workTaskDataRootDirectoryFullPath;
            _TaskDelayMilliseconds = taskDelayMilliseconds;

            _CurrentWorkTaskDataContextFileToken = SpiderID + "_" + typeof(TCrawlerDataContext).Name;

            _CurrentLanymyIsolatedStorage = new LanymyIsolatedStorage(_CurrentWorkTaskDataRootDirectoryFullPath);

            //OnLoadWorkTaskDataContextAsync().Wait();
            LoadWorkTaskDataContextAsync().Wait();

        }


        private async Task LoadWorkTaskDataContextAsync()
        {
            var workTaskDataContext = await OnLoadWorkTaskDataContextAsync();
            if (workTaskDataContext.IfIsNullOrEmpty())
            {
                workTaskDataContext = new TCrawlerDataContext();
            }
            _CurrentCrawlerDataContext = workTaskDataContext;
        }


        private async Task SaveWorkTaskDataContextAsync()
        {

            await OnSaveWorkTaskDataContextAsync(_CurrentCrawlerDataContext);

        }





        protected virtual async Task<TCrawlerDataContext> OnLoadWorkTaskDataContextAsync()
        {

            var workTaskDataContext = _CurrentLanymyIsolatedStorage.GetModel<TCrawlerDataContext>(_CurrentWorkTaskDataContextFileToken);

            return await Task.FromResult(workTaskDataContext);

        }

        protected virtual async Task OnSaveWorkTaskDataContextAsync(TCrawlerDataContext crawlerDataContext)
        {

            _CurrentLanymyIsolatedStorage.SaveModel(crawlerDataContext, _CurrentWorkTaskDataContextFileToken);

            await Task.CompletedTask;

        }


        protected override async Task OnDisposeAsync()
        {

            await SaveWorkTaskDataContextAsync();

        }

    }

}
