using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class SocketServerTests
    {
        private sealed class TestSessionToken : BaseSessionToken
        {
            public override byte[] CacheHeartBytes => null;

            public TestSessionToken(string ip, int port)
                : base(ip, port)
            {
            }
        }

        private sealed class TestFixedHeaderPackageFilter : BaseFixedHeaderPackageFilter<object, TestSendPackage, TestSessionToken>
        {
            private readonly byte[] _packageBytes;
            private readonly bool _checkPackageResult;
            private readonly bool _throwOnEncode;

            public TestFixedHeaderPackageFilter(byte[] packageBytes = null, bool checkPackageResult = true, bool throwOnEncode = false)
                : base(4)
            {
                _packageBytes = packageBytes;
                _checkPackageResult = checkPackageResult;
                _throwOnEncode = throwOnEncode;
            }

            public override int GetBodyLengthFromHeader(int cursorIndex, byte[] bufferBytes)
            {
                return 0;
            }

            public override bool CheckPackage(byte[] packageBytes)
            {
                return _checkPackageResult;
            }

            public override byte[] EncodePackage(TestSendPackage sendPackage)
            {
                if (_throwOnEncode)
                {
                    throw new InvalidOperationException("encode package failed");
                }

                return Array.Empty<byte>();
            }

            public override object DecodePackage(byte[] packageBytes)
            {
                return packageBytes;
            }

            public override byte[] GetHeartBytes(ISessionToken sessionToken)
            {
                return Array.Empty<byte>();
            }

            public override byte[] GetPackageBytes(BufferModel buffer, CacheModel cache)
            {
                return _packageBytes;
            }
        }

        private sealed class TestSendPackage : ISendPackageSendNum
        {
            public byte SendNum { get; set; }
        }

        private class TestTcpServerClient : BaseTcpServerClient
        {
            public int CloseAsyncCallCount { get; protected set; }
            public int SendCallCount { get; private set; }
            public Exception SendException { get; set; }

            public TestTcpServerClient()
                : base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, heartIntervalMilliseconds: 1)
            {
            }

            protected override void OnStartReceiveEvent()
            {
            }

            protected override void OnCloseEvent()
            {
            }

            protected override void OnServerClientErrorEvent(Exception ex)
            {
            }

            protected override void OnReceiveDataEvent(BufferModel buffer, CacheModel cache)
            {
            }

            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                return Task.CompletedTask;
            }

            public override Task SendAsync(byte[] sendDataBytes)
            {
                return Task.CompletedTask;
            }

            public override void Send(byte[] sendDataBytes)
            {
                SendCallCount++;

                if (SendException != null)
                {
                    throw SendException;
                }
            }

            protected void RaiseCloseEventForTest()
            {
                var closeEventField = typeof(BaseTcpServerClient).GetField("CloseEvent", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(closeEventField);

                var closeEvent = closeEventField.GetValue(this) as TcpCloseEvent;
                closeEvent?.Invoke(this);
            }
        }

        private sealed class ReentrantCloseEventTcpServerClient : TestTcpServerClient
        {
            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                RaiseCloseEventForTest();
                return Task.CompletedTask;
            }
        }

        private sealed class ThrowingStartSendWorkTaskQueue : WorkTaskQueue<byte[]>
        {
            private readonly Exception _Exception;

            public ThrowingStartSendWorkTaskQueue(Exception exception)
                : base(_ => { }, null, taskSleepMilliseconds: 1)
            {
                _Exception = exception;
            }

            protected override Task OnStartAsync()
            {
                throw _Exception;
            }
        }

        private sealed class TestTcpServer : BaseTcpServer<TestTcpServerClient, TestSessionToken, TestFixedHeaderPackageFilter, object, TestSendPackage>
        {
            public int ErrorCallbackCount { get; private set; }
            public int CloseCallbackCount { get; private set; }
            public int StartReceiveCallbackCount { get; private set; }
            public int ReceiveDataCallbackCount { get; private set; }
            public int HeartCallbackCount { get; private set; }
            public int ReceivePackageCallbackCount { get; private set; }
            public Exception LastError { get; private set; }
            public ITcpServerClient LastErrorClient { get; private set; }
            public Exception AcceptException { get; set; }
            public bool CanSendDataResult { get; set; }
            public bool ThrowOnBindAndListen { get; set; }
            public int BindAndListenCallCount { get; private set; }
            public bool BlockBindAndListen { get; set; }
            private readonly ManualResetEventSlim _BindAndListenEnteredEvent = new ManualResetEventSlim(false);
            private readonly ManualResetEventSlim _ContinueBindAndListenEvent = new ManualResetEventSlim(true);

            public TestTcpServer(TestFixedHeaderPackageFilter fixedHeaderPackageFilter, int port = 9527)
                : base(fixedHeaderPackageFilter, port: port, receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, intervalHeartTotalMilliseconds: 1, heartTimeOutCount: 1)
            {
            }

            protected override TestTcpServerClient CreateTcpServerClient(Socket client)
            {
                throw new NotSupportedException();
            }

            protected override void OnServerClientHeartCallBackEvent(ITcpServerClient tcpServerClient)
            {
                HeartCallbackCount++;
            }

            protected override void OnServerClientCloseCallBackEvent(ITcpServerClient tcpServerClient)
            {
                CloseCallbackCount++;
            }

            protected override void OnServerClientStartReceiveCallBackEvent(ITcpServerClient tcpServerClient)
            {
                StartReceiveCallbackCount++;
            }

            protected override void OnServerClientReceiveDataCallBackEvent(ITcpServerClient tcpServerClient, BufferModel buffer, CacheModel cache)
            {
                ReceiveDataCallbackCount++;
            }

            protected override void OnServerReceivePackageEvent(object package, ISessionToken sessionToken)
            {
                ReceivePackageCallbackCount++;
            }

            protected override TestSessionToken CreateSessionToken(string ip, int port)
            {
                return new TestSessionToken(ip, port);
            }

            protected override bool CanSendData(ISessionToken sessionToken)
            {
                return CanSendDataResult;
            }

            protected override void BindAndListenSocket(Socket currentSocket, IPEndPoint ipEndPoint)
            {
                BindAndListenCallCount++;

                if (BlockBindAndListen)
                {
                    _BindAndListenEnteredEvent.Set();
                    _ContinueBindAndListenEvent.Wait(TimeSpan.FromSeconds(3));
                }

                if (ThrowOnBindAndListen)
                {
                    ThrowOnBindAndListen = false;
                    throw new InvalidOperationException("bind listen failed");
                }

                base.BindAndListenSocket(currentSocket, ipEndPoint);
            }

            protected override void OnServerCloseEvent()
            {
            }

            protected override void OnAcceptEvent(ITcpServerClient client)
            {
                if (AcceptException != null)
                {
                    throw AcceptException;
                }
            }

            protected override void OnServerClientErrorCallBackEvent(ITcpServerClient client, Exception ex)
            {
                ErrorCallbackCount++;
                LastError = ex;
                LastErrorClient = client;
            }

            protected override void OnServerErrorEvent(Exception ex)
            {
                throw new AssertFailedException($"Unexpected server error: {ex.Message}");
            }

            public void TrackClient(TestTcpServerClient client)
            {
                _TcpServerClientDic[client.CurrentSessionToken.SessionID] = client;
            }

            public void AttachTrackedClientForTest(ITcpServerClient client)
            {
                AttachTcpServerClientEventHandlers(client);
                _TcpServerClientDic[client.CurrentSessionToken.SessionID] = client;
            }

            public bool HasTrackedClient(Guid sessionId)
            {
                return _TcpServerClientDic.ContainsKey(sessionId);
            }

            public void TriggerHeart(TestTcpServerClient client)
            {
                OnServerClientHeartEvent(client);
            }

            public void TriggerServerClientErrorForTest(ITcpServerClient client, Exception ex)
            {
                OnServerClientErrorEvent(client, ex);
            }

            public void TriggerServerClientCloseForTest(ITcpServerClient client)
            {
                OnServerClientCloseEvent(client);
            }

            public void TriggerServerClientStartReceiveForTest(ITcpServerClient client)
            {
                OnServerClientStartReceiveEvent(client);
            }

            public void TriggerServerClientReceiveDataForTest(ITcpServerClient client, BufferModel buffer, CacheModel cache)
            {
                OnServerClientReceiveDataEvent(client, buffer, cache);
            }

            public void TriggerReceiveLoop(TestTcpServerClient client, BufferModel buffer, CacheModel cache)
            {
                OnServerClientReceiveDataLoopEvent(client, buffer, cache);
            }

            public void TriggerAccept(TestTcpServerClient client)
            {
                OnAccept(client);
            }

            public bool CanIgnoreAcceptExceptionForTest(Exception ex, Socket acceptSocket)
            {
                return CanIgnoreAcceptException(ex, acceptSocket);
            }

            public void SetAcceptContextForTest(Socket currentSocket, bool isRunning)
            {
                CurrentSocket = currentSocket;
                _IsRunning = isRunning;
            }

            public bool WaitForBindAndListenEntered(TimeSpan timeout)
            {
                return _BindAndListenEnteredEvent.Wait(timeout);
            }

            public void PrepareBlockedBindAndListen()
            {
                BlockBindAndListen = true;
                _BindAndListenEnteredEvent.Reset();
                _ContinueBindAndListenEvent.Reset();
            }

            public void ReleaseBlockedBindAndListen()
            {
                BlockBindAndListen = false;
                _ContinueBindAndListenEvent.Set();
            }
        }

        private sealed class StartReceiveSendQueueFailureTcpServerClient : BaseTcpServerClient
        {
            public StartReceiveSendQueueFailureTcpServerClient(Socket socket)
                : base(socket, receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, heartIntervalMilliseconds: 1)
            {
                _CurrentSendWorkTaskQueue = new ThrowingStartSendWorkTaskQueue(new InvalidOperationException("start send queue failed"));
            }

            protected override void OnStartReceiveEvent()
            {
            }

            protected override void OnCloseEvent()
            {
            }

            protected override void OnServerClientErrorEvent(Exception ex)
            {
            }

            protected override void OnReceiveDataEvent(BufferModel buffer, CacheModel cache)
            {
            }
        }

        private sealed class StartReceiveSendQueueFailureAcceptServer : BaseTcpServer<StartReceiveSendQueueFailureTcpServerClient, TestSessionToken, TestFixedHeaderPackageFilter, object, TestSendPackage>
        {
            private readonly TaskCompletionSource<bool> _ErrorSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            public int AcceptCallbackCount { get; private set; }
            public int ErrorCallbackCount { get; private set; }

            public StartReceiveSendQueueFailureAcceptServer()
                : base(new TestFixedHeaderPackageFilter(), port: 0, receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, intervalHeartTotalMilliseconds: 1, heartTimeOutCount: 1)
            {
            }

            protected override StartReceiveSendQueueFailureTcpServerClient CreateTcpServerClient(Socket client)
            {
                return new StartReceiveSendQueueFailureTcpServerClient(client);
            }

            protected override void OnServerClientHeartCallBackEvent(ITcpServerClient tcpServerClient)
            {
            }

            protected override void OnServerClientCloseCallBackEvent(ITcpServerClient tcpServerClient)
            {
            }

            protected override void OnServerClientStartReceiveCallBackEvent(ITcpServerClient tcpServerClient)
            {
            }

            protected override void OnServerClientReceiveDataCallBackEvent(ITcpServerClient tcpServerClient, BufferModel buffer, CacheModel cache)
            {
            }

            protected override void OnServerReceivePackageEvent(object package, ISessionToken sessionToken)
            {
            }

            protected override TestSessionToken CreateSessionToken(string ip, int port)
            {
                return new TestSessionToken(ip, port);
            }

            protected override bool CanSendData(ISessionToken sessionToken)
            {
                return true;
            }

            protected override void OnServerCloseEvent()
            {
            }

            protected override void OnAcceptEvent(ITcpServerClient client)
            {
                AcceptCallbackCount++;
            }

            protected override void OnServerClientErrorCallBackEvent(ITcpServerClient client, Exception ex)
            {
                ErrorCallbackCount++;
                _ErrorSignal.TrySetResult(true);
            }

            protected override void OnServerErrorEvent(Exception ex)
            {
                _ErrorSignal.TrySetException(ex);
            }

            public Task WaitForErrorAsync()
            {
                return _ErrorSignal.Task;
            }

            public int TrackedClientCount => _TcpServerClientDic.Count;
        }

        [TestMethod]
        public void OnServerClientHeartEvent_WithTimedOutSession_ShouldCloseClient()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter());
            var client = new TestTcpServerClient();
            var currentTotalMilliseconds = DateTimeHelper.GetTotalMillisecondsFromInstantiation(DateTime.Now);
            var sessionToken = new TestSessionToken("127.0.0.1", 9527)
            {
                LastReceiveDateTimeTotalMillisecondsFromInstantiation = currentTotalMilliseconds > 10 ? currentTotalMilliseconds - 10 : 0,
            };

            client.CurrentSessionToken = sessionToken;
            server.TrackClient(client);

            server.TriggerHeart(client);

            Assert.AreEqual(1, server.ErrorCallbackCount);
            Assert.AreEqual("心跳超时断开连接", server.LastError.Message);
            Assert.AreEqual(1, client.CloseAsyncCallCount);
            Assert.IsFalse(server.HasTrackedClient(sessionToken.SessionID));
        }

        [TestMethod]
        public void OnServerClientReceiveDataLoopEvent_WithInvalidPackage_ShouldCloseClient()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter(new byte[] { 0xAA }, checkPackageResult: false));
            var client = new TestTcpServerClient();
            var sessionToken = new TestSessionToken("127.0.0.1", 9527);

            client.CurrentSessionToken = sessionToken;
            server.TrackClient(client);

            server.TriggerReceiveLoop(client, new BufferModel(), new CacheModel());

            Assert.AreEqual(1, server.ErrorCallbackCount);
            Assert.AreEqual("data bytes error", server.LastError.Message);
            Assert.AreEqual(1, client.CloseAsyncCallCount);
            Assert.IsFalse(server.HasTrackedClient(sessionToken.SessionID));
        }

        [TestMethod]
        public void BaseTcpServer_Dispose_ShouldSetDisposedStateAndPreventRestart()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter());

            server.Dispose();
            server.Start();

            Assert.IsTrue(server.IsDisposed);
            Assert.IsFalse(server.IsAccept);
        }

        [TestMethod]
        public void BaseTcpServer_Start_WhenAlreadyRunning_ShouldKeepCurrentSocket()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter(), port: 0);

            try
            {
                server.Start();
                var firstSocket = server.CurrentSocket;

                server.Start();

                Assert.IsTrue(server.IsAccept);
                Assert.AreSame(firstSocket, server.CurrentSocket);
            }
            finally
            {
                server.Dispose();
            }
        }

        [TestMethod]
        public async Task BaseTcpServer_CloseAsync_ShouldAllowRestart()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter(), port: 0);

            try
            {
                server.Start();
                var firstSocket = server.CurrentSocket;

                await server.CloseAsync();

                Assert.IsFalse(server.IsRunning);
                Assert.IsFalse(server.IsAccept);
                Assert.IsNull(server.CurrentSocket);

                server.Start();

                Assert.IsTrue(server.IsRunning);
                Assert.IsTrue(server.IsAccept);
                Assert.IsNotNull(server.CurrentSocket);
                Assert.AreNotSame(firstSocket, server.CurrentSocket);
            }
            finally
            {
                server.Dispose();
            }
        }

        [TestMethod]
        public async Task BaseTcpServer_CloseAsync_WhenClientCloseRaisesCloseEvent_ShouldNotReenterClose()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter(), port: 0);
            var client = new ReentrantCloseEventTcpServerClient
            {
                CurrentSessionToken = new TestSessionToken("127.0.0.1", 9527),
            };

            using var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server.SetAcceptContextForTest(listenSocket, true);
                server.AttachTrackedClientForTest(client);

                await server.CloseAsync();

                Assert.AreEqual(1, client.CloseAsyncCallCount);
                Assert.AreEqual(0, server.CloseCallbackCount);
                Assert.IsFalse(server.HasTrackedClient(client.CurrentSessionToken.SessionID));
            }
            finally
            {
                server.Dispose();
            }
        }

        [TestMethod]
        public void BaseTcpServer_StaleClientErrorCloseAndStartReceiveEvents_ShouldBeIgnored()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter(), port: 0);
            var client = new TestTcpServerClient
            {
                CurrentSessionToken = new TestSessionToken("127.0.0.1", 9527),
            };

            server.TriggerServerClientErrorForTest(client, new InvalidOperationException("stale error"));
            server.TriggerServerClientCloseForTest(client);
            server.TriggerServerClientStartReceiveForTest(client);

            Assert.AreEqual(0, server.ErrorCallbackCount);
            Assert.AreEqual(0, server.CloseCallbackCount);
            Assert.AreEqual(0, server.StartReceiveCallbackCount);
            Assert.AreEqual(0, client.CloseAsyncCallCount);
        }

        [TestMethod]
        public void BaseTcpServer_StaleClientReceiveAndHeartEvents_ShouldBeIgnored()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter(new byte[] { 0x01 }));
            var client = new TestTcpServerClient
            {
                CurrentSessionToken = new TestSessionToken("127.0.0.1", 9527),
            };

            server.TriggerServerClientReceiveDataForTest(client, new BufferModel(), new CacheModel());
            server.TriggerHeart(client);

            Assert.AreEqual(0, server.ReceiveDataCallbackCount);
            Assert.AreEqual(0, server.HeartCallbackCount);
            Assert.AreEqual(0, client.SendCallCount);
            Assert.AreEqual(0, client.CloseAsyncCallCount);
        }

        [TestMethod]
        public void BaseTcpServer_StaleClientReceiveLoop_ShouldBeIgnored()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter(new byte[] { 0x01 }));
            var client = new TestTcpServerClient
            {
                CurrentSessionToken = new TestSessionToken("127.0.0.1", 9527),
            };

            server.TriggerReceiveLoop(client, new BufferModel(), new CacheModel());

            Assert.AreEqual(0, server.ReceivePackageCallbackCount);
            Assert.AreEqual(0, server.ErrorCallbackCount);
            Assert.AreEqual(0, client.CloseAsyncCallCount);
        }

        [TestMethod]
        public void SendDataBytes_WhenClientIsNotManaged_ShouldIgnoreSend()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter())
            {
                CanSendDataResult = true,
            };
            var client = new TestTcpServerClient
            {
                CurrentSessionToken = new TestSessionToken("127.0.0.1", 9527),
            };

            server.SendDataBytes(client, new byte[] { 0x01 });

            Assert.AreEqual(0, client.SendCallCount);
            Assert.AreEqual(0, server.ErrorCallbackCount);
            Assert.AreEqual(0, client.CloseAsyncCallCount);
        }

        [TestMethod]
        public void BaseTcpServer_Start_WhenBindListenFails_ShouldResetStateAndAllowRetry()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter(), port: 0)
            {
                ThrowOnBindAndListen = true,
            };

            try
            {
                Assert.ThrowsExactly<InvalidOperationException>(() => server.Start());

                Assert.IsFalse(server.IsRunning);
                Assert.IsFalse(server.IsAccept);
                Assert.IsNull(server.CurrentSocket);
                Assert.AreEqual(1, server.BindAndListenCallCount);

                server.Start();

                Assert.IsTrue(server.IsRunning);
                Assert.IsTrue(server.IsAccept);
                Assert.IsNotNull(server.CurrentSocket);
                Assert.AreEqual(2, server.BindAndListenCallCount);
            }
            finally
            {
                server.Dispose();
            }
        }

        [TestMethod]
        public async Task BaseTcpServer_CloseAsync_WhenStartInProgress_ShouldCancelStartupAndAllowRetry()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter(), port: 0);

            try
            {
                server.PrepareBlockedBindAndListen();
                var startTask = Task.Run(() => server.Start());

                Assert.IsTrue(server.WaitForBindAndListenEntered(TimeSpan.FromSeconds(3)));

                await server.CloseAsync();
                server.ReleaseBlockedBindAndListen();
                await startTask;

                Assert.IsFalse(server.IsRunning);
                Assert.IsFalse(server.IsAccept);
                Assert.IsNull(server.CurrentSocket);

                server.Start();

                Assert.IsTrue(server.IsRunning);
                Assert.IsTrue(server.IsAccept);
                Assert.IsNotNull(server.CurrentSocket);
            }
            finally
            {
                server.ReleaseBlockedBindAndListen();
                server.Dispose();
            }
        }

        [TestMethod]
        public void CanIgnoreAcceptException_WhenServerRestartedAndSocketReplaced_ShouldIgnoreDisposedOldSocket()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter(), port: 0);
            using var oldSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            using var newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            server.SetAcceptContextForTest(newSocket, true);

            var result = server.CanIgnoreAcceptExceptionForTest(new ObjectDisposedException(nameof(Socket)), oldSocket);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanIgnoreAcceptException_WhenCurrentSocketStillRunning_ShouldNotIgnore()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter(), port: 0);
            using var currentSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            server.SetAcceptContextForTest(currentSocket, true);

            var result = server.CanIgnoreAcceptExceptionForTest(new ObjectDisposedException(nameof(Socket)), currentSocket);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void OnAcceptEvent_Throw_ShouldReportErrorWithoutClosingClient()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter())
            {
                AcceptException = new InvalidOperationException("accept callback failed"),
            };
            var client = new TestTcpServerClient();
            var sessionToken = new TestSessionToken("127.0.0.1", 9527);

            client.CurrentSessionToken = sessionToken;
            server.TrackClient(client);

            server.TriggerAccept(client);

            Assert.AreEqual(1, server.ErrorCallbackCount);
            Assert.AreEqual("accept callback failed", server.LastError.Message);
            Assert.AreSame(client, server.LastErrorClient);
            Assert.AreEqual(0, client.CloseAsyncCallCount);
            Assert.IsTrue(server.HasTrackedClient(sessionToken.SessionID));
        }

        [TestMethod]
        public void SendDataBytes_WhenClientSendThrows_ShouldReportErrorAndCloseClient()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter())
            {
                CanSendDataResult = true,
            };
            var client = new TestTcpServerClient
            {
                SendException = new InvalidOperationException("send failed"),
            };
            var sessionToken = new TestSessionToken("127.0.0.1", 9527);

            client.CurrentSessionToken = sessionToken;
            server.TrackClient(client);

            server.SendDataBytes(client, new byte[] { 0x01 });

            Assert.AreEqual(1, client.SendCallCount);
            Assert.AreEqual(1, server.ErrorCallbackCount);
            Assert.AreEqual("send failed", server.LastError.Message);
            Assert.AreSame(client, server.LastErrorClient);
            Assert.AreEqual(1, client.CloseAsyncCallCount);
            Assert.IsFalse(server.HasTrackedClient(sessionToken.SessionID));
        }

        [TestMethod]
        public void SendPackage_WhenEncodeThrows_ShouldReportErrorWithoutClosingClient()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter(throwOnEncode: true))
            {
                CanSendDataResult = true,
            };
            var client = new TestTcpServerClient();
            var sessionToken = new TestSessionToken("127.0.0.1", 9527);
            var sendPackage = new TestSendPackage();

            client.CurrentSessionToken = sessionToken;
            server.TrackClient(client);

            server.SendPackage(sessionToken.SessionID, sendPackage);

            Assert.AreEqual(1, server.ErrorCallbackCount);
            Assert.AreEqual("encode package failed", server.LastError.Message);
            Assert.AreSame(client, server.LastErrorClient);
            Assert.AreEqual(0, client.SendCallCount);
            Assert.AreEqual(0, client.CloseAsyncCallCount);
            Assert.IsTrue(server.HasTrackedClient(sessionToken.SessionID));
        }

        [TestMethod]
        public async Task BeginAcceptAsync_WhenSendQueueStartFails_ShouldCloseClientAndNotInvokeAcceptCallback()
        {
            var server = new StartReceiveSendQueueFailureAcceptServer();

            try
            {
                server.Start();
                var port = ((IPEndPoint)server.CurrentSocket.LocalEndPoint).Port;

                using var remoteClient = new TcpClient();
                await remoteClient.ConnectAsync(IPAddress.Loopback, port);

                var errorTask = server.WaitForErrorAsync();
                var completedTask = await Task.WhenAny(errorTask, Task.Delay(TimeSpan.FromSeconds(3)));

                Assert.AreSame(errorTask, completedTask);
                await errorTask;

                await Task.Delay(100);

                Assert.AreEqual(1, server.ErrorCallbackCount);
                Assert.AreEqual(0, server.AcceptCallbackCount);
                Assert.AreEqual(0, server.TrackedClientCount);
            }
            finally
            {
                await server.CloseAsync();
            }
        }
    }
}
