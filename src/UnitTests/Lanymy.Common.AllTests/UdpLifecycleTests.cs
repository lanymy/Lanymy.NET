using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class UdpLifecycleTests
    {
        private sealed class TestSessionToken : ISessionToken
        {
            public Guid SessionID { get; } = Guid.NewGuid();
            public string IP { get; } = "127.0.0.1";
            public int Port { get; } = 0;
            public byte SendNum { get; } = 0;
            public byte[] CacheHeartBytes { get; } = null;
            public uint LastReceiveDateTimeTotalMillisecondsFromInstantiation { get; set; }
            public DateTime ConnectionDateTime { get; } = DateTime.UtcNow;
            public DateTime LastReceiveDateTime { get; set; } = DateTime.UtcNow;
            public DateTime LastSendDateTime { get; set; } = DateTime.UtcNow;
        }

        private sealed class TestUdpPackage : IUdpPackage
        {
            public IPEndPoint RemoteIpEndPoint { get; set; }
        }

        private sealed class TestFixedHeaderPackageFilter : BaseFixedHeaderPackageFilter<TestUdpPackage, TestUdpPackage, TestSessionToken>
        {
            public bool IsPackageValid { get; set; } = true;
            public bool ThrowOnEncode { get; set; }

            public TestFixedHeaderPackageFilter()
                : base(4)
            {
            }

            public override int GetBodyLengthFromHeader(int cursorIndex, byte[] bufferBytes)
            {
                return 0;
            }

            public override bool CheckPackage(byte[] packageBytes)
            {
                return IsPackageValid;
            }

            public override byte[] EncodePackage(TestUdpPackage sendPackage)
            {
                if (ThrowOnEncode)
                {
                    throw new InvalidOperationException("udp encode package failed");
                }

                return Array.Empty<byte>();
            }

            public override TestUdpPackage DecodePackage(byte[] packageBytes)
            {
                return new TestUdpPackage();
            }

            public override byte[] GetHeartBytes(ISessionToken sessionToken)
            {
                return Array.Empty<byte>();
            }
        }

        private class TestUdpClient : BaseUdpClient<TestSessionToken, TestFixedHeaderPackageFilter, TestUdpPackage, TestUdpPackage>
        {
            private int _receivedPackageCount;

            public Exception ReceivePackageException { get; set; }
            public Exception StartEventException { get; set; }
            public Exception CloseEventException { get; set; }
            public Exception CloseReceiveQueueException { get; set; }
            public Exception CloseSendQueueException { get; set; }
            public Exception DisposeUdpClientException { get; set; }
            public Exception DisposeReceiveQueueException { get; set; }
            public Exception DisposeSendQueueException { get; set; }
            public Exception LastError { get; private set; }
            public int ErrorCount { get; private set; }
            public int ReceivedPackageCount => _receivedPackageCount;
            public bool ThrowOnErrorEvent { get; set; }
            public TestFixedHeaderPackageFilter TestFilter { get; }

            public TestUdpClient(int port)
                : this(new TestFixedHeaderPackageFilter(), port)
            {
            }

            private TestUdpClient(TestFixedHeaderPackageFilter fixedHeaderPackageFilter, int port)
                : base(fixedHeaderPackageFilter, port, sendDataIntervalMilliseconds: 1)
            {
                TestFilter = fixedHeaderPackageFilter;
            }

            protected override void OnReceivePackage(TestUdpPackage package)
            {
                Interlocked.Increment(ref _receivedPackageCount);

                if (ReceivePackageException != null)
                {
                    throw ReceivePackageException;
                }
            }

            protected override void OnStartEvent()
            {
                if (StartEventException != null)
                {
                    throw StartEventException;
                }
            }

            protected override void OnCloseEvent()
            {
                if (CloseEventException != null)
                {
                    throw CloseEventException;
                }
            }

            protected override void OnErrorEvent(IPEndPoint remoteIPEndPoint, Exception ex)
            {
                ErrorCount++;
                LastError = ex;

                if (ThrowOnErrorEvent)
                {
                    throw new InvalidOperationException("error event failed");
                }
            }

            public void TriggerReceiveData(IPEndPoint remoteIPEndPoint, byte[] packageBytes)
            {
                OnReceiveDataEvent(remoteIPEndPoint, packageBytes);
            }

            public Task EnqueueReceiveAsync(IPEndPoint remoteIPEndPoint, byte[] packageBytes)
            {
                return _ReceiveWorkTaskQueue.AddToQueueAsync(new UdpSourceDataModel
                {
                    RemoteIPEndPoint = remoteIPEndPoint,
                    SourceDataBytes = packageBytes,
                });
            }

            public bool TryGetReceiveContextForTest()
            {
                return TryGetReceiveContext(out _, out _);
            }

            public bool CanContinueReceiveForCurrentSnapshot()
            {
                if (!TryGetReceiveContext(out var currentUdpClient, out var receiveWorkTaskQueue))
                {
                    return false;
                }

                return CanContinueReceive(currentUdpClient, receiveWorkTaskQueue);
            }

            public object GetReceiveWorkTaskQueueForTest()
            {
                return _ReceiveWorkTaskQueue;
            }

            public object GetSendWorkTaskQueueForTest()
            {
                return _SendWorkTaskQueue;
            }

            protected override async Task StopReceiveWorkTaskQueueAsync(WorkTaskQueue<UdpSourceDataModel> receiveWorkTaskQueue)
            {
                if (CloseReceiveQueueException != null)
                {
                    throw CloseReceiveQueueException;
                }

                await base.StopReceiveWorkTaskQueueAsync(receiveWorkTaskQueue);
            }

            protected override async Task StopSendWorkTaskQueueAsync(WorkTaskQueue<SendUdpDataModel> sendWorkTaskQueue)
            {
                if (CloseSendQueueException != null)
                {
                    throw CloseSendQueueException;
                }

                await base.StopSendWorkTaskQueueAsync(sendWorkTaskQueue);
            }

            protected override void DisposeCurrentUdpClient(UdpClient currentUdpClient)
            {
                if (DisposeUdpClientException != null)
                {
                    throw DisposeUdpClientException;
                }

                base.DisposeCurrentUdpClient(currentUdpClient);
            }

            protected override void DisposeReceiveWorkTaskQueue(WorkTaskQueue<UdpSourceDataModel> receiveWorkTaskQueue)
            {
                if (DisposeReceiveQueueException != null)
                {
                    throw DisposeReceiveQueueException;
                }

                base.DisposeReceiveWorkTaskQueue(receiveWorkTaskQueue);
            }

            protected override void DisposeSendWorkTaskQueue(WorkTaskQueue<SendUdpDataModel> sendWorkTaskQueue)
            {
                if (DisposeSendQueueException != null)
                {
                    throw DisposeSendQueueException;
                }

                base.DisposeSendWorkTaskQueue(sendWorkTaskQueue);
            }
        }

        private sealed class RollbackFailureUdpClient : TestUdpClient
        {
            public Exception CanContinueStartException { get; set; }
            public Exception ResetStartReceiveQueueException { get; set; }

            public RollbackFailureUdpClient(int port)
                : base(port)
            {
            }

            protected override bool CanContinueStart(WorkTaskQueue<UdpSourceDataModel> receiveWorkTaskQueue, WorkTaskQueue<SendUdpDataModel> sendWorkTaskQueue, UdpClient currentUdpClient)
            {
                if (currentUdpClient != null && CanContinueStartException != null)
                {
                    throw CanContinueStartException;
                }

                return base.CanContinueStart(receiveWorkTaskQueue, sendWorkTaskQueue, currentUdpClient);
            }

            protected override WorkTaskQueue<UdpSourceDataModel> CreateReceiveWorkTaskQueue()
            {
                return new WorkTaskQueue<UdpSourceDataModel>(
                    data => TriggerReceiveData(data.RemoteIPEndPoint, data.SourceDataBytes),
                    _ =>
                    {
                        if (ResetStartReceiveQueueException != null)
                        {
                            throw ResetStartReceiveQueueException;
                        }
                    },
                    taskSleepMilliseconds: 1);
            }
        }

        private sealed class ThrowingSyncSendUdpClient : TestUdpClient
        {
            public ThrowingSyncSendUdpClient(int port)
                : base(port)
            {
            }

            protected override Task<bool> TrySendAsync(SendUdpDataModel sendUdpDataModel)
            {
                throw new InvalidOperationException("udp sync send failed");
            }
        }

        private sealed class ThrowingDirectSyncSendUdpClient : TestUdpClient
        {
            public int TrySendSynchronouslyCallCount { get; private set; }

            public ThrowingDirectSyncSendUdpClient(int port)
                : base(port)
            {
            }

            protected override Exception TryWaitSynchronously(Func<Task<bool>> taskFactory, out bool result)
            {
                TrySendSynchronouslyCallCount++;
                result = default;
                throw new InvalidOperationException("udp sync send bridge failed");
            }
        }

        private sealed class ThrowingSyncCloseUdpClient : TestUdpClient
        {
            public int CloseAsyncCallCount { get; private set; }

            public ThrowingSyncCloseUdpClient(int port)
                : base(port)
            {
            }

            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                throw new InvalidOperationException("udp close failed");
            }
        }

        private sealed class ThrowingDirectSyncCloseUdpClient : TestUdpClient
        {
            public int TryCloseSynchronouslyCallCount { get; private set; }

            public ThrowingDirectSyncCloseUdpClient(int port)
                : base(port)
            {
            }

            protected override Exception TryCloseSynchronously()
            {
                TryCloseSynchronouslyCallCount++;
                throw new InvalidOperationException("udp sync bridge failed");
            }
        }

        private sealed class CallbackAsyncResult : IAsyncResult
        {
            public object AsyncState { get; init; }
            public WaitHandle AsyncWaitHandle => null;
            public bool CompletedSynchronously => true;
            public bool IsCompleted => true;
        }

        private sealed class CallbackLifecycleUdpClient : TestUdpClient
        {
            private UdpClient _lastBeginReceiveUdpClient;

            public int BeginReceiveCallCount { get; private set; }
            public int EndReceiveCallCount { get; private set; }
            public byte[] EndReceiveBytes { get; set; } = new byte[] { 1, 2, 3, 4 };
            public IPEndPoint EndReceiveRemoteEndPoint { get; set; } = new IPEndPoint(IPAddress.Loopback, 9527);

            public CallbackLifecycleUdpClient(int port)
                : base(port)
            {
            }

            protected override void BeginReceive(UdpClient currentUdpClient)
            {
                BeginReceiveCallCount++;
                _lastBeginReceiveUdpClient = currentUdpClient;
            }

            protected override byte[] EndReceive(UdpClient currentUdpClient, IAsyncResult asyncResult, ref IPEndPoint remoteIPEndPoint)
            {
                EndReceiveCallCount++;
                remoteIPEndPoint = EndReceiveRemoteEndPoint;
                return EndReceiveBytes;
            }

            public void InvokeLastReceiveCallback()
            {
                base.ReciveCallBack(new CallbackAsyncResult
                {
                    AsyncState = _lastBeginReceiveUdpClient,
                });
            }
        }

        [TestMethod]
        public void BaseUdpClient_Start_WhenBindFails_ShouldRollbackRunningState()
        {
            using var occupiedSocket = new UdpClient(0);
            var port = ((IPEndPoint)occupiedSocket.Client.LocalEndPoint).Port;
            var client = new TestUdpClient(port);

            Assert.ThrowsExactly<SocketException>(() => client.Start());

            Assert.IsFalse(client.IsAccept);

            client.Close();
        }

        [TestMethod]
        public void BaseUdpClient_Start_WhenResetStartReceiveQueueFails_ShouldReportErrorReplaceQueueAndAllowRetry()
        {
            var client = new RollbackFailureUdpClient(0)
            {
                CanContinueStartException = new InvalidOperationException("start continuation failed"),
                ResetStartReceiveQueueException = new InvalidOperationException("receive queue rollback stop failed"),
            };

            try
            {
                var receiveQueueBeforeStart = client.GetReceiveWorkTaskQueueForTest();
                var sendQueueBeforeStart = client.GetSendWorkTaskQueueForTest();

                var exception = Assert.ThrowsExactly<InvalidOperationException>(() => client.Start());

                Assert.AreEqual("start continuation failed", exception.Message);
                Assert.IsFalse(client.IsAccept);
                Assert.AreEqual(1, client.ErrorCount);
                Assert.AreEqual("UdpClient reset start receive queue failed.", client.LastError?.Message);
                Assert.IsInstanceOfType(client.LastError?.InnerException, typeof(InvalidOperationException));
                Assert.AreEqual("receive queue rollback stop failed", client.LastError?.InnerException?.Message);
                Assert.AreNotSame(receiveQueueBeforeStart, client.GetReceiveWorkTaskQueueForTest());
                Assert.AreSame(sendQueueBeforeStart, client.GetSendWorkTaskQueueForTest());

                client.CanContinueStartException = null;
                client.ResetStartReceiveQueueException = null;

                client.Start();

                Assert.IsTrue(client.IsAccept);
            }
            finally
            {
                client.Dispose();
            }
        }

        [TestMethod]
        public void BaseUdpClient_OnReceivePackageThrow_ShouldReportErrorWithoutBubbling()
        {
            var client = new TestUdpClient(0)
            {
                ReceivePackageException = new InvalidOperationException("receive package callback failed"),
            };
            var remoteIpEndPoint = new IPEndPoint(IPAddress.Loopback, 9527);

            client.TriggerReceiveData(remoteIpEndPoint, new byte[] { 1, 2, 3, 4 });

            Assert.AreEqual(1, client.ErrorCount);
            Assert.AreEqual("receive package callback failed", client.LastError?.Message);
        }

        [TestMethod]
        public void BaseUdpClient_OnStartEventThrow_ShouldReportErrorWithoutBubbling()
        {
            var client = new TestUdpClient(0)
            {
                StartEventException = new InvalidOperationException("start callback failed"),
            };

            client.OnStart();

            Assert.AreEqual(1, client.ErrorCount);
            Assert.AreEqual("start callback failed", client.LastError?.Message);
        }

        [TestMethod]
        public void BaseUdpClient_OnErrorEventThrow_ShouldNotBubble()
        {
            var client = new TestUdpClient(0)
            {
                ThrowOnErrorEvent = true,
            };
            var remoteIpEndPoint = new IPEndPoint(IPAddress.Loopback, 9527);
            client.TestFilter.IsPackageValid = false;

            client.TriggerReceiveData(remoteIpEndPoint, new byte[] { 1, 2, 3, 4 });

            Assert.AreEqual(1, client.ErrorCount);
            Assert.AreEqual("data bytes error", client.LastError?.Message);
        }

        [TestMethod]
        public void BaseUdpClient_Send_WhenClientIsNotRunning_ShouldReturnFalse()
        {
            var client = new TestUdpClient(0);

            var result = client.Send(new SendUdpDataModel(new IPEndPoint(IPAddress.Loopback, 9527), new byte[] { 1, 2, 3, 4 }));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void BaseUdpClient_Send_WhenSendModelIsInvalid_ShouldReturnFalse()
        {
            var client = new TestUdpClient(0);

            Assert.IsFalse(client.Send((TestUdpPackage)null));
            Assert.IsFalse(client.Send(new SendUdpDataModel(null, new byte[] { 1, 2, 3, 4 })));
            Assert.IsFalse(client.Send(new SendUdpDataModel(new IPEndPoint(IPAddress.Loopback, 9527), Array.Empty<byte>())));
        }

        [TestMethod]
        public void BaseUdpClient_Send_WhenClientIsRunningAndPayloadValid_ShouldReturnTrue()
        {
            var client = new TestUdpClient(0);

            try
            {
                client.Start();

                var result = client.Send(new SendUdpDataModel(new IPEndPoint(IPAddress.Loopback, 9527), new byte[] { 1, 2, 3, 4 }));

                Assert.IsTrue(result);
            }
            finally
            {
                client.Close();
            }
        }

        [TestMethod]
        public void BaseUdpClient_Send_WhenTrySendAsyncThrowsSynchronously_ShouldReportErrorWithoutEscalating()
        {
            var client = new ThrowingSyncSendUdpClient(0);

            try
            {
                client.Start();

                var result = client.Send(new SendUdpDataModel(new IPEndPoint(IPAddress.Loopback, 9527), new byte[] { 1, 2, 3, 4 }));

                Assert.IsFalse(result);
                Assert.AreEqual(1, client.ErrorCount);
                Assert.AreEqual("udp sync send failed", client.LastError?.Message);
            }
            finally
            {
                client.Dispose();
            }
        }

        [TestMethod]
        public void BaseUdpClient_Send_WhenTryWaitSynchronouslyThrows_ShouldReportErrorWithoutEscalating()
        {
            var client = new ThrowingDirectSyncSendUdpClient(0);

            try
            {
                client.Start();

                var result = client.Send(new SendUdpDataModel(new IPEndPoint(IPAddress.Loopback, 9527), new byte[] { 1, 2, 3, 4 }));

                Assert.IsFalse(result);
                Assert.AreEqual(1, client.TrySendSynchronouslyCallCount);
                Assert.AreEqual(1, client.ErrorCount);
                Assert.AreEqual("udp sync send bridge failed", client.LastError?.Message);
            }
            finally
            {
                client.Dispose();
            }
        }

        [TestMethod]
        public void BaseUdpClient_SendPackage_WhenEncodeThrows_ShouldReportErrorWithoutEscalating()
        {
            var client = new TestUdpClient(0);
            client.TestFilter.ThrowOnEncode = true;

            var result = client.Send(new TestUdpPackage
            {
                RemoteIpEndPoint = new IPEndPoint(IPAddress.Loopback, 9527),
            });

            Assert.IsFalse(result);
            Assert.AreEqual(1, client.ErrorCount);
            Assert.IsNotNull(client.LastError);
            Assert.AreEqual("udp encode package failed", client.LastError.Message);
        }

        [TestMethod]
        public void BaseUdpClient_Dispose_WhenNeverStarted_ShouldSetDisposedState()
        {
            var client = new TestUdpClient(0);

            client.Dispose();

            Assert.IsTrue(client.IsDisposed);
            Assert.IsFalse(client.IsAccept);
        }

        [TestMethod]
        public void BaseUdpClient_Send_AfterDispose_ShouldReturnFalse()
        {
            var client = new TestUdpClient(0);
            client.Dispose();

            var result = client.Send(new SendUdpDataModel(new IPEndPoint(IPAddress.Loopback, 9527), new byte[] { 1, 2, 3, 4 }));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void BaseUdpClient_Start_AfterDispose_ShouldRemainStopped()
        {
            var client = new TestUdpClient(0);
            client.Dispose();

            client.Start();

            Assert.IsTrue(client.IsDisposed);
            Assert.IsFalse(client.IsAccept);
        }

        [TestMethod]
        public void BaseUdpClient_CloseThenRestart_SendShouldReturnTrue()
        {
            var client = new TestUdpClient(0);

            try
            {
                client.Start();
                client.Close();
                client.Start();

                var result = client.Send(new SendUdpDataModel(new IPEndPoint(IPAddress.Loopback, 9527), new byte[] { 1, 2, 3, 4 }));

                Assert.IsTrue(result);
            }
            finally
            {
                client.Dispose();
            }
        }

        [TestMethod]
        public async Task BaseUdpClient_CloseThenRestart_ReceiveQueueShouldContinueProcessingNewMessages()
        {
            var client = new TestUdpClient(0);
            var remoteIpEndPoint = new IPEndPoint(IPAddress.Loopback, 9527);

            try
            {
                client.Start();
                await client.EnqueueReceiveAsync(remoteIpEndPoint, new byte[] { 1, 2, 3, 4 });
                await Task.Delay(50);

                client.Close();
                client.Start();

                await client.EnqueueReceiveAsync(remoteIpEndPoint, new byte[] { 5, 6, 7, 8 });
                await Task.Delay(50);

                Assert.AreEqual(2, client.ReceivedPackageCount);
            }
            finally
            {
                client.Dispose();
            }
        }

        [TestMethod]
        public void BaseUdpClient_MultipleStartCloseCycles_SendStateShouldRemainStable()
        {
            var client = new TestUdpClient(0);

            try
            {
                for (var i = 0; i < 3; i++)
                {
                    client.Start();

                    var sendWhileRunningResult = client.Send(new SendUdpDataModel(new IPEndPoint(IPAddress.Loopback, 9527), new byte[] { 1, 2, 3, 4 }));

                    Assert.IsTrue(sendWhileRunningResult);
                    Assert.IsTrue(client.IsAccept);
                    Assert.IsFalse(client.IsDisposed);

                    client.Close();

                    var sendAfterCloseResult = client.Send(new SendUdpDataModel(new IPEndPoint(IPAddress.Loopback, 9527), new byte[] { 5, 6, 7, 8 }));

                    Assert.IsFalse(sendAfterCloseResult);
                    Assert.IsFalse(client.IsAccept);
                    Assert.IsFalse(client.IsDisposed);
                }
            }
            finally
            {
                client.Dispose();
            }
        }

        [TestMethod]
        public void BaseUdpClient_StaleReceiveCallbackAfterClose_ShouldEndReceiveWithoutRedispatching()
        {
            var client = new CallbackLifecycleUdpClient(0);

            try
            {
                client.Start();
                client.Close();

                client.InvokeLastReceiveCallback();

                Assert.AreEqual(1, client.BeginReceiveCallCount);
                Assert.AreEqual(1, client.EndReceiveCallCount);
                Assert.AreEqual(0, client.ReceivedPackageCount);
                Assert.AreEqual(0, client.ErrorCount);
                Assert.IsFalse(client.IsAccept);
                Assert.IsFalse(client.IsDisposed);
            }
            finally
            {
                client.Dispose();
            }
        }

        [TestMethod]
        public async Task BaseUdpClient_MultipleStartCloseCycles_ReceiveQueueShouldProcessMessagesEveryCycle()
        {
            var client = new TestUdpClient(0);
            var remoteIpEndPoint = new IPEndPoint(IPAddress.Loopback, 9527);

            try
            {
                for (var i = 0; i < 3; i++)
                {
                    client.Start();
                    await client.EnqueueReceiveAsync(remoteIpEndPoint, new byte[] { (byte)i, 2, 3, 4 });
                    await Task.Delay(50);
                    client.Close();
                }

                Assert.AreEqual(3, client.ReceivedPackageCount);
                Assert.IsFalse(client.IsAccept);
                Assert.IsFalse(client.IsDisposed);
            }
            finally
            {
                client.Dispose();
            }
        }

        [TestMethod]
        public void BaseUdpClient_StartThenDispose_ShouldSetDisposedAndRemainStopped()
        {
            var client = new TestUdpClient(0);

            client.Start();
            client.Dispose();

            Assert.IsTrue(client.IsDisposed);
            Assert.IsFalse(client.IsAccept);
            Assert.IsFalse(client.Send(new SendUdpDataModel(new IPEndPoint(IPAddress.Loopback, 9527), new byte[] { 1, 2, 3, 4 })));
        }

        [TestMethod]
        public void BaseUdpClient_CloseThenDisposeThenClose_ShouldRemainStable()
        {
            var client = new TestUdpClient(0);

            client.Start();
            client.Close();
            client.Dispose();
            client.Close();

            Assert.IsTrue(client.IsDisposed);
            Assert.IsFalse(client.IsAccept);
        }

        [TestMethod]
        public void BaseUdpClient_CloseAsync_WhenReceiveQueueStopFails_ShouldReportCloseErrorAndRemainStopped()
        {
            var client = new TestUdpClient(0)
            {
                CloseReceiveQueueException = new InvalidOperationException("receive queue stop failed"),
            };

            client.Start();
            client.Close();

            Assert.IsFalse(client.IsAccept);
            Assert.AreEqual(1, client.ErrorCount);
            StringAssert.Contains(client.LastError?.Message, "UdpClient close receive queue failed.");
            Assert.IsInstanceOfType(client.LastError?.InnerException, typeof(InvalidOperationException));
            Assert.AreEqual("receive queue stop failed", client.LastError?.InnerException?.Message);
        }

        [TestMethod]
        public void BaseUdpClient_Close_WhenCloseAsyncThrows_ShouldReportSyncCloseErrorWithoutEscalating()
        {
            var client = new ThrowingSyncCloseUdpClient(0);

            client.Close();

            Assert.AreEqual(1, client.CloseAsyncCallCount);
            Assert.AreEqual(1, client.ErrorCount);
            Assert.AreEqual("UdpClient sync close failed.", client.LastError?.Message);
            Assert.IsInstanceOfType(client.LastError?.InnerException, typeof(InvalidOperationException));
            Assert.AreEqual("udp close failed", client.LastError?.InnerException?.Message);
        }

        [TestMethod]
        public void BaseUdpClient_Close_WhenTryCloseSynchronouslyThrows_ShouldReportSyncCloseErrorWithoutEscalating()
        {
            var client = new ThrowingDirectSyncCloseUdpClient(0);

            client.Close();

            Assert.AreEqual(1, client.TryCloseSynchronouslyCallCount);
            Assert.AreEqual(1, client.ErrorCount);
            Assert.AreEqual("UdpClient sync close failed.", client.LastError?.Message);
            Assert.IsInstanceOfType(client.LastError?.InnerException, typeof(InvalidOperationException));
            Assert.AreEqual("udp sync bridge failed", client.LastError?.InnerException?.Message);
        }

        [TestMethod]
        public void BaseUdpClient_CloseAsync_WhenQueueStopFails_ShouldReplaceFailedQueuesBeforeRestart()
        {
            var client = new TestUdpClient(0)
            {
                CloseReceiveQueueException = new InvalidOperationException("receive queue stop failed"),
                CloseSendQueueException = new InvalidOperationException("send queue stop failed"),
            };

            try
            {
                client.Start();

                var receiveQueueBeforeClose = client.GetReceiveWorkTaskQueueForTest();
                var sendQueueBeforeClose = client.GetSendWorkTaskQueueForTest();

                client.Close();

                Assert.IsFalse(client.IsAccept);
                Assert.AreNotSame(receiveQueueBeforeClose, client.GetReceiveWorkTaskQueueForTest());
                Assert.AreNotSame(sendQueueBeforeClose, client.GetSendWorkTaskQueueForTest());

                client.CloseReceiveQueueException = null;
                client.CloseSendQueueException = null;

                client.Start();

                Assert.IsTrue(client.IsAccept);
                Assert.IsTrue(client.Send(new SendUdpDataModel(new IPEndPoint(IPAddress.Loopback, 9527), new byte[] { 1, 2, 3, 4 })));
            }
            finally
            {
                client.Dispose();
            }
        }

        [TestMethod]
        public void BaseUdpClient_CloseAsync_WhenCloseEventFails_ShouldReportCloseFinalizationError()
        {
            var client = new TestUdpClient(0)
            {
                CloseEventException = new InvalidOperationException("close callback failed"),
            };

            client.Start();
            client.Close();

            Assert.IsFalse(client.IsAccept);
            Assert.AreEqual(1, client.ErrorCount);
            StringAssert.Contains(client.LastError?.Message, "UdpClient close finalization failed.");
            Assert.IsInstanceOfType(client.LastError?.InnerException, typeof(InvalidOperationException));
            Assert.AreEqual("close callback failed", client.LastError?.InnerException?.Message);
        }

        [TestMethod]
        public void BaseUdpClient_Dispose_WhenReceiveQueueDisposeFails_ShouldReportAndThrow()
        {
            var client = new TestUdpClient(0)
            {
                DisposeReceiveQueueException = new InvalidOperationException("receive queue dispose failed"),
            };

            var exception = Assert.ThrowsExactly<InvalidOperationException>(() => client.Dispose());

            Assert.IsTrue(client.IsDisposed);
            Assert.IsFalse(client.IsAccept);
            Assert.AreEqual("UdpClient dispose receive queue failed.", exception.Message);
            Assert.AreEqual(1, client.ErrorCount);
            Assert.AreEqual("UdpClient dispose receive queue failed.", client.LastError?.Message);
            Assert.IsInstanceOfType(client.LastError?.InnerException, typeof(InvalidOperationException));
            Assert.AreEqual("receive queue dispose failed", client.LastError?.InnerException?.Message);
        }

        [TestMethod]
        public void BaseUdpClient_Dispose_WhenCloseThrows_ShouldReportAndThrowWrappedCloseError()
        {
            var client = new ThrowingSyncCloseUdpClient(0);

            var exception = Assert.ThrowsExactly<InvalidOperationException>(() => client.Dispose());

            Assert.IsTrue(client.IsDisposed);
            Assert.IsFalse(client.IsAccept);
            Assert.AreEqual(1, client.CloseAsyncCallCount);
            Assert.AreEqual("UdpClient dispose close failed.", exception.Message);
            Assert.AreEqual(1, client.ErrorCount);
            Assert.AreEqual("UdpClient dispose close failed.", client.LastError?.Message);
            Assert.IsInstanceOfType(client.LastError?.InnerException, typeof(InvalidOperationException));
            Assert.AreEqual("udp close failed", client.LastError?.InnerException?.Message);
        }

        [TestMethod]
        public void BaseUdpClient_Dispose_WhenTryCloseSynchronouslyThrows_ShouldReportAndContinueCleanup()
        {
            var client = new ThrowingDirectSyncCloseUdpClient(0);

            client.Start();

            var exception = Assert.ThrowsExactly<InvalidOperationException>(() => client.Dispose());

            Assert.IsTrue(client.IsDisposed);
            Assert.IsFalse(client.IsAccept);
            Assert.AreEqual(1, client.TryCloseSynchronouslyCallCount);
            Assert.AreEqual("UdpClient dispose close failed.", exception.Message);
            Assert.AreEqual(1, client.ErrorCount);
            Assert.AreEqual("UdpClient dispose close failed.", client.LastError?.Message);
            Assert.AreEqual("udp sync bridge failed", client.LastError?.InnerException?.Message);
            Assert.IsNull(client.GetReceiveWorkTaskQueueForTest());
            Assert.IsNull(client.GetSendWorkTaskQueueForTest());
        }

        [TestMethod]
        public void BaseUdpClient_WhileRunning_ShouldProvideStableReceiveSnapshot()
        {
            var client = new TestUdpClient(0);

            try
            {
                client.Start();

                Assert.IsTrue(client.TryGetReceiveContextForTest());
                Assert.IsTrue(client.CanContinueReceiveForCurrentSnapshot());
            }
            finally
            {
                client.Dispose();
            }
        }

        [TestMethod]
        public void BaseUdpClient_AfterClose_ShouldNotProvideReceiveSnapshot()
        {
            var client = new TestUdpClient(0);

            client.Start();
            client.Close();

            try
            {
                Assert.IsFalse(client.TryGetReceiveContextForTest());
                Assert.IsFalse(client.CanContinueReceiveForCurrentSnapshot());
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
