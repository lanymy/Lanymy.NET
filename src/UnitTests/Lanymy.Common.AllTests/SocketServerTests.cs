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

        private class TestFixedHeaderPackageFilter : BaseFixedHeaderPackageFilter<object, TestSendPackage, TestSessionToken>
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

        private sealed class SinglePackageFixedHeaderPackageFilter : TestFixedHeaderPackageFilter
        {
            private int _GetPackageCallCount;

            public SinglePackageFixedHeaderPackageFilter(byte[] packageBytes = null)
                : base(packageBytes ?? new byte[] { 0x01 })
            {
            }

            public override byte[] GetPackageBytes(BufferModel buffer, CacheModel cache)
            {
                return Interlocked.Increment(ref _GetPackageCallCount) == 1
                    ? base.GetPackageBytes(buffer, cache)
                    : null;
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

        private sealed class ThrowingCloseTcpServerClient : TestTcpServerClient
        {
            public void MarkRunningForTest()
            {
                _IsRunning = true;
            }

            public bool HasHeartTimerForTest()
            {
                return _CurrentHeartTimerWorkTask != null;
            }

            public bool HasSendQueueForTest()
            {
                return _CurrentSendWorkTaskQueue != null;
            }

            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                _IsRunning = false;
                throw new InvalidOperationException("child close failed");
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
            public Exception StartReceiveCallbackException { get; set; }
            public Exception ReceiveDataCallbackException { get; set; }
            public Exception HeartCallbackException { get; set; }
            public Exception ReceivePackageException { get; set; }
            public Exception CanSendDataException { get; set; }
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

                if (HeartCallbackException != null)
                {
                    throw HeartCallbackException;
                }
            }

            protected override void OnServerClientCloseCallBackEvent(ITcpServerClient tcpServerClient)
            {
                CloseCallbackCount++;
            }

            protected override void OnServerClientStartReceiveCallBackEvent(ITcpServerClient tcpServerClient)
            {
                StartReceiveCallbackCount++;

                if (StartReceiveCallbackException != null)
                {
                    throw StartReceiveCallbackException;
                }
            }

            protected override void OnServerClientReceiveDataCallBackEvent(ITcpServerClient tcpServerClient, BufferModel buffer, CacheModel cache)
            {
                ReceiveDataCallbackCount++;

                if (ReceiveDataCallbackException != null)
                {
                    throw ReceiveDataCallbackException;
                }
            }

            protected override void OnServerReceivePackageEvent(object package, ISessionToken sessionToken)
            {
                ReceivePackageCallbackCount++;

                if (ReceivePackageException != null)
                {
                    throw ReceivePackageException;
                }
            }

            protected override TestSessionToken CreateSessionToken(string ip, int port)
            {
                return new TestSessionToken(ip, port);
            }

            protected override bool CanSendData(ISessionToken sessionToken)
            {
                if (CanSendDataException != null)
                {
                    throw CanSendDataException;
                }

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

        private sealed class ThrowingSyncCloseTcpServer : BaseTcpServer<TestTcpServerClient, TestSessionToken, TestFixedHeaderPackageFilter, object, TestSendPackage>
        {
            public int CloseAsyncCallCount { get; private set; }
            public int ServerErrorCount { get; private set; }
            public Exception LastServerError { get; private set; }

            public ThrowingSyncCloseTcpServer()
                : base(new TestFixedHeaderPackageFilter(), port: 0, receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, intervalHeartTotalMilliseconds: 1, heartTimeOutCount: 1)
            {
            }

            protected override TestTcpServerClient CreateTcpServerClient(Socket client)
            {
                throw new NotSupportedException();
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
            }

            protected override void OnServerClientErrorCallBackEvent(ITcpServerClient client, Exception ex)
            {
            }

            protected override void OnServerErrorEvent(Exception ex)
            {
                ServerErrorCount++;
                LastServerError = ex;
            }

            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                _IsRunning = false;
                throw new InvalidOperationException("tcp server close failed");
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

            public void SetAcceptContextForTest(Socket currentSocket, bool isRunning)
            {
                CurrentSocket = currentSocket;
                _IsRunning = isRunning;
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

        private sealed class AcceptedTcpServerClient : BaseTcpServerClient
        {
            public AcceptedTcpServerClient(Socket socket, bool disableSendQueue = false)
                : base(socket, receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, heartIntervalMilliseconds: 1)
            {
                if (disableSendQueue)
                {
                    _CurrentSendWorkTaskQueue = null;
                }
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

            public bool HasHeartTimerForTest()
            {
                return _CurrentHeartTimerWorkTask != null;
            }

            public bool HasSendQueueForTest()
            {
                return _CurrentSendWorkTaskQueue != null;
            }
        }

        private sealed class IncompleteStartReceiveAcceptServer : BaseTcpServer<AcceptedTcpServerClient, TestSessionToken, TestFixedHeaderPackageFilter, object, TestSendPackage>
        {
            private readonly TaskCompletionSource<bool> _AcceptSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            private int _CreateCallCount;

            public int AcceptCallbackCount { get; private set; }
            public int ServerErrorCount { get; private set; }
            public Exception LastServerError { get; private set; }
            public AcceptedTcpServerClient FirstAcceptedClient { get; private set; }
            public int TrackedClientCount => _TcpServerClientDic.Count;

            public IncompleteStartReceiveAcceptServer()
                : base(new TestFixedHeaderPackageFilter(), port: 0, receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, intervalHeartTotalMilliseconds: 1, heartTimeOutCount: 1)
            {
            }

            protected override AcceptedTcpServerClient CreateTcpServerClient(Socket client)
            {
                if (Interlocked.Increment(ref _CreateCallCount) == 1)
                {
                    var failedClient = new AcceptedTcpServerClient(client, disableSendQueue: true);
                    FirstAcceptedClient = failedClient;
                    return failedClient;
                }

                return new AcceptedTcpServerClient(client);
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
                _AcceptSignal.TrySetResult(true);
            }

            protected override void OnServerClientErrorCallBackEvent(ITcpServerClient client, Exception ex)
            {
            }

            protected override void OnServerErrorEvent(Exception ex)
            {
                ServerErrorCount++;
                LastServerError = ex;
            }

            public Task WaitForAcceptAsync()
            {
                return _AcceptSignal.Task;
            }
        }

        private sealed class ThrowingCloseFinalizationTcpServer : BaseTcpServer<TestTcpServerClient, TestSessionToken, TestFixedHeaderPackageFilter, object, TestSendPackage>
        {
            public int ServerErrorCount { get; private set; }
            public Exception LastServerError { get; private set; }

            public ThrowingCloseFinalizationTcpServer()
                : base(new TestFixedHeaderPackageFilter(), port: 0, receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, intervalHeartTotalMilliseconds: 1, heartTimeOutCount: 1)
            {
            }

            protected override TestTcpServerClient CreateTcpServerClient(Socket client)
            {
                throw new NotSupportedException();
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
                throw new InvalidOperationException("server close finalization failed");
            }

            protected override void OnAcceptEvent(ITcpServerClient client)
            {
            }

            protected override void OnServerClientErrorCallBackEvent(ITcpServerClient client, Exception ex)
            {
            }

            protected override void OnServerErrorEvent(Exception ex)
            {
                ServerErrorCount++;
                LastServerError = ex;
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

            public void SetAcceptContextForTest(Socket currentSocket, bool isRunning)
            {
                CurrentSocket = currentSocket;
                _IsRunning = isRunning;
            }
        }

        private sealed class ThrowingErrorCallbackTcpServer : BaseTcpServer<TestTcpServerClient, TestSessionToken, TestFixedHeaderPackageFilter, object, TestSendPackage>
        {
            public int ServerErrorCount { get; private set; }
            public Exception LastServerError { get; private set; }

            public ThrowingErrorCallbackTcpServer()
                : base(new TestFixedHeaderPackageFilter(), port: 0, receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, intervalHeartTotalMilliseconds: 1, heartTimeOutCount: 1)
            {
            }

            protected override TestTcpServerClient CreateTcpServerClient(Socket client)
            {
                throw new NotSupportedException();
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
            }

            protected override void OnServerClientErrorCallBackEvent(ITcpServerClient client, Exception ex)
            {
                throw new InvalidOperationException("error callback failed");
            }

            protected override void OnServerErrorEvent(Exception ex)
            {
                ServerErrorCount++;
                LastServerError = ex;
            }

            public void TrackClient(TestTcpServerClient client)
            {
                _TcpServerClientDic[client.CurrentSessionToken.SessionID] = client;
            }

            public bool HasTrackedClient(Guid sessionId)
            {
                return _TcpServerClientDic.ContainsKey(sessionId);
            }
        }

        private sealed class ThrowingCloseCallbackTcpServer : BaseTcpServer<TestTcpServerClient, TestSessionToken, TestFixedHeaderPackageFilter, object, TestSendPackage>
        {
            public int ErrorCallbackCount { get; private set; }
            public Exception LastError { get; private set; }
            public int ServerErrorCount { get; private set; }

            public ThrowingCloseCallbackTcpServer()
                : base(new TestFixedHeaderPackageFilter(), port: 0, receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, intervalHeartTotalMilliseconds: 1, heartTimeOutCount: 1)
            {
            }

            protected override TestTcpServerClient CreateTcpServerClient(Socket client)
            {
                throw new NotSupportedException();
            }

            protected override void OnServerClientHeartCallBackEvent(ITcpServerClient tcpServerClient)
            {
            }

            protected override void OnServerClientCloseCallBackEvent(ITcpServerClient tcpServerClient)
            {
                throw new InvalidOperationException("close callback failed");
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
            }

            protected override void OnServerClientErrorCallBackEvent(ITcpServerClient client, Exception ex)
            {
                ErrorCallbackCount++;
                LastError = ex;
            }

            protected override void OnServerErrorEvent(Exception ex)
            {
                ServerErrorCount++;
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

            public void TriggerServerClientCloseForTest(ITcpServerClient client)
            {
                OnServerClientCloseEvent(client);
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

        private sealed class ThrowingCreateClientAcceptServer : BaseTcpServer<AcceptedTcpServerClient, TestSessionToken, TestFixedHeaderPackageFilter, object, TestSendPackage>
        {
            private readonly TaskCompletionSource<bool> _ServerErrorSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            private readonly TaskCompletionSource<bool> _AcceptSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            private int _CreateCallCount;

            public int AcceptCallbackCount { get; private set; }
            public int ServerErrorCount { get; private set; }
            public Exception LastServerError { get; private set; }

            public ThrowingCreateClientAcceptServer()
                : base(new TestFixedHeaderPackageFilter(), port: 0, receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, intervalHeartTotalMilliseconds: 1, heartTimeOutCount: 1)
            {
            }

            protected override AcceptedTcpServerClient CreateTcpServerClient(Socket client)
            {
                if (Interlocked.Increment(ref _CreateCallCount) == 1)
                {
                    throw new InvalidOperationException("create client failed");
                }

                return new AcceptedTcpServerClient(client);
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
                _AcceptSignal.TrySetResult(true);
            }

            protected override void OnServerClientErrorCallBackEvent(ITcpServerClient client, Exception ex)
            {
            }

            protected override void OnServerErrorEvent(Exception ex)
            {
                ServerErrorCount++;
                LastServerError = ex;
                _ServerErrorSignal.TrySetResult(true);
            }

            public Task WaitForServerErrorAsync()
            {
                return _ServerErrorSignal.Task;
            }

            public Task WaitForAcceptAsync()
            {
                return _AcceptSignal.Task;
            }
        }

        private sealed class ThrowingSessionTokenAcceptServer : BaseTcpServer<AcceptedTcpServerClient, TestSessionToken, TestFixedHeaderPackageFilter, object, TestSendPackage>
        {
            private readonly TaskCompletionSource<bool> _ServerErrorSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            private readonly TaskCompletionSource<bool> _AcceptSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            private int _CreateSessionTokenCallCount;

            public int AcceptCallbackCount { get; private set; }
            public int ServerErrorCount { get; private set; }
            public Exception LastServerError { get; private set; }
            public AcceptedTcpServerClient FirstAcceptedClient { get; private set; }

            public ThrowingSessionTokenAcceptServer()
                : base(new TestFixedHeaderPackageFilter(), port: 0, receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, intervalHeartTotalMilliseconds: 1, heartTimeOutCount: 1)
            {
            }

            protected override AcceptedTcpServerClient CreateTcpServerClient(Socket client)
            {
                var tcpServerClient = new AcceptedTcpServerClient(client);
                FirstAcceptedClient ??= tcpServerClient;
                return tcpServerClient;
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
                if (Interlocked.Increment(ref _CreateSessionTokenCallCount) == 1)
                {
                    throw new InvalidOperationException("create session token failed");
                }

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
                _AcceptSignal.TrySetResult(true);
            }

            protected override void OnServerClientErrorCallBackEvent(ITcpServerClient client, Exception ex)
            {
            }

            protected override void OnServerErrorEvent(Exception ex)
            {
                ServerErrorCount++;
                LastServerError = ex;
                _ServerErrorSignal.TrySetResult(true);
            }

            public Task WaitForServerErrorAsync()
            {
                return _ServerErrorSignal.Task;
            }

            public Task WaitForAcceptAsync()
            {
                return _AcceptSignal.Task;
            }
        }

        [TestMethod]
        public void OnServerClientHeartEvent_WithTimedOutSession_ShouldCloseClient()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter());
            var client = new TestTcpServerClient();
            var currentTotalMilliseconds = DateTimeHelper.GetTotalMillisecondsFromInstantiation(DateTime.Now);
            while (currentTotalMilliseconds <= 10)
            {
                Thread.SpinWait(1000);
                currentTotalMilliseconds = DateTimeHelper.GetTotalMillisecondsFromInstantiation(DateTime.Now);
            }
            var sessionToken = new TestSessionToken("127.0.0.1", 9527)
            {
                LastReceiveDateTimeTotalMillisecondsFromInstantiation = currentTotalMilliseconds - 10,
            };

            client.CurrentSessionToken = sessionToken;
            server.TrackClient(client);

            server.TriggerHeart(client);

            Assert.AreEqual(1, server.ErrorCallbackCount);
            Assert.AreEqual("心跳超时断开连接", server.LastError.Message);
            Assert.AreEqual(0, server.HeartCallbackCount);
            Assert.AreEqual(1, client.CloseAsyncCallCount);
            Assert.IsFalse(server.HasTrackedClient(sessionToken.SessionID));
        }

        [TestMethod]
        public void OnServerClientHeartEvent_WhenHeartSendThrows_ShouldCloseClientWithoutInvokingHeartCallback()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter())
            {
                CanSendDataResult = true,
            };
            var client = new TestTcpServerClient
            {
                CurrentSessionToken = new TestSessionToken("127.0.0.1", 9527)
                {
                    LastReceiveDateTimeTotalMillisecondsFromInstantiation = DateTimeHelper.GetTotalMillisecondsFromInstantiation(DateTime.Now),
                },
                SendException = new InvalidOperationException("heart send failed"),
            };

            server.TrackClient(client);

            server.TriggerHeart(client);

            Assert.AreEqual(1, server.ErrorCallbackCount);
            Assert.AreEqual("heart send failed", server.LastError.Message);
            Assert.AreEqual(0, server.HeartCallbackCount);
            Assert.AreEqual(1, client.CloseAsyncCallCount);
            Assert.IsFalse(server.HasTrackedClient(client.CurrentSessionToken.SessionID));
        }

        [TestMethod]
        public void OnServerClientHeartEvent_WhenHeartCallbackThrows_ShouldReportErrorWithoutClosingClient()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter())
            {
                CanSendDataResult = true,
                HeartCallbackException = new InvalidOperationException("heart callback failed"),
            };
            var client = new TestTcpServerClient
            {
                CurrentSessionToken = new TestSessionToken("127.0.0.1", 9527)
                {
                    LastReceiveDateTimeTotalMillisecondsFromInstantiation = DateTimeHelper.GetTotalMillisecondsFromInstantiation(DateTime.Now),
                },
            };

            server.TrackClient(client);

            server.TriggerHeart(client);

            Assert.AreEqual(1, client.SendCallCount);
            Assert.AreEqual(1, server.HeartCallbackCount);
            Assert.AreEqual(1, server.ErrorCallbackCount);
            Assert.AreEqual("TcpServer heart callback failed.", server.LastError.Message);
            Assert.IsNotNull(server.LastError.InnerException);
            Assert.AreEqual("heart callback failed", server.LastError.InnerException.Message);
            Assert.AreEqual(0, client.CloseAsyncCallCount);
            Assert.IsTrue(server.HasTrackedClient(client.CurrentSessionToken.SessionID));
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
        public async Task BaseTcpServer_CloseAsync_WhenOnServerCloseEventThrows_ShouldStillClearTrackedClients()
        {
            var server = new ThrowingCloseFinalizationTcpServer();
            var client = new TestTcpServerClient
            {
                CurrentSessionToken = new TestSessionToken("127.0.0.1", 9527),
            };

            using var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server.SetAcceptContextForTest(listenSocket, true);
                server.AttachTrackedClientForTest(client);

                await server.CloseAsync();

                Assert.IsFalse(server.IsRunning);
                Assert.IsNull(server.CurrentSocket);
                Assert.AreEqual(1, client.CloseAsyncCallCount);
                Assert.IsFalse(server.HasTrackedClient(client.CurrentSessionToken.SessionID));
                Assert.AreEqual(1, server.ServerErrorCount);
                Assert.AreEqual("TcpServer close finalization failed.", server.LastServerError.Message);
                Assert.IsNotNull(server.LastServerError.InnerException);
                Assert.AreEqual("server close finalization failed", server.LastServerError.InnerException.Message);
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
        public void BaseTcpServer_Close_WhenCloseAsyncThrows_ShouldReportSyncCloseErrorWithoutEscalating()
        {
            var server = new ThrowingSyncCloseTcpServer();

            server.Close();

            Assert.AreEqual(1, server.CloseAsyncCallCount);
            Assert.AreEqual(1, server.ServerErrorCount);
            Assert.AreEqual("TcpServer sync close failed.", server.LastServerError.Message);
            Assert.IsNotNull(server.LastServerError.InnerException);
            Assert.AreEqual("tcp server close failed", server.LastServerError.InnerException.Message);
        }

        [TestMethod]
        public void BaseTcpServer_Dispose_WhenCloseThrows_ShouldStillReleaseTrackedResources()
        {
            var server = new ThrowingSyncCloseTcpServer();
            var client = new ThrowingCloseTcpServerClient
            {
                CurrentSessionToken = new TestSessionToken("127.0.0.1", 9527),
            };
            var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                client.MarkRunningForTest();
                server.SetAcceptContextForTest(listenSocket, true);
                server.AttachTrackedClientForTest(client);

                server.Dispose();

                Assert.AreEqual(1, server.CloseAsyncCallCount);
                Assert.IsTrue(server.IsDisposed);
                Assert.IsFalse(server.IsRunning);
                Assert.IsNull(server.CurrentSocket);
                Assert.IsTrue(listenSocket.SafeHandle.IsClosed);
                Assert.AreEqual(1, client.CloseAsyncCallCount);
                Assert.IsTrue(client.IsDisposed);
                Assert.IsFalse(client.IsRunning);
                Assert.IsFalse(client.HasHeartTimerForTest());
                Assert.IsFalse(client.HasSendQueueForTest());
                Assert.IsTrue(client.CurrentSocket.SafeHandle.IsClosed);
                Assert.IsFalse(server.HasTrackedClient(client.CurrentSessionToken.SessionID));
                Assert.AreEqual(1, server.ServerErrorCount);
                Assert.AreEqual("TcpServer dispose close failed.", server.LastServerError.Message);
                Assert.IsNotNull(server.LastServerError.InnerException);
                Assert.AreEqual("tcp server close failed", server.LastServerError.InnerException.Message);
            }
            finally
            {
                client.Dispose();
                listenSocket.Dispose();
                server.Dispose();
            }
        }

        [TestMethod]
        public async Task BaseTcpServer_CloseAsync_WhenChildCloseThrows_ShouldStillDisposeFailedChild()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter(), port: 0);
            var client = new ThrowingCloseTcpServerClient
            {
                CurrentSessionToken = new TestSessionToken("127.0.0.1", 9527),
            };

            using var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                client.MarkRunningForTest();
                server.SetAcceptContextForTest(listenSocket, true);
                server.AttachTrackedClientForTest(client);

                await server.CloseAsync();

                Assert.AreEqual(2, client.CloseAsyncCallCount);
                Assert.IsTrue(client.IsDisposed);
                Assert.IsFalse(client.IsRunning);
                Assert.IsFalse(client.HasHeartTimerForTest());
                Assert.IsFalse(client.HasSendQueueForTest());
                Assert.IsTrue(client.CurrentSocket.SafeHandle.IsClosed);
                Assert.AreEqual(1, server.ErrorCallbackCount);
                Assert.AreEqual("TcpServer close child client failed.", server.LastError.Message);
                Assert.IsNotNull(server.LastError.InnerException);
                Assert.AreEqual("child close failed", server.LastError.InnerException.Message);
                Assert.IsFalse(server.HasTrackedClient(client.CurrentSessionToken.SessionID));
                Assert.IsFalse(server.IsRunning);
                Assert.IsNull(server.CurrentSocket);
            }
            finally
            {
                server.Dispose();
            }
        }

        [TestMethod]
        public void BaseTcpServer_OnServerClientCloseEvent_WhenCloseCallbackThrows_ShouldStillCloseClient()
        {
            var server = new ThrowingCloseCallbackTcpServer();
            var client = new TestTcpServerClient
            {
                CurrentSessionToken = new TestSessionToken("127.0.0.1", 9527),
            };

            try
            {
                server.AttachTrackedClientForTest(client);

                server.TriggerServerClientCloseForTest(client);

                Assert.AreEqual(1, server.ErrorCallbackCount);
                Assert.AreEqual("TcpServer close callback failed.", server.LastError.Message);
                Assert.IsNotNull(server.LastError.InnerException);
                Assert.AreEqual("close callback failed", server.LastError.InnerException.Message);
                Assert.AreEqual(1, client.CloseAsyncCallCount);
                Assert.IsFalse(server.HasTrackedClient(client.CurrentSessionToken.SessionID));
                Assert.AreEqual(0, server.ServerErrorCount);
            }
            finally
            {
                client.Dispose();
                server.Dispose();
            }
        }

        [TestMethod]
        public void BaseTcpServer_OnServerClientStartReceiveEvent_WhenCallbackThrows_ShouldReportErrorWithoutClosingClient()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter(), port: 0)
            {
                StartReceiveCallbackException = new InvalidOperationException("start receive callback failed"),
            };
            var client = new TestTcpServerClient
            {
                CurrentSessionToken = new TestSessionToken("127.0.0.1", 9527),
            };

            server.TrackClient(client);

            server.TriggerServerClientStartReceiveForTest(client);

            Assert.AreEqual(1, server.StartReceiveCallbackCount);
            Assert.AreEqual(1, server.ErrorCallbackCount);
            Assert.AreEqual("TcpServer start receive callback failed.", server.LastError.Message);
            Assert.IsNotNull(server.LastError.InnerException);
            Assert.AreEqual("start receive callback failed", server.LastError.InnerException.Message);
            Assert.AreEqual(0, client.CloseAsyncCallCount);
            Assert.IsTrue(server.HasTrackedClient(client.CurrentSessionToken.SessionID));
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
        public void BaseTcpServer_OnServerClientReceiveDataEvent_WhenCallbackThrows_ShouldReportErrorAndContinuePackageLoop()
        {
            var server = new TestTcpServer(new SinglePackageFixedHeaderPackageFilter(), port: 0)
            {
                ReceiveDataCallbackException = new InvalidOperationException("receive callback failed"),
            };
            var client = new TestTcpServerClient
            {
                CurrentSessionToken = new TestSessionToken("127.0.0.1", 9527),
            };

            server.TrackClient(client);

            server.TriggerServerClientReceiveDataForTest(client, new BufferModel(), new CacheModel());

            Assert.AreEqual(1, server.ReceiveDataCallbackCount);
            Assert.AreEqual(1, server.ReceivePackageCallbackCount);
            Assert.AreEqual(1, server.ErrorCallbackCount);
            Assert.AreEqual("TcpServer receive data callback failed.", server.LastError.Message);
            Assert.IsNotNull(server.LastError.InnerException);
            Assert.AreEqual("receive callback failed", server.LastError.InnerException.Message);
            Assert.AreEqual(0, client.CloseAsyncCallCount);
            Assert.IsTrue(server.HasTrackedClient(client.CurrentSessionToken.SessionID));
        }

        [TestMethod]
        public void BaseTcpServer_OnServerReceivePackage_WhenCallbackThrows_ShouldReportErrorWithoutClosingClient()
        {
            var server = new TestTcpServer(new SinglePackageFixedHeaderPackageFilter(), port: 0)
            {
                ReceivePackageException = new InvalidOperationException("receive package callback failed"),
            };
            var client = new TestTcpServerClient
            {
                CurrentSessionToken = new TestSessionToken("127.0.0.1", 9527),
            };

            server.TrackClient(client);

            server.TriggerReceiveLoop(client, new BufferModel(), new CacheModel());

            Assert.AreEqual(1, server.ReceivePackageCallbackCount);
            Assert.AreEqual(1, server.ErrorCallbackCount);
            Assert.AreEqual("TcpServer receive package callback failed.", server.LastError.Message);
            Assert.IsNotNull(server.LastError.InnerException);
            Assert.AreEqual("receive package callback failed", server.LastError.InnerException.Message);
            Assert.AreEqual(0, client.CloseAsyncCallCount);
            Assert.IsTrue(server.HasTrackedClient(client.CurrentSessionToken.SessionID));
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
        public void SendDataBytes_WhenCanSendDataThrows_ShouldReportErrorWithoutClosingClient()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter())
            {
                CanSendDataException = new InvalidOperationException("can send data failed"),
            };
            var client = new TestTcpServerClient
            {
                CurrentSessionToken = new TestSessionToken("127.0.0.1", 9527),
            };

            server.TrackClient(client);

            server.SendDataBytes(client, new byte[] { 0x01 });

            Assert.AreEqual(0, client.SendCallCount);
            Assert.AreEqual(1, server.ErrorCallbackCount);
            Assert.AreEqual("TcpServer can send data check failed.", server.LastError.Message);
            Assert.IsNotNull(server.LastError.InnerException);
            Assert.AreEqual("can send data failed", server.LastError.InnerException.Message);
            Assert.AreEqual(0, client.CloseAsyncCallCount);
            Assert.IsTrue(server.HasTrackedClient(client.CurrentSessionToken.SessionID));
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
        public void SendDataBytes_WhenChildCloseThrows_ShouldReportCloseFailureWithoutEscalatingToCaller()
        {
            var server = new TestTcpServer(new TestFixedHeaderPackageFilter())
            {
                CanSendDataResult = true,
            };
            var client = new ThrowingCloseTcpServerClient
            {
                CurrentSessionToken = new TestSessionToken("127.0.0.1", 9527),
                SendException = new InvalidOperationException("send failed"),
            };

            try
            {
                client.MarkRunningForTest();
                server.TrackClient(client);

                server.SendDataBytes(client, new byte[] { 0x01 });

                Assert.AreEqual(1, client.SendCallCount);
                Assert.AreEqual(2, client.CloseAsyncCallCount);
                Assert.AreEqual(2, server.ErrorCallbackCount);
                Assert.AreEqual("TcpServer close child client failed.", server.LastError.Message);
                Assert.IsNotNull(server.LastError.InnerException);
                Assert.AreEqual("child close failed", server.LastError.InnerException.Message);
                Assert.IsTrue(client.IsDisposed);
                Assert.IsFalse(client.IsRunning);
                Assert.IsFalse(client.HasHeartTimerForTest());
                Assert.IsFalse(client.HasSendQueueForTest());
                Assert.IsTrue(client.CurrentSocket.SafeHandle.IsClosed);
                Assert.IsFalse(server.HasTrackedClient(client.CurrentSessionToken.SessionID));
            }
            finally
            {
                client.Dispose();
                server.Dispose();
            }
        }

        [TestMethod]
        public void SendDataBytes_WhenErrorCallbackThrows_ShouldStillCloseClientAndReportServerError()
        {
            var server = new ThrowingErrorCallbackTcpServer();
            var client = new TestTcpServerClient
            {
                CurrentSessionToken = new TestSessionToken("127.0.0.1", 9527),
                SendException = new InvalidOperationException("send failed"),
            };

            try
            {
                server.TrackClient(client);

                server.SendDataBytes(client, new byte[] { 0x01 });

                Assert.AreEqual(1, client.SendCallCount);
                Assert.AreEqual(1, client.CloseAsyncCallCount);
                Assert.IsFalse(server.HasTrackedClient(client.CurrentSessionToken.SessionID));
                Assert.AreEqual(1, server.ServerErrorCount);
                Assert.AreEqual("TcpServer report child client error failed.", server.LastServerError.Message);
                Assert.IsNotNull(server.LastServerError.InnerException);
                Assert.AreEqual("error callback failed", server.LastServerError.InnerException.Message);
            }
            finally
            {
                client.Dispose();
                server.Dispose();
            }
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

        [TestMethod]
        public async Task BeginAcceptAsync_WhenCreateTcpServerClientThrows_ShouldReportErrorWithoutStoppingServer()
        {
            var server = new ThrowingCreateClientAcceptServer();

            try
            {
                server.Start();
                var port = ((IPEndPoint)server.CurrentSocket.LocalEndPoint).Port;

                using var firstClient = new TcpClient();
                await firstClient.ConnectAsync(IPAddress.Loopback, port);

                var errorTask = server.WaitForServerErrorAsync();
                var errorCompletedTask = await Task.WhenAny(errorTask, Task.Delay(TimeSpan.FromSeconds(3)));

                Assert.AreSame(errorTask, errorCompletedTask);
                await errorTask;

                Assert.IsTrue(server.IsRunning);
                Assert.IsNotNull(server.CurrentSocket);
                Assert.AreEqual(1, server.ServerErrorCount);
                Assert.AreEqual("TcpServer create accepted client failed.", server.LastServerError.Message);
                Assert.IsNotNull(server.LastServerError.InnerException);
                Assert.AreEqual("create client failed", server.LastServerError.InnerException.Message);

                using var secondClient = new TcpClient();
                await secondClient.ConnectAsync(IPAddress.Loopback, port);

                var acceptTask = server.WaitForAcceptAsync();
                var acceptCompletedTask = await Task.WhenAny(acceptTask, Task.Delay(TimeSpan.FromSeconds(3)));

                Assert.AreSame(acceptTask, acceptCompletedTask);
                await acceptTask;

                Assert.AreEqual(1, server.AcceptCallbackCount);
                Assert.IsTrue(server.IsRunning);
            }
            finally
            {
                await server.CloseAsync();
            }
        }

        [TestMethod]
        public async Task BeginAcceptAsync_WhenCreateSessionTokenThrows_ShouldDisposeAcceptedClientAndKeepServerRunning()
        {
            var server = new ThrowingSessionTokenAcceptServer();

            try
            {
                server.Start();
                var port = ((IPEndPoint)server.CurrentSocket.LocalEndPoint).Port;

                using var firstClient = new TcpClient();
                await firstClient.ConnectAsync(IPAddress.Loopback, port);

                var errorTask = server.WaitForServerErrorAsync();
                var errorCompletedTask = await Task.WhenAny(errorTask, Task.Delay(TimeSpan.FromSeconds(3)));

                Assert.AreSame(errorTask, errorCompletedTask);
                await errorTask;

                var failedClient = server.FirstAcceptedClient;
                Assert.IsNotNull(failedClient);

                var waitDeadline = DateTime.UtcNow.AddSeconds(3);
                while (!failedClient.IsDisposed && DateTime.UtcNow < waitDeadline)
                {
                    await Task.Delay(10);
                }

                Assert.IsTrue(server.IsRunning);
                Assert.IsNotNull(server.CurrentSocket);
                Assert.AreEqual(1, server.ServerErrorCount);
                Assert.AreEqual("TcpServer initialize accepted client failed.", server.LastServerError.Message);
                Assert.IsNotNull(server.LastServerError.InnerException);
                Assert.AreEqual("create session token failed", server.LastServerError.InnerException.Message);
                Assert.IsTrue(failedClient.IsDisposed);
                Assert.IsFalse(failedClient.HasHeartTimerForTest());
                Assert.IsFalse(failedClient.HasSendQueueForTest());
                Assert.IsTrue(failedClient.CurrentSocket.SafeHandle.IsClosed);

                using var secondClient = new TcpClient();
                await secondClient.ConnectAsync(IPAddress.Loopback, port);

                var acceptTask = server.WaitForAcceptAsync();
                var acceptCompletedTask = await Task.WhenAny(acceptTask, Task.Delay(TimeSpan.FromSeconds(3)));

                Assert.AreSame(acceptTask, acceptCompletedTask);
                await acceptTask;

                Assert.AreEqual(1, server.AcceptCallbackCount);
                Assert.IsTrue(server.IsRunning);
            }
            finally
            {
                await server.CloseAsync();
            }
        }

        [TestMethod]
        public async Task BeginAcceptAsync_WhenStartReceiveReturnsNotRunning_ShouldDisposeAcceptedClientAndKeepServerRunning()
        {
            var server = new IncompleteStartReceiveAcceptServer();

            try
            {
                server.Start();
                var port = ((IPEndPoint)server.CurrentSocket.LocalEndPoint).Port;

                using var firstClient = new TcpClient();
                await firstClient.ConnectAsync(IPAddress.Loopback, port);

                var waitDeadline = DateTime.UtcNow.AddSeconds(3);
                while ((server.FirstAcceptedClient == null || !server.FirstAcceptedClient.IsDisposed || server.TrackedClientCount != 0) && DateTime.UtcNow < waitDeadline)
                {
                    await Task.Delay(10);
                }

                var failedClient = server.FirstAcceptedClient;
                Assert.IsNotNull(failedClient);
                Assert.IsTrue(failedClient.IsDisposed);
                Assert.IsFalse(failedClient.IsRunning);
                Assert.IsFalse(failedClient.HasHeartTimerForTest());
                Assert.IsTrue(failedClient.CurrentSocket.SafeHandle.IsClosed);
                Assert.AreEqual(0, server.TrackedClientCount);
                Assert.AreEqual(0, server.AcceptCallbackCount);
                Assert.AreEqual(0, server.ServerErrorCount);
                Assert.IsTrue(server.IsRunning);
                Assert.IsNotNull(server.CurrentSocket);

                using var secondClient = new TcpClient();
                await secondClient.ConnectAsync(IPAddress.Loopback, port);

                var acceptTask = server.WaitForAcceptAsync();
                var acceptCompletedTask = await Task.WhenAny(acceptTask, Task.Delay(TimeSpan.FromSeconds(3)));

                Assert.AreSame(acceptTask, acceptCompletedTask);
                await acceptTask;

                Assert.AreEqual(1, server.AcceptCallbackCount);
                Assert.IsTrue(server.IsRunning);
            }
            finally
            {
                await server.CloseAsync();
            }
        }
    }
}
