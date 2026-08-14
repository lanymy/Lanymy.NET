using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.Models;

namespace Lanymy.Common.Instruments.Crawlers
{
    /// <summary>
    /// 示例下载爬虫，演示如何把下载任务接入下载型爬虫基类。
    /// </summary>
    public class LanymyDownloadCrawler : BaseDownloadCrawler<Guid, BaseDownloadCrawlerDataModel>
    {
        /// <summary>
        /// 初始化示例下载爬虫。
        /// </summary>
        public LanymyDownloadCrawler(Action<TaskProgressModel> taskProgressAction, Action<List<BaseDownloadCrawlerDataModel>> stopAndReadQueueAllDataAction, int workTaskTotalCount = 3, int taskDelayMilliseconds = 3 * 1000, int channelCapacityCount = 0) : base(taskProgressAction, stopAndReadQueueAllDataAction, workTaskTotalCount, taskDelayMilliseconds, channelCapacityCount)
        {
        }

        protected override void OnDownload(BaseDownloadCrawlerDataModel crawlerDataModel)
        {
            //下载m3u8资源例子
            if (crawlerDataModel.ResourceType == ResourceTypeEnum.Video && crawlerDataModel.ResourceDownloadType == ResourceDownloadTypeEnum.M3u8)
            {
                try
                {

                    var ffmpegFileFullPath = "ffmpeg.exe 文件全路径";

                    // 这里只是示例：真实业务里通常会在这里补下载目录、任务状态持久化和失败重试。
                    using var lanymyFfmpeg = new LanymyFfmpeg(ffmpegFileFullPath);

                    var downloadUrl = crawlerDataModel.DownloadUrl;
                    var saveFileFullPath = "保存下载资源文件的全路径";

                    lanymyFfmpeg.SaveM3u8ToMp4File(downloadUrl, saveFileFullPath);

                }
                catch (Exception)
                {

                }
                finally
                {

                }
            }

            Thread.Sleep(TaskDelayMilliseconds);
        }

        protected override async Task OnDisposeAsync()
        {
            await Task.CompletedTask;
        }
    }
}
