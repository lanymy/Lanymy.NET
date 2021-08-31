using System;
using System.Threading.Tasks;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.Crawlers;
using Lanymy.Common.Instruments.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests.Crawlers
{


    [TestClass()]
    public class LanymyResourceCrawlerTests
    {


        [TestMethod()]
        public void LanymyResourceCrawlerTest()
        {

            var lanymyResourceCrawler = new LanymyResourceCrawler("www.baidu.com",
                taskProgressModel =>
                {
                    var json = JsonSerializeHelper.SerializeToJson(taskProgressModel);
                });


            lanymyResourceCrawler.StartAsync().Wait();


            Task.Delay(24 * 60 * 60 * 1000).Wait();
            //Task.Delay(10 * 1000).Wait();


            lanymyResourceCrawler.StopAsync().Wait();

            //Task.Delay(24 * 60 * 60 * 1000).Wait();



            //var lanymyDownloadCrawler = new LanymyDownloadCrawler(
            //    taskProgressModel =>
            //    {
            //        var json = JsonSerializeHelper.SerializeToJson(taskProgressModel);
            //    });

            //lanymyDownloadCrawler.StartAsync().Wait();

            //int count = 10;
            ////
            //var tasks = new Task[]
            //{
            //    Task.Run(() =>
            //    {
            //        for (int i = 0; i < count; i++)
            //        {
            //            lanymyDownloadCrawler.AddToDownloadAsync(new ImageDownloadCrawlerDataModel
            //            {
            //                ID = Guid.NewGuid(),
            //                CreateDateTime = DateTime.Now,
            //                ResourceType = ResourceTypeEnum.Image,
            //                ResourceDownloadType = ResourceDownloadTypeEnum.Jpg,
            //            }).Wait();
            //        }
            //    }),


            //    //Task.Run(() =>
            //    //{
            //    //    for (int i = 0; i < count; i++)
            //    //    {
            //    //        lanymyDownloadCrawler.AddToDownloadAsync(new ImageDownloadCrawlerDataModel
            //    //        {
            //    //            ID = Guid.NewGuid(),
            //    //            CreateDateTime = DateTime.Now,
            //    //            ResourceType = ResourceTypeEnum.Image,
            //    //            ResourceDownloadType = ResourceDownloadTypeEnum.Jpg,
            //    //        }).Wait();
            //    //    }
            //    //}),

            //    //Task.Run(() =>
            //    //{
            //    //    for (int i = 0; i < count; i++)
            //    //    {
            //    //        lanymyDownloadCrawler.AddToDownloadAsync(new ImageDownloadCrawlerDataModel
            //    //        {
            //    //            ID = Guid.NewGuid(),
            //    //            CreateDateTime = DateTime.Now,
            //    //            ResourceType = ResourceTypeEnum.Image,
            //    //            ResourceDownloadType = ResourceDownloadTypeEnum.Jpg,
            //    //        }).Wait();
            //    //    }
            //    //}),

            //    //Task.Run(() =>
            //    //{
            //    //    for (int i = 0; i < count; i++)
            //    //    {
            //    //        lanymyDownloadCrawler.AddToDownloadAsync(new ImageDownloadCrawlerDataModel
            //    //        {
            //    //            ID = Guid.NewGuid(),
            //    //            CreateDateTime = DateTime.Now,
            //    //            ResourceType = ResourceTypeEnum.Image,
            //    //            ResourceDownloadType = ResourceDownloadTypeEnum.Jpg,
            //    //        }).Wait();
            //    //    }
            //    //}),

            //};

            ////Task.WhenAll(tasks).Wait();

            ////Task.Delay(24 * 60 * 60 * 1000).Wait();
            //Task.Delay(2 * 1000).Wait();


            //lanymyDownloadCrawler.StopAsync().Wait();

            ////Task.Delay(24 * 60 * 60 * 1000).Wait();


        }


    }
}