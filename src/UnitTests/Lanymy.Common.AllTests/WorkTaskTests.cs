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

        sealed class FaultingSimpleWorkTask : BaseSimpleWorkTask
        {
            private readonly TaskCompletionSource<bool> _delayEnteredSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            public FaultingSimpleWorkTask()
                : base(_ => { }, sleepIntervalMilliseconds: 10)
            {
            }

            public bool HasTaskForTest => _CurrentTask != null;
            public bool HasCancellationTokenSourceForTest => _CurrentCancellationTokenSource != null;
            public Task WaitForDelayFaultAsync() => _delayEnteredSignal.Task;

            protected override Task DelayAsync(CancellationToken token)
            {
                _delayEnteredSignal.TrySetResult(true);
                throw new InvalidOperationException("simple worker failed");
            }

            protected override async Task OnDisposeAsync()
            {
                await Task.CompletedTask;
            }
        }

        sealed class FaultingSimpleWorkTaskQueue : BaseSimpleWorkTaskQueue<WorkTaskQueueDataModel>
        {
            private readonly TaskCompletionSource<bool> _delayEnteredSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            public FaultingSimpleWorkTaskQueue()
                : base(_ => { }, sleepIntervalMilliseconds: 10)
            {
            }

            public bool HasTaskForTest => _CurrentTask != null;
            public bool HasCancellationTokenSourceForTest => _CurrentCancellationTokenSource != null;
            public int CachedCountForTest => _CurrentCacheConcurrentQueue.Count;
            public Task WaitForDelayFaultAsync() => _delayEnteredSignal.Task;

            protected override Task DelayAsync(CancellationToken token)
            {
                _delayEnteredSignal.TrySetResult(true);
                throw new InvalidOperationException("simple queue worker failed");
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

        sealed class FaultingStopWorkTaskQueue : BaseWorkTaskQueue<WorkTaskQueueDataModel>
        {
            private readonly TaskCompletionSource<bool> _workerStartedSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            public FaultingStopWorkTaskQueue()
                : base(null, _ => { }, null, taskSleepMilliseconds: 10)
            {
            }

            public bool HasChannelForTest => _CurrentChannel != null;
            public bool HasCancellationTokenSourceForTest => _CurrentCancellationTokenSource != null;
            public int WorkTaskCountForTest => _CurrentWorkTaskList.Count;
            public Task WaitForWorkerFaultAsync() => _workerStartedSignal.Task;

            protected override Task OnTaskAsync(CancellationToken token)
            {
                _workerStartedSignal.TrySetResult(true);
                throw new InvalidOperationException("queue worker failed");
            }
        }

        sealed class ThrowingStopReadWorkTaskQueue : BaseWorkTaskQueue<WorkTaskQueueDataModel>
        {
            public ThrowingStopReadWorkTaskQueue()
                : base(null, _ => { }, _ => throw new InvalidOperationException("queue stop read failed"), taskSleepMilliseconds: 10)
            {
            }

            public bool HasChannelForTest => _CurrentChannel != null;
            public bool HasCancellationTokenSourceForTest => _CurrentCancellationTokenSource != null;
            public int WorkTaskCountForTest => _CurrentWorkTaskList.Count;
        }

        sealed class FaultingStopTimerWorkTask : BaseTimerWorkTask
        {
            private readonly TaskCompletionSource<bool> _timerStartedSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            public FaultingStopTimerWorkTask()
                : base(() => null, taskSleepMilliseconds: 10)
            {
            }

            public bool HasTaskForTest => _CurrentTask != null;
            public bool HasCancellationTokenSourceForTest => _CurrentCancellationTokenSource != null;
            public Task WaitForTimerFaultAsync() => _timerStartedSignal.Task;

            protected override Task OnTaskAsync(CancellationToken token)
            {
                _timerStartedSignal.TrySetResult(true);
                throw new InvalidOperationException("timer worker failed");
            }

            protected override Task OnDisposeAsync()
            {
                return Task.CompletedTask;
            }
        }

        sealed class TestChannelWorkTask : BaseChannelWorkTask<WorkTaskQueueDataModel>
        {
            public TestChannelWorkTask(Channel<WorkTaskQueueDataModel> channel)
                : base(channel, _ => { }, null, 1, 10, 1, BoundedChannelFullMode.Wait)
            {
            }

            protected override Task OnStartAsync()
            {
                return Task.CompletedTask;
            }

            protected override Task OnStopAsync()
            {
                _CurrentChannel?.Writer.TryComplete();
                _CurrentChannel = null;
                return Task.CompletedTask;
            }

            protected override Task OnDisposeAsync()
            {
                return Task.CompletedTask;
            }
        }

        sealed class TrackingContextChildWorkTask : BaseWorkTask
        {
            public Exception StartException { get; set; }
            public Exception StopException { get; set; }
            public Exception DisposeException { get; set; }

            public int StartCallCount { get; private set; }
            public int StopCallCount { get; private set; }
            public int DisposeCallCount { get; private set; }

            protected override Task OnStartAsync()
            {
                StartCallCount++;

                if (StartException != null)
                {
                    throw StartException;
                }

                return Task.CompletedTask;
            }

            protected override Task OnStopAsync()
            {
                StopCallCount++;

                if (StopException != null)
                {
                    throw StopException;
                }

                return Task.CompletedTask;
            }

            protected override Task OnDisposeAsync()
            {
                DisposeCallCount++;

                if (DisposeException != null)
                {
                    throw DisposeException;
                }

                return Task.CompletedTask;
            }
        }

        sealed class TrackingWorkTaskTriggerQueueContext : WorkTaskTriggerQueueContext<WorkTaskQueueDataModel>
        {
            private readonly Func<ushort, TrackingContextChildWorkTask> _workTaskFactory;

            public TrackingWorkTaskTriggerQueueContext(Func<ushort, TrackingContextChildWorkTask> workTaskFactory, ushort workTaskCount = 2)
                : base(_ => { }, workTaskCount, actionTriggerCount: 1, actionTriggerTimeSpan: TimeSpan.FromSeconds(3))
            {
                _workTaskFactory = workTaskFactory;
            }

            public List<TrackingContextChildWorkTask> CreatedWorkTasks { get; } = new List<TrackingContextChildWorkTask>();
            public bool HasChannelForTest => _CurrentChannel != null;

            protected override BaseWorkTask CreateWorkTaskQueue(Channel<WorkTaskQueueDataModel> channel, ushort workTaskIndex)
            {
                var workTask = _workTaskFactory(workTaskIndex);
                CreatedWorkTasks.Add(workTask);
                return workTask;
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

        class TestCountingWorkTask : BaseWorkTask
        {
            public int StartCallCount { get; private set; }
            public int StopCallCount { get; private set; }
            public int DisposeCallCount { get; private set; }

            protected override Task OnStartAsync()
            {
                StartCallCount++;
                return Task.CompletedTask;
            }

            protected override Task OnStopAsync()
            {
                StopCallCount++;
                return Task.CompletedTask;
            }

            protected override Task OnDisposeAsync()
            {
                DisposeCallCount++;
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
                    Assert.AreEqual(1, dataList[0].Index);
                    Assert.AreEqual(9, dataList.Count);
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
        public async Task BaseSimpleWorkTask_StopAsync_WhenSleeping_ShouldCancelDelayPromptly()
        {
            using var firstRunSignal = new ManualResetEventSlim(false);

            using var workTask = new TestSimpleWorkTask(_ => firstRunSignal.Set(), sleepIntervalMilliseconds: 5000);

            await workTask.StartAsync();

            Assert.IsTrue(firstRunSignal.Wait(TimeSpan.FromSeconds(2)));

            var stopTask = workTask.StopAsync();
            var completedTask = await Task.WhenAny(stopTask, Task.Delay(TimeSpan.FromMilliseconds(500)));

            Assert.AreSame(stopTask, completedTask);
            await stopTask;
            Assert.IsFalse(workTask.IsRunning);
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
        public async Task BaseSimpleWorkTask_StopAsync_WhenWorkerTaskFaults_ShouldStillReleaseInternalState()
        {
            using var workTask = new FaultingSimpleWorkTask();

            await workTask.StartAsync();
            await workTask.WaitForDelayFaultAsync();

            var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => workTask.StopAsync());

            Assert.AreEqual("simple worker failed", exception.Message);
            Assert.IsFalse(workTask.IsRunning);
            Assert.IsFalse(workTask.HasTaskForTest);
            Assert.IsFalse(workTask.HasCancellationTokenSourceForTest);
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
        public async Task BaseSimpleWorkTaskQueue_StopAsync_WhenSleeping_ShouldCancelDelayPromptly()
        {
            using var processedSignal = new ManualResetEventSlim(false);

            using var queue = new TestResilientSimpleWorkTaskQueue(
                data => processedSignal.Set(),
                (_, _) => { },
                sleepIntervalMilliseconds: 5000);

            await queue.StartAsync();
            queue.AddToQueue(new WorkTaskQueueDataModel { Index = 1 });

            Assert.IsTrue(processedSignal.Wait(TimeSpan.FromSeconds(2)));

            var stopTask = queue.StopAsync();
            var completedTask = await Task.WhenAny(stopTask, Task.Delay(TimeSpan.FromMilliseconds(500)));

            Assert.AreSame(stopTask, completedTask);
            await stopTask;
            Assert.IsFalse(queue.IsRunning);
        }

        [TestMethod()]
        public async Task BaseSimpleWorkTaskQueue_StopAsync_WhenWorkerTaskFaults_ShouldStillReleaseInternalState()
        {
            using var queue = new FaultingSimpleWorkTaskQueue();

            await queue.StartAsync();
            queue.AddToQueue(new WorkTaskQueueDataModel { Index = 1 });
            await queue.WaitForDelayFaultAsync();

            var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => queue.StopAsync());

            Assert.AreEqual("simple queue worker failed", exception.Message);
            Assert.IsFalse(queue.IsRunning);
            Assert.IsFalse(queue.HasTaskForTest);
            Assert.IsFalse(queue.HasCancellationTokenSourceForTest);
            Assert.AreEqual(0, queue.CachedCountForTest);
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

        [TestMethod]
        public async Task BaseWorkTask_StartAsync_AfterDispose_ShouldRemainStoppedAndNotInvokeStart()
        {
            var workTask = new TestCountingWorkTask();

            workTask.Dispose();
            await workTask.StartAsync();

            Assert.IsTrue(workTask.IsDisposed);
            Assert.IsFalse(workTask.IsRunning);
            Assert.AreEqual(0, workTask.StartCallCount);
            Assert.AreEqual(0, workTask.StopCallCount);
            Assert.AreEqual(1, workTask.DisposeCallCount);
        }

        [TestMethod]
        public async Task BaseWorkTask_Dispose_WhenCalledMultipleTimes_ShouldOnlyDisposeOnce()
        {
            var workTask = new TestCountingWorkTask();
            await workTask.StartAsync();

            workTask.Dispose();
            workTask.Dispose();

            Assert.IsTrue(workTask.IsDisposed);
            Assert.IsFalse(workTask.IsRunning);
            Assert.AreEqual(1, workTask.StartCallCount);
            Assert.AreEqual(1, workTask.StopCallCount);
            Assert.AreEqual(1, workTask.DisposeCallCount);
        }

        [TestMethod]
        public async Task BaseChannelWorkTask_AddToQueueAsync_WhenChannelCompletesDuringStop_ShouldReturnWithoutEscalating()
        {
            var channel = Channel.CreateBounded<WorkTaskQueueDataModel>(new BoundedChannelOptions(1)
            {
                FullMode = BoundedChannelFullMode.Wait,
            });

            await channel.Writer.WriteAsync(new WorkTaskQueueDataModel { Index = 1 });

            using var workTask = new TestChannelWorkTask(channel);
            await workTask.StartAsync();

            var enqueueTask = workTask.AddToQueueAsync(new WorkTaskQueueDataModel { Index = 2 });
            await workTask.StopAsync();

            var completedTask = await Task.WhenAny(enqueueTask, Task.Delay(TimeSpan.FromSeconds(2)));

            Assert.AreSame(enqueueTask, completedTask);
            await enqueueTask;
            Assert.IsFalse(workTask.IsRunning);
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

        [TestMethod]
        public async Task WorkTaskTriggerQueueContext_StartAsync_WhenChildStartThrows_ShouldRollbackStateAndCleanupChildren()
        {
            var startedChild = new TrackingContextChildWorkTask();
            var failedChild = new TrackingContextChildWorkTask
            {
                StartException = new InvalidOperationException("child start failed"),
            };

            using var context = new TrackingWorkTaskTriggerQueueContext(index => index == 0 ? startedChild : failedChild);

            var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => context.StartAsync());

            Assert.AreEqual("child start failed", exception.Message);
            Assert.IsFalse(context.IsRunning);
            Assert.AreEqual(DynamicAsyncQueueStateTypeEnum.Stop, context.StateType);
            Assert.IsFalse(context.HasChannelForTest);
            Assert.AreEqual(2, context.CreatedWorkTasks.Count);
            Assert.AreEqual(1, startedChild.StartCallCount);
            Assert.AreEqual(1, startedChild.StopCallCount);
            Assert.AreEqual(1, startedChild.DisposeCallCount);
            Assert.AreEqual(1, failedChild.StartCallCount);
            Assert.AreEqual(0, failedChild.StopCallCount);
            Assert.AreEqual(1, failedChild.DisposeCallCount);
        }

        [TestMethod]
        public async Task WorkTaskTriggerQueueContext_StopAsync_WhenChildCleanupThrows_ShouldStillResetStateAndCleanupAllChildren()
        {
            var failingChild = new TrackingContextChildWorkTask
            {
                StopException = new InvalidOperationException("child stop failed"),
                DisposeException = new InvalidOperationException("child dispose failed"),
            };
            var healthyChild = new TrackingContextChildWorkTask();

            using var context = new TrackingWorkTaskTriggerQueueContext(index => index == 0 ? failingChild : healthyChild);
            await context.StartAsync();

            var exception = await Assert.ThrowsExactlyAsync<AggregateException>(() => context.StopAsync());

            Assert.AreEqual(2, exception.InnerExceptions.Count);
            Assert.AreEqual("WorkTaskTriggerQueueContext stop child work task failed.", exception.InnerExceptions[0].Message);
            Assert.AreEqual("child stop failed", exception.InnerExceptions[0].InnerException?.Message);
            Assert.AreEqual("WorkTaskTriggerQueueContext dispose child work task failed.", exception.InnerExceptions[1].Message);
            Assert.AreEqual("child dispose failed", exception.InnerExceptions[1].InnerException?.Message);
            Assert.IsFalse(context.IsRunning);
            Assert.AreEqual(DynamicAsyncQueueStateTypeEnum.Stop, context.StateType);
            Assert.IsFalse(context.HasChannelForTest);
            Assert.AreEqual(1, failingChild.StartCallCount);
            Assert.AreEqual(1, failingChild.StopCallCount);
            Assert.AreEqual(1, failingChild.DisposeCallCount);
            Assert.AreEqual(1, healthyChild.StartCallCount);
            Assert.AreEqual(1, healthyChild.StopCallCount);
            Assert.AreEqual(1, healthyChild.DisposeCallCount);
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

        [TestMethod]
        public async Task BaseWorkTaskQueue_StopAsync_WhenWorkerTaskFaults_ShouldStillReleaseInternalState()
        {
            var queue = new FaultingStopWorkTaskQueue();

            await queue.StartAsync();
            await queue.WaitForWorkerFaultAsync();

            var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => queue.StopAsync());

            Assert.AreEqual("queue worker failed", exception.Message);
            Assert.IsFalse(queue.IsRunning);
            Assert.AreEqual(0, queue.WorkTaskCountForTest);
            Assert.IsFalse(queue.HasCancellationTokenSourceForTest);
            Assert.IsFalse(queue.HasChannelForTest);
        }

        [TestMethod]
        public async Task BaseWorkTaskQueue_StopAsync_WhenReadRemainingCallbackThrows_ShouldStillReleaseInternalState()
        {
            var queue = new ThrowingStopReadWorkTaskQueue();

            await queue.StartAsync();

            var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => queue.StopAsync());

            Assert.AreEqual("queue stop read failed", exception.Message);
            Assert.IsFalse(queue.IsRunning);
            Assert.AreEqual(0, queue.WorkTaskCountForTest);
            Assert.IsFalse(queue.HasCancellationTokenSourceForTest);
            Assert.IsFalse(queue.HasChannelForTest);
        }

        [TestMethod]
        public async Task BaseTimerWorkTask_StopAsync_WhenWorkerTaskFaults_ShouldStillReleaseInternalState()
        {
            var workTask = new FaultingStopTimerWorkTask();

            await workTask.StartAsync();
            await workTask.WaitForTimerFaultAsync();

            var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => workTask.StopAsync());

            Assert.AreEqual("timer worker failed", exception.Message);
            Assert.IsFalse(workTask.IsRunning);
            Assert.IsFalse(workTask.HasTaskForTest);
            Assert.IsFalse(workTask.HasCancellationTokenSourceForTest);
        }



    }



}
