using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.Crawlers;
using Lanymy.Common.Instruments.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class CrawlerTests
    {
        private sealed class TestCrawlerDataModel : BaseCrawlerDataModel<string>
        {
        }

        private sealed class ThrowingQueueResourceCrawler : BaseResourceCrawler<string, TestCrawlerDataModel>
        {
            public override bool IsEnabled { get; set; } = true;

            public ThrowingQueueResourceCrawler()
                : base("https://example.test", _ => { }, _ => { }, workTaskTotalCount: 1, taskDelayMilliseconds: 1, channelCapacityCount: 0)
            {
            }

            protected override AnalysisResourceListResult<string, TestCrawlerDataModel> OnAnalysisResourceList()
            {
                return new AnalysisResourceListResult<string, TestCrawlerDataModel>
                {
                    AnalysisResourceList = new List<TestCrawlerDataModel>
                    {
                        new TestCrawlerDataModel
                        {
                            ID = "1",
                        },
                    },
                };
            }

            protected override void OnAnalysisResourceDetail(TestCrawlerDataModel crawlerDataModel)
            {
            }

            protected override Task OnDisposeAsync()
            {
                return Task.CompletedTask;
            }

            protected override Task AddToQueueAsync(TestCrawlerDataModel crawlerDataModel)
            {
                return Task.FromException(new InvalidOperationException("crawler queue add failed"));
            }

            public void InvokeAnalysisTickForTest()
            {
                _ = base.OnAnalysisResourceListTimerWorkTask();
            }
        }

        [TestMethod]
        public void BaseResourceCrawler_OnAnalysisResourceListTimerWorkTask_WhenAddToQueueFails_ShouldThrowWrappedRootCause()
        {
            var crawler = new ThrowingQueueResourceCrawler();

            var exception = Assert.ThrowsExactly<InvalidOperationException>(() => crawler.InvokeAnalysisTickForTest());

            Assert.AreEqual("Crawler add to queue failed.", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.AreEqual("crawler queue add failed", exception.InnerException.Message);
        }
    }
}
