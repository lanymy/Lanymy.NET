using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        class TestResilientSimpleWorkTask : BaseSimpleWorkTask
        {
            private readonly Action<CancellationToken> _action;
            private readonly Action<Exception> _errorAction;

            public TestResilientSimpleWorkTask(Action<CancellationToken> action, Action<Exception> errorAction, int sleepIntervalMilliseconds = 0)
                : base(_ => { }, sleepIntervalMilliseconds)
            {
                _action = action;
                _errorAction = errorAction;
            }

            protected override void OnWorkAction(CancellationToken token)
            {
                _action(token);
            }

            protected override void OnWorkError(CancellationToken token, Exception ex)
            {
                _errorAction(ex);
            }

            protected override async Task OnDisposeAsync()
            {
                await Task.CompletedTask;
            }
        }

        class TestResilientSimpleWorkTaskQueue : BaseSimpleWorkTaskQueue<WorkTaskQueueDataModel>
        {
            private readonly Action<WorkTaskQueueDataModel> _action;
            private readonly Action<WorkTaskQueueDataModel, Exception> _errorAction;

            public TestResilientSimpleWorkTaskQueue(Action<WorkTaskQueueDataModel> action, Action<WorkTaskQueueDataModel, Exception> errorAction, int sleepIntervalMilliseconds = 10)
                : base(_ => { }, sleepIntervalMilliseconds)
            {
                _action = action;
                _errorAction = errorAction;
            }

            protected override void OnWorkAction(WorkTaskQueueDataModel data)
            {
                _action(data);
            }

            protected override void OnWorkError(WorkTaskQueueDataModel data, Exception ex)
            {
                _errorAction(data, ex);
            }

            protected override async Task OnDisposeAsync()
            {
                await Task.CompletedTask;
            }
        }

        class TestResilientTimerWorkTask : BaseTimerWorkTask
        {
            private readonly Func<TimerWorkTaskDataResult> _func;
            private readonly Action<Exception> _errorAction;

            public TestResilientTimerWorkTask(Func<TimerWorkTaskDataResult> func, Action<Exception> errorAction, int taskSleepMilliseconds = 10)
                : base(() => null, taskSleepMilliseconds)
            {
                _func = func;
                _errorAction = errorAction;
            }

            protected override TimerWorkTaskDataResult OnWorkFunc()
            {
                return _func();
            }

            protected override void OnWorkError(Exception ex)
            {
                _errorAction(ex);
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

        class TestResilientWorkTaskQueue : BaseWorkTaskQueue<WorkTaskQueueDataModel>
        {
            private readonly Action<WorkTaskQueueDataModel> _action;
            private readonly Action<WorkTaskQueueDataModel, Exception> _errorAction;

            public TestResilientWorkTaskQueue(Action<WorkTaskQueueDataModel> action, Action<WorkTaskQueueDataModel, Exception> errorAction)
                : base(null, _ => { }, null, taskSleepMilliseconds: 50)
            {
                _action = action;
                _errorAction = errorAction;
            }

            protected override void OnWorkAction(WorkTaskQueueDataModel dataModel)
            {
                _action(dataModel);
            }

            protected override void OnWorkError(WorkTaskQueueDataModel dataModel, Exception ex)
            {
                _errorAction(dataModel, ex);
            }
        }

        class TestStartFailWorkTask : BaseWorkTask
        {
            private readonly Exception _startException;
            private readonly Action _disposeAction;

            public TestStartFailWorkTask(Exception startException, Action disposeAction = null)
            {
                _startException = startException;
                _disposeAction = disposeAction;
            }

            protected override Task OnStartAsync()
            {
                throw _startException;
            }

            protected override Task OnStopAsync()
            {
                return Task.CompletedTask;
            }

            protected override Task OnDisposeAsync()
            {
                _disposeAction?.Invoke();
                return Task.CompletedTask;
            }
        }

        class TestDisposeOrderWorkTask : BaseWorkTask
        {
            private readonly Action _stopAction;
            private readonly Action _disposeAction;

            public TestDisposeOrderWorkTask(Action stopAction, Action disposeAction)
            {
                _stopAction = stopAction;
                _disposeAction = disposeAction;
            }

            protected override Task OnStartAsync()
            {
                return Task.CompletedTask;
            }

            protected override Task OnStopAsync()
            {
                _stopAction();
                throw new InvalidOperationException("stop failed");
            }

            protected override Task OnDisposeAsync()
            {
                _disposeAction();
                return Task.CompletedTask;
            }
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
        public async Task BaseSimpleWorkTask_ShouldContinueAfterSingleIterationThrows()
        {
            var errorTypes = new List<Type>();
            var successSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var executionCount = 0;

            using var workTask = new TestResilientSimpleWorkTask
            (
                _ =>
                {
                    var currentCount = Interlocked.Increment(ref executionCount);

                    if (currentCount == 1)
                    {
                        throw new InvalidOperationException("boom");
                    }

                    successSignal.TrySetResult(true);
                },
                ex =>
                {
                    lock (errorTypes)
                    {
                        errorTypes.Add(ex.GetType());
                    }
                },
                sleepIntervalMilliseconds: 10
            );

            await workTask.StartAsync();

            var completedTask = await Task.WhenAny(successSignal.Task, Task.Delay(TimeSpan.FromSeconds(2)));

            Assert.AreSame(successSignal.Task, completedTask);
            Assert.IsTrue(workTask.IsRunning);

            lock (errorTypes)
            {
                CollectionAssert.AreEqual(new List<Type> { typeof(InvalidOperationException) }, errorTypes);
            }

            Assert.IsTrue(Volatile.Read(ref executionCount) >= 2);

            await workTask.StopAsync();
        }

        [TestMethod()]
        public async Task BaseSimpleWorkTaskQueue_ShouldContinueAfterSingleItemThrows()
        {
            var processedIndexes = new List<int>();
            var errorIndexes = new List<int>();
            var processedSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            using var queue = new TestResilientSimpleWorkTaskQueue
            (
                data =>
                {
                    if (data.Index == 1)
                    {
                        throw new InvalidOperationException("boom");
                    }

                    lock (processedIndexes)
                    {
                        processedIndexes.Add(data.Index);
                    }

                    if (data.Index == 2)
                    {
                        processedSignal.TrySetResult(true);
                    }
                },
                (data, ex) =>
                {
                    if (ex is InvalidOperationException)
                    {
                        lock (errorIndexes)
                        {
                            errorIndexes.Add(data.Index);
                        }
                    }
                },
                sleepIntervalMilliseconds: 10
            );

            await queue.StartAsync();
            queue.AddToQueue(new WorkTaskQueueDataModel { Index = 1 });
            queue.AddToQueue(new WorkTaskQueueDataModel { Index = 2 });

            var completedTask = await Task.WhenAny(processedSignal.Task, Task.Delay(TimeSpan.FromSeconds(2)));

            Assert.AreSame(processedSignal.Task, completedTask);
            Assert.IsTrue(queue.IsRunning);

            lock (errorIndexes)
            {
                CollectionAssert.AreEqual(new List<int> { 1 }, errorIndexes);
            }

            lock (processedIndexes)
            {
                CollectionAssert.AreEqual(new List<int> { 2 }, processedIndexes);
            }

            await queue.StopAsync();
        }

        [TestMethod()]
        public async Task BaseTimerWorkTask_ShouldContinueAfterSingleTickThrows()
        {
            var errorTypes = new List<Type>();
            var successSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var tickCount = 0;

            using var workTask = new TestResilientTimerWorkTask
            (
                () =>
                {
                    var currentTick = Interlocked.Increment(ref tickCount);

                    if (currentTick == 1)
                    {
                        throw new InvalidOperationException("boom");
                    }

                    successSignal.TrySetResult(true);
                    return null;
                },
                ex =>
                {
                    lock (errorTypes)
                    {
                        errorTypes.Add(ex.GetType());
                    }
                },
                taskSleepMilliseconds: 10
            );

            await workTask.StartAsync();

            var completedTask = await Task.WhenAny(successSignal.Task, Task.Delay(TimeSpan.FromSeconds(2)));

            Assert.AreSame(successSignal.Task, completedTask);
            Assert.IsTrue(workTask.IsRunning);

            lock (errorTypes)
            {
                CollectionAssert.AreEqual(new List<Type> { typeof(InvalidOperationException) }, errorTypes);
            }

            Assert.IsTrue(Volatile.Read(ref tickCount) >= 2);

            await workTask.StopAsync();
        }

        [TestMethod()]
        public async Task BaseWorkTask_StartAsync_WhenStartFails_ShouldRollbackIsRunning()
        {
            var workTask = new TestStartFailWorkTask(new InvalidOperationException("start failed"));

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => workTask.StartAsync());

            Assert.IsFalse(workTask.IsRunning);
        }

        [TestMethod()]
        public void BaseWorkTask_Dispose_WhenStopFails_ShouldStillInvokeOnDisposeAsync()
        {
            var order = new List<string>();
            var workTask = new TestDisposeOrderWorkTask(
                () => order.Add("stop"),
                () => order.Add("dispose"));

            workTask.StartAsync().Wait();

            var ex = Assert.ThrowsExactly<InvalidOperationException>(() => workTask.Dispose());

            Assert.AreEqual("stop failed", ex.Message);
            CollectionAssert.AreEqual(new List<string> { "stop", "dispose" }, order);
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

        [TestMethod()]
        public async Task WorkTaskTriggerQueue_WorkTriggerActionThrow_ShouldRecoverAndRetryBatch()
        {
            var triggerCallCount = 0;
            List<int> processedIndexes = null;
            var processedSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            var queue = new TestWorkTaskTriggerQueue
            (
                dataList =>
                {
                    var currentCallCount = Interlocked.Increment(ref triggerCallCount);
                    if (currentCallCount == 1)
                    {
                        throw new InvalidOperationException("trigger failed");
                    }

                    processedIndexes = dataList.Select(o => o.Index).ToList();
                    processedSignal.TrySetResult(true);
                },
                actionTriggerCount: 1,
                actionTriggerTimeSpan: TimeSpan.FromSeconds(30),
                taskSleepMilliseconds: 50
            );

            await queue.StartAsync();
            await queue.AddToQueueAsync(new WorkTaskQueueDataModel { Index = 1 });

            var firstAttemptDeadline = DateTime.UtcNow.AddSeconds(2);
            while (triggerCallCount == 0 && DateTime.UtcNow < firstAttemptDeadline)
            {
                await Task.Delay(20);
            }

            Assert.AreEqual(1, triggerCallCount);
            Assert.IsTrue(queue.IsRunning);

            await queue.AddToQueueAsync(new WorkTaskQueueDataModel { Index = 2 });

            var completedTask = await Task.WhenAny(processedSignal.Task, Task.Delay(TimeSpan.FromSeconds(2)));

            Assert.AreSame(processedSignal.Task, completedTask);
            Assert.AreEqual(2, triggerCallCount);
            CollectionAssert.AreEqual(new List<int> { 1, 2 }, processedIndexes);
            Assert.IsTrue(queue.IsRunning);

            await queue.StopAsync();
        }

        [TestMethod()]
        public async Task WorkTaskTriggerQueue_WorkTriggerActionCanRequeueWithoutDeadlock()
        {
            var processedBatches = new List<List<int>>();
            var secondBatchSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var hasRequeued = 0;
            TestWorkTaskTriggerQueue queue = null;

            queue = new TestWorkTaskTriggerQueue
            (
                dataList =>
                {
                    var batchIndexes = dataList.Select(o => o.Index).ToList();
                    lock (processedBatches)
                    {
                        processedBatches.Add(batchIndexes);
                    }

                    if (batchIndexes.Count == 1 && batchIndexes[0] == 1 && Interlocked.Exchange(ref hasRequeued, 1) == 0)
                    {
                        queue.AddToQueueAsync(new WorkTaskQueueDataModel { Index = 2 }).GetAwaiter().GetResult();
                        return;
                    }

                    if (batchIndexes.Count == 1 && batchIndexes[0] == 2)
                    {
                        secondBatchSignal.TrySetResult(true);
                    }
                },
                actionTriggerCount: 1,
                actionTriggerTimeSpan: TimeSpan.FromSeconds(30),
                taskSleepMilliseconds: 50
            );

            await queue.StartAsync();
            await queue.AddToQueueAsync(new WorkTaskQueueDataModel { Index = 1 });

            var completedTask = await Task.WhenAny(secondBatchSignal.Task, Task.Delay(TimeSpan.FromSeconds(2)));

            Assert.AreSame(secondBatchSignal.Task, completedTask);

            lock (processedBatches)
            {
                Assert.AreEqual(2, processedBatches.Count);
                CollectionAssert.AreEqual(new List<int> { 1 }, processedBatches[0]);
                CollectionAssert.AreEqual(new List<int> { 2 }, processedBatches[1]);
            }

            await queue.StopAsync();
        }

        [TestMethod()]
        public async Task WorkTaskTriggerQueueContext_StopAsync_WithPendingChannelData_ShouldNotHang()
        {
            var context = new WorkTaskTriggerQueueContext<WorkTaskQueueDataModel>
            (
                _ =>
                {
                    Thread.Sleep(200);
                },
                workTaskCount: 1,
                actionTriggerCount: 1,
                actionTriggerTimeSpan: TimeSpan.FromSeconds(3)
            );

            await context.StartAsync();

            for (var i = 0; i < 20; i++)
            {
                await context.AddToQueueAsync(new WorkTaskQueueDataModel { Index = i });
            }

            var stopTask = context.StopAsync();
            var completedTask = await Task.WhenAny(stopTask, Task.Delay(TimeSpan.FromSeconds(2)));

            Assert.AreSame(stopTask, completedTask);
            await stopTask;
            Assert.IsFalse(context.IsRunning);
        }

        [TestMethod()]
        public async Task WorkTaskQueue_WorkerShouldContinueAfterSingleItemThrows()
        {
            var processedIndexes = new List<int>();
            var errorIndexes = new List<int>();
            var processedSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            var queue = new TestResilientWorkTaskQueue
            (
                dataModel =>
                {
                    if (dataModel.Index == 1)
                    {
                        throw new InvalidOperationException("boom");
                    }

                    lock (processedIndexes)
                    {
                        processedIndexes.Add(dataModel.Index);
                    }

                    if (dataModel.Index == 2)
                    {
                        processedSignal.TrySetResult(true);
                    }
                },
                (dataModel, ex) =>
                {
                    if (ex is InvalidOperationException)
                    {
                        lock (errorIndexes)
                        {
                            errorIndexes.Add(dataModel.Index);
                        }
                    }
                }
            );

            await queue.StartAsync();
            await queue.AddToQueueAsync(new WorkTaskQueueDataModel { Index = 1 });
            await queue.AddToQueueAsync(new WorkTaskQueueDataModel { Index = 2 });

            var completedTask = await Task.WhenAny(processedSignal.Task, Task.Delay(TimeSpan.FromSeconds(2)));

            Assert.AreSame(processedSignal.Task, completedTask);
            Assert.IsTrue(queue.IsRunning);

            lock (errorIndexes)
            {
                CollectionAssert.AreEqual(new List<int> { 1 }, errorIndexes);
            }

            lock (processedIndexes)
            {
                CollectionAssert.AreEqual(new List<int> { 2 }, processedIndexes);
            }

            await queue.StopAsync();
        }



    }



}
