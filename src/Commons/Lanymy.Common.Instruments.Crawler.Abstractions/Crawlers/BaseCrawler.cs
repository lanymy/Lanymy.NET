using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Models;

namespace Lanymy.Common.Instruments.Crawlers
{

    public abstract class BaseCrawler<TKey, TCrawlerDataModel> : BaseWorkTask
        where TCrawlerDataModel : BaseCrawlerDataModel<TKey>
    {

        public string SpiderID => GetType().Name;

        public int TaskDelayMilliseconds { get; }
        public int WorkTaskTotalCount { get; }
        public int ChannelCapacityCount { get; }


        //private readonly string _CurrentWorkTaskDataContextFileToken;
        //protected readonly string _CurrentWorkTaskDataRootDirectoryFullPath;
        //protected readonly LanymyIsolatedStorage _CurrentLanymyIsolatedStorage;

        protected TimerWorkTask _CurrentProgressTimerWorkTask;
        protected WorkTaskQueue<TCrawlerDataModel> _CurrentWorkTaskQueue;
        protected readonly TaskProgressModel _CurrentTaskProgressModel = new TaskProgressModel();
        /// <summary>
        /// 总任务数
        /// </summary>
        protected int _CurrentTaskProgressTotalCount = 0;
        /// <summary>
        /// 已完成任务数
        /// </summary>
        protected int _CurrentTaskProgressCompleteCount = 0;

        protected readonly Action<TaskProgressModel> _TaskProgressAction;

        //protected BaseCrawlerN(string workTaskDataRootDirectoryFullPath = null, int taskDelayMilliseconds = 5 * 1000)
        protected BaseCrawler(Action<TaskProgressModel> taskProgressAction, int workTaskTotalCount, int taskDelayMilliseconds, int channelCapacityCount)
        {

            //if (workTaskDataRootDirectoryFullPath.IfIsNullOrEmpty())
            //{
            //    workTaskDataRootDirectoryFullPath = string.Empty;
            //}

            //_CurrentWorkTaskDataRootDirectoryFullPath = workTaskDataRootDirectoryFullPath;

            _TaskProgressAction = taskProgressAction;
            TaskDelayMilliseconds = taskDelayMilliseconds;
            WorkTaskTotalCount = workTaskTotalCount;
            ChannelCapacityCount = channelCapacityCount;

            //_CurrentWorkTaskDataContextFileToken = SpiderID + "_" + typeof(TCrawlerDataModel).Name;

            //_CurrentLanymyIsolatedStorage = new LanymyIsolatedStorage(_CurrentWorkTaskDataRootDirectoryFullPath);


        }





        protected virtual TimerWorkTaskDataResult OnProgressTimerWorkTask()
        {

            _CurrentTaskProgressModel.TotalCount = _CurrentTaskProgressTotalCount;
            _CurrentTaskProgressModel.CompleteCount = _CurrentTaskProgressCompleteCount;

            _TaskProgressAction(_CurrentTaskProgressModel);

            return null;

        }


        protected override async Task OnStartAsync()
        {

            _CurrentProgressTimerWorkTask = new TimerWorkTask(OnProgressTimerWorkTask, TaskDelayMilliseconds);
            await _CurrentProgressTimerWorkTask.StartAsync();

        }

        protected override async Task OnStopAsync()
        {

            await _CurrentProgressTimerWorkTask.StopAsync();
            _CurrentProgressTimerWorkTask.Dispose();
            _CurrentProgressTimerWorkTask = null;

        }



    }
}
