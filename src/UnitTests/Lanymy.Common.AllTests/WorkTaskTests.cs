using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lanymy.Common.Abstractions.Models;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.CryptoModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{



    [TestClass()]
    public class WorkTaskTests
    {


        class WorkTaskQueueDataModel : IWorkTaskQueueDataModel
        {

            public int Index { get; set; }

        }

        class TestSimpleWorkTask : BaseSimpleWorkTask
        {
            public TestSimpleWorkTask(Action<CancellationToken> workAction, int sleepIntervalMilliseconds = 0)
                : base(workAction, sleepIntervalMilliseconds)
            {
            }

            protected override async Task OnDisposeAsync()
            {
                await Task.CompletedTask;
            }
        }

        class TestExternalChannelWorkTaskQueue : BaseWorkTaskQueue<WorkTaskQueueDataModel>
        {
            public TestExternalChannelWorkTaskQueue(Channel<WorkTaskQueueDataModel> channel, Action<List<WorkTaskQueueDataModel>> stopAndReadQueueAllDataAction)
                : base(channel, _ => { }, stopAndReadQueueAllDataAction, taskSleepMilliseconds: 50)
            {
            }
        }

        class TestWorkTaskTriggerQueue : BaseWorkTaskTriggerQueue<WorkTaskQueueDataModel>
        {
            public TestWorkTaskTriggerQueue(Action<List<WorkTaskQueueDataModel>> workTriggerAction, ushort actionTriggerCount, TimeSpan actionTriggerTimeSpan, int taskSleepMilliseconds = 50)
                : base(null, workTriggerAction, actionTriggerCount, actionTriggerTimeSpan, taskSleepMilliseconds)
            {
            }

            public int CachedCount => _CurrentCacheConcurrentQueue.Count;
        }


        [TestMethod()]
        public async Task WorkTaskTest()
        {

            //var channel = Channel.CreateUnbounded<int>();

            //for (var i = 0; i < 10; i++)
            //{
            //    await channel.Writer.WriteAsync(i);
            //}


            //var reader = channel.Reader;

            //if (reader.TryRead(out var number))
            //{
            //    Debug.WriteLine(number);
            //}

            //var canCount = reader.CanCount;
            //var count = reader.Count;

            //var list = new List<int>();

            //await foreach (var item in reader.ReadAllAsync())
            //{
            //    list.Add(item);
            //    if (item == 8)
            //    {
            //        channel.Writer.Complete();
            //    }
            //}

            //canCount = reader.CanCount;
            //count = reader.Count;

            //if (reader.TryRead(out number))
            //{
            //    Debug.WriteLine(number);
            //}


            ////while (await reader.WaitToReadAsync())
            ////{
            ////    if (reader.TryRead(out var number))
            ////    {
            ////        Debug.WriteLine(number);
            ////    }
            ////}

            var workTaskQueue = new WorkTaskQueue<WorkTaskQueueDataModel>
            (
                dataModel =>
                {
                    Task.Delay(10 * 1000).Wait();
                    Debug.WriteLine(dataModel.Index);
                },
                dataList =>
                {
                    Assert.AreEqual(dataList[0].Index, 1);
                    Assert.AreEqual(dataList.Count, 9);
                }
            );

            await workTaskQueue.StartAsync();

            for (var i = 0; i < 10; i++)
            {
                await workTaskQueue.AddToQueueAsync(new WorkTaskQueueDataModel
                {
                    Index = i,
                });
            }

            await Task.Delay(5 * 1000);
            //await Task.Delay(10000 * 1000);

            await workTaskQueue.StopAsync();

            var strEnd = string.Empty;



        }

        [TestMethod()]
        public async Task BaseSimpleWorkTask_StopAsync_ShouldNotThrowAfterStart()
        {
            using var stopSignal = new ManualResetEventSlim(false);
            using var workEnteredSignal = new ManualResetEventSlim(false);

            using var workTask = new TestSimpleWorkTask(token =>
            {
                workEnteredSignal.Set();
                stopSignal.Wait(token);
            });

            await workTask.StartAsync();

            Assert.IsTrue(workEnteredSignal.Wait(TimeSpan.FromSeconds(2)));

            stopSignal.Set();
            await workTask.StopAsync();
        }

        [TestMethod()]
        public async Task WorkTaskTriggerQueue_StopAsync_ShouldNotThrowAfterStart()
        {
            var queue = new WorkTaskTriggerQueue<WorkTaskQueueDataModel>
            (
                _ => { },
                actionTriggerCount: 1,
                actionTriggerTimeSpan: TimeSpan.FromSeconds(1),
                taskSleepMilliseconds: 50
            );

            await queue.StartAsync();
            await queue.StopAsync();
        }

        [TestMethod()]
        public async Task WorkTaskQueue_Dispose_ShouldStopRunningTask()
        {
            var queue = new WorkTaskQueue<WorkTaskQueueDataModel>
            (
                _ => { },
                _ => { },
                taskSleepMilliseconds: 50
            );

            await queue.StartAsync();

            Assert.IsTrue(queue.IsRunning);

            queue.Dispose();

            Assert.IsFalse(queue.IsRunning);
        }

        [TestMethod()]
        public async Task WorkTaskQueue_StopAsync_WithExternalChannelAndReadCallback_ShouldNotHang()
        {
            var channel = Channel.CreateUnbounded<WorkTaskQueueDataModel>();
            List<WorkTaskQueueDataModel> remainingDataList = null;

            var queue = new TestExternalChannelWorkTaskQueue(channel, dataList =>
            {
                remainingDataList = dataList;
            });

            await queue.StartAsync();

            var stopTask = queue.StopAsync();
            var completedTask = await Task.WhenAny(stopTask, Task.Delay(TimeSpan.FromSeconds(2)));

            Assert.AreSame(stopTask, completedTask);
            await stopTask;
            Assert.IsNotNull(remainingDataList);
            Assert.AreEqual(0, remainingDataList.Count);
        }

        [TestMethod()]
        public async Task WorkTaskTriggerQueue_StopAsync_ShouldFlushCachedBatch()
        {
            List<WorkTaskQueueDataModel> flushedDataList = null;

            var queue = new TestWorkTaskTriggerQueue
            (
                dataList =>
                {
                    flushedDataList = dataList;
                },
                actionTriggerCount: 10,
                actionTriggerTimeSpan: TimeSpan.FromSeconds(30),
                taskSleepMilliseconds: 50
            );

            await queue.StartAsync();
            await queue.AddToQueueAsync(new WorkTaskQueueDataModel { Index = 7 });

            var deadline = DateTime.UtcNow.AddSeconds(2);
            while (queue.CachedCount == 0 && DateTime.UtcNow < deadline)
            {
                await Task.Delay(20);
            }

            Assert.AreEqual(1, queue.CachedCount);

            await queue.StopAsync();

            Assert.IsNotNull(flushedDataList);
            Assert.AreEqual(1, flushedDataList.Count);
            Assert.AreEqual(7, flushedDataList[0].Index);
        }



    }



}
