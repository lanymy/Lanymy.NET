﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CrawlerDataContextModels;
using Lanymy.Common.Instruments.Models;

namespace Lanymy.Common.Instruments.Crawlers
{


    public class LanymyDownloadCrawler : BaseDownloadCrawler<Guid, BaseDownloadCrawlerDataModel, DownloadCrawlerDataContext>
    {


        /// <summary>
        /// 下载资源处理逻辑
        /// </summary>
        /// <param name="crawlerDataModel"></param>
        protected override void OnDownload(BaseDownloadCrawlerDataModel crawlerDataModel)
        {

            //下载m3u8资源例子
            if (crawlerDataModel.ResourceType == ResourceTypeEnum.Video && crawlerDataModel.ResourceDownloadType == ResourceDownloadTypeEnum.M3u8)
            {
                try
                {

                    var ffmpegFileFullPath = "ffmpeg.exe 文件全路径";

                    //处理下载任务
                    using var lanymyFfmpeg = new LanymyFfmpeg(ffmpegFileFullPath);

                    var downloadUrl = crawlerDataModel.DownloadUrl;
                    var saveFileFullPath = "保存下载资源文件的全路径";

                    lanymyFfmpeg.SaveM3u8ToMp4File(downloadUrl, saveFileFullPath);

                }
                catch (Exception e)
                {

                }
                finally
                {

                }
            }


            Task.Delay(_TaskDelayMilliseconds).Wait();

        }


    }
}
