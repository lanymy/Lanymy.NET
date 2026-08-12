using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class SocketLifecycleTests
    {
        private sealed class TestSessionToken : BaseSessionToken
        {
            public override byte[] CacheHeartBytes => null;

            public TestSessionToken(string ip, int port)
                : base(ip, port)
            {
            }
        }

        private sealed class TestFixedHeaderPackageFilter : BaseFixedHeaderPackageFilter<object, object, TestSessionToken>
        {
            private readonly bool _throwOnEncode;

            public TestFixedHeaderPackageFilter(bool throwOnEncode = false)
                : base(4)
            {
                _throwOnEncode = throwOnEncode;
            }

            public TestFixedHeaderPackageFilter()
                : this(false)
            {
            }

            public override int GetBodyLengthFromHeader(int cursorIndex, byte[] bufferBytes)
            {
                return 0;
            }

            public override bool CheckPackage(byte[] packageBytes)
            {
                return true;
            }

            public override byte[] EncodePackage(object sendPackage)
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
                return null;
            }
        }

        private sealed class ThrowingSendWorkTaskQueue : WorkTaskQueue<byte[]>
        {
            private readonly Exception _exception;

            public ThrowingSendWorkTaskQueue(Exception exception)
                : base(_ => { }, null, taskSleepMilliseconds: 1)
            {
                _exception = exception;
            }

            public override Task AddToQueueAsync(byte[] data)
            {
                throw _exception;
            }
        }

        private sealed class ThrowingStopSendWorkTaskQueue : WorkTaskQueue<byte[]>
        {
            private readonly Exception _stopException;

            public ThrowingStopSendWorkTaskQueue(Exception stopException)
                : base(_ => { }, null, taskSleepMilliseconds: 1)
            {
                _stopException = stopException;
            }

            public bool DisposeCalled { get; private set; }

            public void MarkRunningForTest()
            {
                IsRunning = true;
            }

            protected override Task OnStopAsync()
            {
                throw _stopException;
            }

            protected override async Task OnDisposeAsync()
            {
                DisposeCalled = true;
                await base.OnDisposeAsync();
            }
        }

        private sealed class ThrowingStartSendWorkTaskQueue : WorkTaskQueue<byte[]>
        {
            private readonly Exception _startException;

            public ThrowingStartSendWorkTaskQueue(Exception startException)
                : base(_ => { }, null, taskSleepMilliseconds: 1)
            {
                _startException = startException;
            }

            protected override Task OnStartAsync()
            {
                throw _startException;
            }
        }

        private sealed class ThrowingStopTimerWorkTask : TimerWorkTask
        {
            private readonly Exception _stopException;

            public ThrowingStopTimerWorkTask(Exception stopException)
                : base(() => null, taskSleepMilliseconds: 1)
            {
                _stopException = stopException;
            }

            public bool DisposeCalled { get; private set; }

            public void MarkRunningForTest()
            {
                IsRunning = true;
            }

            protected override Task OnStopAsync()
            {
                throw _stopException;
            }

            protected override async Task OnDisposeAsync()
            {
                DisposeCalled = true;
                await Task.CompletedTask;
            }
        }

        private sealed class TestTcpClient : BaseTcpClient<object, object, TestSessionToken, TestFixedHeaderPackageFilter>
        {
            private readonly byte[] _startupPayload;

            public Exception LastError { get; private set; }

            public TestTcpClient(string serverIP, int port, byte[] startupPayload)
                : base(new TestFixedHeaderPackageFilter(), serverIP, port, sendDataIntervalMilliseconds: 1, receiveBufferSize: 16, sendBufferSize: 16)
            {
                _startupPayload = startupPayload;
            }

            protected override void OnConnectionEvent()
            {
                Send(_startupPayload);
            }

            protected override void OnCloseEvent()
            {
            }

            protected override void OnReceivePackageEvent(object package)
            {
            }

            protected override void OnErrorEvent(Exception ex)
            {
                LastError = ex;
            }
        }

        private sealed class RetryableStartTcpClient : BaseTcpClient<object, object, TestSessionToken, TestFixedHeaderPackageFilter>
        {
            private readonly byte[] _startupPayload;

            public Exception LastError { get; private set; }
            public int ErrorCount { get; private set; }

            public RetryableStartTcpClient(string serverIP, int port, byte[] startupPayload)
                : base(new TestFixedHeaderPackageFilter(), serverIP, port, sendDataIntervalMilliseconds: 1, receiveBufferSize: 16, sendBufferSize: 16)
            {
                _startupPayload = startupPayload;
            }

            protected override void OnConnectionEvent()
            {
                Send(_startupPayload);
            }

            protected override void OnCloseEvent()
            {
            }

            protected override void OnReceivePackageEvent(object package)
            {
            }

            protected override void OnErrorEvent(Exception ex)
            {
                ErrorCount++;
                LastError = ex;
            }
        }

        private sealed class RetryableStartQueueTcpClient : BaseTcpClient<object, object, TestSessionToken, TestFixedHeaderPackageFilter>
        {
            private readonly byte[] _startupPayload;
            private int _CreateSendQueueCallCount;

            public Exception LastError { get; private set; }
            public int ErrorCount { get; private set; }

            public RetryableStartQueueTcpClient(string serverIP, int port, byte[] startupPayload)
                : base(new TestFixedHeaderPackageFilter(), serverIP, port, sendDataIntervalMilliseconds: 1, receiveBufferSize: 16, sendBufferSize: 16)
            {
                _startupPayload = startupPayload;
            }

            protected override WorkTaskQueue<byte[]> CreateSendWorkTaskQueue()
            {
                if (Interlocked.Increment(ref _CreateSendQueueCallCount) == 1)
                {
                    return new ThrowingStartSendWorkTaskQueue(new InvalidOperationException("send queue start failed"));
                }

                return base.CreateSendWorkTaskQueue();
            }

            protected override void OnConnectionEvent()
            {
                Send(_startupPayload);
            }

            protected override void OnCloseEvent()
            {
            }

            protected override void OnReceivePackageEvent(object package)
            {
            }

            protected override void OnErrorEvent(Exception ex)
            {
                ErrorCount++;
                LastError = ex;
            }
        }

        private sealed class ThrowingConnectionTcpClient : BaseTcpClient<object, object, TestSessionToken, TestFixedHeaderPackageFilter>
        {
            public int CloseAsyncCallCount { get; private set; }
            public Exception LastError { get; private set; }

            public ThrowingConnectionTcpClient()
                : base(new TestFixedHeaderPackageFilter(), IPAddress.Loopback.ToString(), 9527, sendDataIntervalMilliseconds: 1, receiveBufferSize: 16, sendBufferSize: 16)
            {
            }

            protected override void OnConnectionEvent()
            {
                throw new InvalidOperationException("connection callback failed");
            }

            protected override void OnCloseEvent()
            {
            }

            protected override void OnReceivePackageEvent(object package)
            {
            }

            protected override void OnErrorEvent(Exception ex)
            {
                LastError = ex;
            }

            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                return Task.CompletedTask;
            }

            public void TriggerConnection()
            {
                var method = typeof(BaseTcpClient<object, object, TestSessionToken, TestFixedHeaderPackageFilter>).GetMethod("OnConnection", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(method);
                method.Invoke(this, null);
            }
        }

        private sealed class ThrowingReceivePackageTcpClient : BaseTcpClient<object, object, TestSessionToken, TestFixedHeaderPackageFilter>
        {
            public int CloseAsyncCallCount { get; private set; }
            public Exception LastError { get; private set; }

            public ThrowingReceivePackageTcpClient()
                : base(new TestFixedHeaderPackageFilter(), IPAddress.Loopback.ToString(), 9527, sendDataIntervalMilliseconds: 1, receiveBufferSize: 16, sendBufferSize: 16)
            {
            }

            protected override void OnConnectionEvent()
            {
            }

            protected override void OnCloseEvent()
            {
            }

            protected override void OnReceivePackageEvent(object package)
            {
                throw new InvalidOperationException("receive package callback failed");
            }

            protected override void OnErrorEvent(Exception ex)
            {
                LastError = ex;
            }

            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                return Task.CompletedTask;
            }

            public void TriggerReceivePackage()
            {
                var method = typeof(BaseTcpClient<object, object, TestSessionToken, TestFixedHeaderPackageFilter>).GetMethod("OnReceivePackage", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(method);
                method.Invoke(this, new object[] { new object() });
            }
        }

        private sealed class ThrowingSendTcpClient : BaseTcpClient<object, object, TestSessionToken, TestFixedHeaderPackageFilter>
        {
            public int CloseAsyncCallCount { get; private set; }
            public Exception LastError { get; private set; }
            public List<Exception> Errors { get; } = new List<Exception>();
            public bool ThrowOnCloseAsync { get; set; }

            public ThrowingSendTcpClient()
                : base(new TestFixedHeaderPackageFilter(), IPAddress.Loopback.ToString(), 9527, sendDataIntervalMilliseconds: 1, receiveBufferSize: 16, sendBufferSize: 16)
            {
            }

            protected override void OnConnectionEvent()
            {
            }

            protected override void OnCloseEvent()
            {
            }

            protected override void OnReceivePackageEvent(object package)
            {
            }

            protected override void OnErrorEvent(Exception ex)
            {
                Errors.Add(ex);
                LastError = ex;
            }

            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                if (ThrowOnCloseAsync)
                {
                    _IsRunning = false;
                    throw new InvalidOperationException("tcp client close failed");
                }

                return Task.CompletedTask;
            }

            public void ConfigureSendQueueFailure(Exception exception)
            {
                _IsRunning = true;
                _CurrentSendWorkTaskQueue = new ThrowingSendWorkTaskQueue(exception);
            }
        }

        private sealed class ThrowingSyncSendTcpClient : BaseTcpClient<object, object, TestSessionToken, TestFixedHeaderPackageFilter>
        {
            public int CloseAsyncCallCount { get; private set; }
            public Exception LastError { get; private set; }

            public ThrowingSyncSendTcpClient()
                : base(new TestFixedHeaderPackageFilter(), IPAddress.Loopback.ToString(), 9527, sendDataIntervalMilliseconds: 1, receiveBufferSize: 16, sendBufferSize: 16)
            {
            }

            protected override void OnConnectionEvent()
            {
            }

            protected override void OnCloseEvent()
            {
            }

            protected override void OnReceivePackageEvent(object package)
            {
            }

            protected override void OnErrorEvent(Exception ex)
            {
                LastError = ex;
            }

            public override Task SendAsync(byte[] sendDataBytes)
            {
                throw new InvalidOperationException("sync send failed");
            }

            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                return Task.CompletedTask;
            }
        }

        private sealed class ThrowingEncodePackageTcpClient : BaseTcpClient<object, object, TestSessionToken, TestFixedHeaderPackageFilter>
        {
            public int CloseAsyncCallCount { get; private set; }
            public Exception LastError { get; private set; }

            public ThrowingEncodePackageTcpClient()
                : base(new TestFixedHeaderPackageFilter(throwOnEncode: true), IPAddress.Loopback.ToString(), 9527, sendDataIntervalMilliseconds: 1, receiveBufferSize: 16, sendBufferSize: 16)
            {
            }

            protected override void OnConnectionEvent()
            {
            }

            protected override void OnCloseEvent()
            {
            }

            protected override void OnReceivePackageEvent(object package)
            {
            }

            protected override void OnErrorEvent(Exception ex)
            {
                LastError = ex;
            }

            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                return Task.CompletedTask;
            }
        }

        private sealed class ReceiveRaceTcpClient : BaseTcpClient<object, object, TestSessionToken, TestFixedHeaderPackageFilter>
        {
            public int CloseAsyncCallCount { get; private set; }
            public Exception LastError { get; private set; }

            public ReceiveRaceTcpClient()
                : base(new TestFixedHeaderPackageFilter(), IPAddress.Loopback.ToString(), 9527, sendDataIntervalMilliseconds: 1, receiveBufferSize: 16, sendBufferSize: 16)
            {
            }

            protected override void OnConnectionEvent()
            {
            }

            protected override void OnCloseEvent()
            {
            }

            protected override void OnReceivePackageEvent(object package)
            {
            }

            protected override void OnErrorEvent(Exception ex)
            {
                LastError = ex;
            }

            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                return Task.CompletedTask;
            }

            public void PrepareReceiveStateForTest(bool isRunning, NetworkStream currentNetworkStream)
            {
                _IsRunning = isRunning;
                _CurrentNetworkStream = currentNetworkStream;
            }

            public void TriggerReceiveForTest(IAsyncResult asyncResult)
            {
                var method = typeof(BaseTcpClient<object, object, TestSessionToken, TestFixedHeaderPackageFilter>).GetMethod("OnReceive", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(method);
                method.Invoke(this, new object[] { asyncResult });
            }

            public bool CanContinueReceiveForTest(NetworkStream currentNetworkStream)
            {
                return CanContinueReceive(currentNetworkStream);
            }

            public bool CanIgnoreReceiveExceptionForTest(Exception exception, NetworkStream currentNetworkStream)
            {
                return CanIgnoreReceiveException(exception, currentNetworkStream);
            }
        }

        private sealed class CloseFinalizationTcpClient : BaseTcpClient<object, object, TestSessionToken, TestFixedHeaderPackageFilter>
        {
            public Exception LastError { get; private set; }
            public Exception OnCloseEventException { get; set; }
            public List<Exception> Errors { get; } = new List<Exception>();

            public CloseFinalizationTcpClient()
                : base(new TestFixedHeaderPackageFilter(), IPAddress.Loopback.ToString(), 9527, sendDataIntervalMilliseconds: 1, receiveBufferSize: 16, sendBufferSize: 16)
            {
            }

            protected override void OnConnectionEvent()
            {
            }

            protected override void OnCloseEvent()
            {
                if (OnCloseEventException != null)
                {
                    throw OnCloseEventException;
                }
            }

            protected override void OnReceivePackageEvent(object package)
            {
            }

            protected override void OnErrorEvent(Exception ex)
            {
                Errors.Add(ex);
                LastError = ex;
            }

            public void PrepareCloseStateForTest()
            {
                _IsRunning = true;
            }

            public bool HasSendQueueForTest()
            {
                return _CurrentSendWorkTaskQueue != null;
            }

            public void ReplaceSendQueueForTest(WorkTaskQueue<byte[]> sendQueue)
            {
                _CurrentSendWorkTaskQueue = sendQueue;
            }
        }

        private sealed class ThrowingCloseDisposeTcpClient : BaseTcpClient<object, object, TestSessionToken, TestFixedHeaderPackageFilter>
        {
            public int CloseAsyncCallCount { get; private set; }
            public Exception LastError { get; private set; }
            public List<Exception> Errors { get; } = new List<Exception>();

            public ThrowingCloseDisposeTcpClient()
                : base(new TestFixedHeaderPackageFilter(), IPAddress.Loopback.ToString(), 9527, sendDataIntervalMilliseconds: 1, receiveBufferSize: 16, sendBufferSize: 16)
            {
            }

            protected override void OnConnectionEvent()
            {
            }

            protected override void OnCloseEvent()
            {
            }

            protected override void OnReceivePackageEvent(object package)
            {
            }

            protected override void OnErrorEvent(Exception ex)
            {
                Errors.Add(ex);
                LastError = ex;
            }

            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                _IsRunning = false;
                throw new InvalidOperationException("tcp client close failed");
            }

            public void PrepareRunningDisposeStateForTest()
            {
                _IsRunning = true;
            }

            public bool HasSendQueueForTest()
            {
                return _CurrentSendWorkTaskQueue != null;
            }
        }

        private sealed class TestTcpServerClient : BaseTcpServerClient
        {
            private readonly byte[] _startupPayload;

            public TestTcpServerClient(Socket socket, byte[] startupPayload)
                : base(socket, receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, heartIntervalMilliseconds: 1000)
            {
                _startupPayload = startupPayload;
            }

            protected override void OnStartReceiveEvent()
            {
                Send(_startupPayload);
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

            public Task StartReceiveForTestAsync()
            {
                var method = typeof(BaseTcpServerClient).GetMethod("StartReceiveAsync", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(method);
                return (Task)method.Invoke(this, null);
            }
        }

        private sealed class ThrowingStartReceiveTcpServerClient : BaseTcpServerClient
        {
            public int CloseAsyncCallCount { get; private set; }
            public Exception LastError { get; private set; }

            public ThrowingStartReceiveTcpServerClient()
                : base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, heartIntervalMilliseconds: 1000)
            {
            }

            protected override void OnStartReceiveEvent()
            {
                throw new InvalidOperationException("start receive callback failed");
            }

            protected override void OnCloseEvent()
            {
            }

            protected override void OnServerClientErrorEvent(Exception ex)
            {
                LastError = ex;
            }

            protected override void OnReceiveDataEvent(BufferModel buffer, CacheModel cache)
            {
            }

            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                return Task.CompletedTask;
            }

            public void TriggerStartReceive()
            {
                var method = typeof(BaseTcpServerClient).GetMethod("OnStartReceive", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(method);
                method.Invoke(this, null);
            }
        }

        private sealed class InvalidSocketStartReceiveTcpServerClient : BaseTcpServerClient
        {
            public int CloseAsyncCallCount { get; private set; }
            public Exception LastError { get; private set; }

            public InvalidSocketStartReceiveTcpServerClient()
                : base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, heartIntervalMilliseconds: 1000)
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
                LastError = ex;
            }

            protected override void OnReceiveDataEvent(BufferModel buffer, CacheModel cache)
            {
            }

            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                return Task.CompletedTask;
            }

            public void DisposeCurrentSocketForTest()
            {
                CurrentSocket.Dispose();
            }

            public Task StartReceiveForTestAsync()
            {
                var method = typeof(BaseTcpServerClient).GetMethod("StartReceiveAsync", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(method);
                return (Task)method.Invoke(this, null);
            }
        }

        private sealed class ThrowingReceiveDataTcpServerClient : BaseTcpServerClient
        {
            public int CloseAsyncCallCount { get; private set; }
            public Exception LastError { get; private set; }

            public ThrowingReceiveDataTcpServerClient()
                : base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, heartIntervalMilliseconds: 1000)
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
                LastError = ex;
            }

            protected override void OnReceiveDataEvent(BufferModel buffer, CacheModel cache)
            {
                throw new InvalidOperationException("receive data callback failed");
            }

            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                return Task.CompletedTask;
            }

            public void TriggerReceiveData()
            {
                var method = typeof(BaseTcpServerClient).GetMethod("OnReceiveData", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(method);
                method.Invoke(this, new object[] { new BufferModel(), new CacheModel() });
            }
        }

        private sealed class ThrowingSendTcpServerClient : BaseTcpServerClient
        {
            public int CloseAsyncCallCount { get; private set; }
            public Exception LastError { get; private set; }
            public List<Exception> Errors { get; } = new List<Exception>();
            public bool ThrowOnCloseAsync { get; set; }

            public ThrowingSendTcpServerClient()
                : base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, heartIntervalMilliseconds: 1000)
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
                Errors.Add(ex);
                LastError = ex;
            }

            protected override void OnReceiveDataEvent(BufferModel buffer, CacheModel cache)
            {
            }

            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                if (ThrowOnCloseAsync)
                {
                    _IsRunning = false;
                    throw new InvalidOperationException("tcp server client close failed");
                }

                return Task.CompletedTask;
            }

            public void ConfigureSendQueueFailure(Exception exception)
            {
                _IsRunning = true;
                _CurrentSendWorkTaskQueue = new ThrowingSendWorkTaskQueue(exception);
            }
        }

        private sealed class ThrowingSyncSendTcpServerClient : BaseTcpServerClient
        {
            public int CloseAsyncCallCount { get; private set; }
            public Exception LastError { get; private set; }

            public ThrowingSyncSendTcpServerClient()
                : base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, heartIntervalMilliseconds: 1000)
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
                LastError = ex;
            }

            protected override void OnReceiveDataEvent(BufferModel buffer, CacheModel cache)
            {
            }

            public override Task SendAsync(byte[] sendDataBytes)
            {
                throw new InvalidOperationException("sync send failed");
            }

            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                return Task.CompletedTask;
            }
        }

        private sealed class DummyAsyncResult : IAsyncResult
        {
            public object AsyncState => null;
            public WaitHandle AsyncWaitHandle => null;
            public bool CompletedSynchronously => false;
            public bool IsCompleted => true;
        }

        private sealed class ReceiveRaceTcpServerClient : BaseTcpServerClient
        {
            public int CloseAsyncCallCount { get; private set; }
            public Exception LastError { get; private set; }

            public ReceiveRaceTcpServerClient()
                : base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, heartIntervalMilliseconds: 1000)
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
                LastError = ex;
            }

            protected override void OnReceiveDataEvent(BufferModel buffer, CacheModel cache)
            {
            }

            public override Task CloseAsync()
            {
                CloseAsyncCallCount++;
                return Task.CompletedTask;
            }

            public void PrepareReceiveStateForTest(bool isRunning, NetworkStream currentNetworkStream)
            {
                _IsRunning = isRunning;
                _CurrentNetworkStream = currentNetworkStream;
            }

            public void TriggerReceiveForTest(IAsyncResult asyncResult)
            {
                var method = typeof(BaseTcpServerClient).GetMethod("OnReceive", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(method);
                method.Invoke(this, new object[] { asyncResult });
            }
        }

        private sealed class CloseFinalizationTcpServerClient : BaseTcpServerClient
        {
            public Exception LastError { get; private set; }
            public Exception CloseEventException { get; set; }
            public Exception OnCloseEventException { get; set; }
            public int ExternalCloseEventCallCount { get; private set; }
            public List<Exception> Errors { get; } = new List<Exception>();

            public CloseFinalizationTcpServerClient()
                : base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, heartIntervalMilliseconds: 1000)
            {
            }

            protected override void OnStartReceiveEvent()
            {
            }

            protected override void OnCloseEvent()
            {
                if (OnCloseEventException != null)
                {
                    throw OnCloseEventException;
                }
            }

            protected override void OnServerClientErrorEvent(Exception ex)
            {
                Errors.Add(ex);
                LastError = ex;
            }

            protected override void OnReceiveDataEvent(BufferModel buffer, CacheModel cache)
            {
            }

            public void PrepareCloseStateForTest()
            {
                _IsRunning = true;
            }

            public void AttachCloseObserver()
            {
                CloseEvent += OnExternalCloseEvent;
            }

            public bool HasCloseEventHandlerForTest()
            {
                var closeEventField = typeof(BaseTcpServerClient).GetField("CloseEvent", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(closeEventField);
                return closeEventField.GetValue(this) != null;
            }

            public bool HasHeartTimerForTest()
            {
                return _CurrentHeartTimerWorkTask != null;
            }

            public bool HasSendQueueForTest()
            {
                return _CurrentSendWorkTaskQueue != null;
            }

            public void ReplaceHeartTimerForTest(TimerWorkTask heartTimer)
            {
                _CurrentHeartTimerWorkTask = heartTimer;
            }

            public void ReplaceSendQueueForTest(WorkTaskQueue<byte[]> sendQueue)
            {
                _CurrentSendWorkTaskQueue = sendQueue;
            }

            private void OnExternalCloseEvent(ITcpServerClient tcpServerClient)
            {
                ExternalCloseEventCallCount++;

                if (CloseEventException != null)
                {
                    throw CloseEventException;
                }
            }
        }

        [TestMethod]
        public async Task BaseTcpClient_OnConnectionEvent_SendShouldNotBeDropped()
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();

            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            var acceptTask = listener.AcceptSocketAsync();

            var client = new TestTcpClient(IPAddress.Loopback.ToString(), port, new byte[] { 0x2A });
            client.Start();

            using var serverSocket = await acceptTask;
            using var serverStream = new NetworkStream(serverSocket, ownsSocket: false);

            var buffer = new byte[1];
            var readTask = serverStream.ReadAsync(buffer, 0, buffer.Length);
            var completedTask = await Task.WhenAny(readTask, Task.Delay(TimeSpan.FromSeconds(3)));

            Assert.AreSame(readTask, completedTask);
            Assert.AreEqual(1, await readTask);
            CollectionAssert.AreEqual(new byte[] { 0x2A }, buffer);
            Assert.IsNull(client.LastError);

            await client.CloseAsync();
        }

        [TestMethod]
        public async Task BaseTcpServerClient_OnStartReceiveEvent_SendShouldNotBeDropped()
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();

            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            using var remoteClient = new TcpClient();
            var connectTask = remoteClient.ConnectAsync(IPAddress.Loopback, port);
            var acceptTask = listener.AcceptSocketAsync();

            await connectTask;
            using var acceptedSocket = await acceptTask;
            using var remoteStream = remoteClient.GetStream();

            var serverClient = new TestTcpServerClient(acceptedSocket, new byte[] { 0x5A });
            serverClient.CurrentSessionToken = new TestSessionToken("127.0.0.1", port);

            await serverClient.StartReceiveForTestAsync();

            var buffer = new byte[1];
            var readTask = remoteStream.ReadAsync(buffer, 0, buffer.Length);
            var completedTask = await Task.WhenAny(readTask, Task.Delay(TimeSpan.FromSeconds(3)));

            Assert.AreSame(readTask, completedTask);
            Assert.AreEqual(1, await readTask);
            CollectionAssert.AreEqual(new byte[] { 0x5A }, buffer);

            await serverClient.CloseAsync();
        }

        [TestMethod]
        public async Task BaseTcpClient_Start_WhenInitialConnectFails_ShouldRollbackAndAllowRetry()
        {
            int port;
            using (var reservedListener = new TcpListener(IPAddress.Loopback, 0))
            {
                reservedListener.Start();
                port = ((IPEndPoint)reservedListener.LocalEndpoint).Port;
            }

            var client = new RetryableStartTcpClient(IPAddress.Loopback.ToString(), port, new byte[] { 0x6B });

            client.Start();

            Assert.IsFalse(client.IsRunning);
            Assert.IsNotNull(client.LastError);
            Assert.AreEqual(1, client.ErrorCount);

            using var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();

            var acceptTask = listener.AcceptSocketAsync();

            client.Start();

            using var serverSocket = await acceptTask;
            using var serverStream = new NetworkStream(serverSocket, ownsSocket: false);

            var buffer = new byte[1];
            var readTask = serverStream.ReadAsync(buffer, 0, buffer.Length);
            var completedTask = await Task.WhenAny(readTask, Task.Delay(TimeSpan.FromSeconds(3)));

            Assert.AreSame(readTask, completedTask);
            Assert.AreEqual(1, await readTask);
            CollectionAssert.AreEqual(new byte[] { 0x6B }, buffer);
            Assert.IsTrue(client.IsRunning);
            Assert.AreEqual(1, client.ErrorCount);

            await client.CloseAsync();
        }

        [TestMethod]
        public async Task BaseTcpClient_Start_WhenSendQueueStartSyncBridgeFails_ShouldRollbackAndAllowRetry()
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;

            var client = new RetryableStartQueueTcpClient(IPAddress.Loopback.ToString(), port, new byte[] { 0x7C });

            var firstAcceptTask = listener.AcceptSocketAsync();
            client.Start();

            using (var firstServerSocket = await firstAcceptTask)
            {
                Assert.IsFalse(client.IsRunning);
                Assert.IsNotNull(client.LastError);
                Assert.AreEqual("send queue start failed", client.LastError.Message);
                Assert.AreEqual(1, client.ErrorCount);
            }

            var secondAcceptTask = listener.AcceptSocketAsync();
            client.Start();

            using var secondServerSocket = await secondAcceptTask;
            using var secondServerStream = new NetworkStream(secondServerSocket, ownsSocket: false);
            var buffer = new byte[1];
            var readTask = secondServerStream.ReadAsync(buffer, 0, buffer.Length);
            var completedTask = await Task.WhenAny(readTask, Task.Delay(TimeSpan.FromSeconds(3)));

            Assert.AreSame(readTask, completedTask);
            Assert.AreEqual(1, await readTask);
            CollectionAssert.AreEqual(new byte[] { 0x7C }, buffer);
            Assert.IsTrue(client.IsRunning);
            Assert.AreEqual(1, client.ErrorCount);

            await client.CloseAsync();
        }

        [TestMethod]
        public void BaseTcpClient_OnConnectionEventThrow_ShouldReportErrorWithoutClosing()
        {
            var client = new ThrowingConnectionTcpClient();

            client.TriggerConnection();

            Assert.IsNotNull(client.LastError);
            Assert.AreEqual("connection callback failed", client.LastError.Message);
            Assert.AreEqual(0, client.CloseAsyncCallCount);
        }

        [TestMethod]
        public void BaseTcpClient_OnReceivePackageEventThrow_ShouldReportErrorWithoutClosing()
        {
            var client = new ThrowingReceivePackageTcpClient();

            client.TriggerReceivePackage();

            Assert.IsNotNull(client.LastError);
            Assert.AreEqual("receive package callback failed", client.LastError.Message);
            Assert.AreEqual(0, client.CloseAsyncCallCount);
        }

        [TestMethod]
        public void BaseTcpClient_OnReceive_WhenNetworkStreamAlreadyCleared_ShouldIgnore()
        {
            var client = new ReceiveRaceTcpClient();

            client.PrepareReceiveStateForTest(isRunning: true, currentNetworkStream: null);
            client.TriggerReceiveForTest(new DummyAsyncResult());

            Assert.IsNull(client.LastError);
            Assert.AreEqual(0, client.CloseAsyncCallCount);
        }

        [TestMethod]
        public void BaseTcpClient_CanContinueReceive_WhenCurrentStreamMissing_ShouldReturnFalse()
        {
            var client = new ReceiveRaceTcpClient();

            client.PrepareReceiveStateForTest(isRunning: true, currentNetworkStream: null);

            Assert.IsFalse(client.CanContinueReceiveForTest(null));
        }

        [TestMethod]
        public void BaseTcpClient_CanIgnoreReceiveException_WhenReceiveAlreadyStoppedAndExceptionIsIgnorable_ShouldReturnTrue()
        {
            var client = new ReceiveRaceTcpClient();

            client.PrepareReceiveStateForTest(isRunning: false, currentNetworkStream: null);

            Assert.IsTrue(client.CanIgnoreReceiveExceptionForTest(new ObjectDisposedException(nameof(NetworkStream)), null));
            Assert.IsTrue(client.CanIgnoreReceiveExceptionForTest(new IOException("stream disposed"), null));
            Assert.IsTrue(client.CanIgnoreReceiveExceptionForTest(new SocketException((int)SocketError.OperationAborted), null));
        }

        [TestMethod]
        public void BaseTcpClient_CanIgnoreReceiveException_WhenExceptionIsNotIgnorable_ShouldReturnFalse()
        {
            var client = new ReceiveRaceTcpClient();

            client.PrepareReceiveStateForTest(isRunning: false, currentNetworkStream: null);

            Assert.IsFalse(client.CanIgnoreReceiveExceptionForTest(new InvalidOperationException("unexpected"), null));
        }

        [TestMethod]
        public void BaseTcpServerClient_OnStartReceiveEventThrow_ShouldReportErrorWithoutClosing()
        {
            var serverClient = new ThrowingStartReceiveTcpServerClient();

            serverClient.TriggerStartReceive();

            Assert.IsNotNull(serverClient.LastError);
            Assert.AreEqual("start receive callback failed", serverClient.LastError.Message);
            Assert.AreEqual(0, serverClient.CloseAsyncCallCount);
        }

        [TestMethod]
        public async Task BaseTcpServerClient_StartReceive_WhenBeginReceiveFails_ShouldNotContinueIntoDisposedResources()
        {
            var serverClient = new InvalidSocketStartReceiveTcpServerClient();
            serverClient.DisposeCurrentSocketForTest();

            await serverClient.StartReceiveForTestAsync();

            Assert.IsNotNull(serverClient.LastError);
            Assert.IsFalse(serverClient.IsRunning);
            Assert.AreEqual(1, serverClient.CloseAsyncCallCount);
        }

        [TestMethod]
        public void BaseTcpClient_Dispose_ShouldSetDisposedStateAndPreventRestart()
        {
            var client = new ThrowingConnectionTcpClient();

            client.Dispose();
            client.Start();

            Assert.IsTrue(client.IsDisposed);
            Assert.IsFalse(client.IsRunning);
            Assert.AreEqual(1, client.CloseAsyncCallCount);
        }

        [TestMethod]
        public async Task BaseTcpServerClient_Dispose_ShouldSetDisposedStateAndPreventRestart()
        {
            var serverClient = new InvalidSocketStartReceiveTcpServerClient();

            serverClient.Dispose();
            await serverClient.StartReceiveForTestAsync();

            Assert.IsTrue(serverClient.IsDisposed);
            Assert.IsFalse(serverClient.IsRunning);
            Assert.AreEqual(1, serverClient.CloseAsyncCallCount);
        }

        [TestMethod]
        public void BaseTcpClient_Dispose_WhenNeverStarted_ShouldReleaseSocketAndSendQueue()
        {
            var client = new CloseFinalizationTcpClient();
            var socket = client.CurrentSocket;

            client.Dispose();

            Assert.IsTrue(client.IsDisposed);
            Assert.IsFalse(client.IsRunning);
            Assert.IsNull(client.CurrentSocket);
            Assert.IsFalse(client.HasSendQueueForTest());
            Assert.IsTrue(socket.SafeHandle.IsClosed);
        }

        [TestMethod]
        public async Task BaseTcpClient_CloseAsync_WhenSendQueueStopThrows_ShouldStillDisposeQueueAndCloseSocket()
        {
            var client = new CloseFinalizationTcpClient();
            var socket = client.CurrentSocket;
            var sendQueue = new ThrowingStopSendWorkTaskQueue(new InvalidOperationException("client queue stop failed"));
            sendQueue.MarkRunningForTest();
            client.ReplaceSendQueueForTest(sendQueue);
            client.PrepareCloseStateForTest();

            await client.CloseAsync();

            Assert.IsTrue(sendQueue.DisposeCalled);
            Assert.IsFalse(client.IsRunning);
            Assert.IsFalse(client.HasSendQueueForTest());
            Assert.IsNull(client.CurrentSocket);
            Assert.IsTrue(socket.SafeHandle.IsClosed);
            CollectionAssert.Contains(client.Errors.ConvertAll(ex => ex.Message), "TcpClient close send queue failed.");
        }

        [TestMethod]
        public void BaseTcpClient_Dispose_WhenCloseThrows_ShouldStillReleaseResources()
        {
            var client = new ThrowingCloseDisposeTcpClient();
            var socket = client.CurrentSocket;

            client.PrepareRunningDisposeStateForTest();
            client.Dispose();

            Assert.AreEqual(1, client.CloseAsyncCallCount);
            Assert.IsTrue(client.IsDisposed);
            Assert.IsFalse(client.IsRunning);
            Assert.IsFalse(client.HasSendQueueForTest());
            Assert.IsNull(client.CurrentSocket);
            Assert.IsTrue(socket.SafeHandle.IsClosed);
            CollectionAssert.Contains(client.Errors.ConvertAll(ex => ex.Message), "TcpClient dispose close failed.");
        }

        [TestMethod]
        public void BaseTcpServerClient_Dispose_WhenNeverStarted_ShouldReleaseSocketAndInternalResources()
        {
            var serverClient = new CloseFinalizationTcpServerClient();
            var socket = serverClient.CurrentSocket;
            serverClient.AttachCloseObserver();

            serverClient.Dispose();

            Assert.IsTrue(serverClient.IsDisposed);
            Assert.IsFalse(serverClient.IsRunning);
            Assert.AreEqual(0, serverClient.ExternalCloseEventCallCount);
            Assert.IsFalse(serverClient.HasHeartTimerForTest());
            Assert.IsFalse(serverClient.HasSendQueueForTest());
            Assert.IsFalse(serverClient.HasCloseEventHandlerForTest());
            Assert.IsTrue(socket.SafeHandle.IsClosed);
        }

        [TestMethod]
        public async Task BaseTcpServerClient_CloseAsync_WhenHeartAndSendStopThrow_ShouldStillDisposeInternalResources()
        {
            var serverClient = new CloseFinalizationTcpServerClient();
            var socket = serverClient.CurrentSocket;
            var heartTimer = new ThrowingStopTimerWorkTask(new InvalidOperationException("heart timer stop failed"));
            var sendQueue = new ThrowingStopSendWorkTaskQueue(new InvalidOperationException("send queue stop failed"));

            heartTimer.MarkRunningForTest();
            sendQueue.MarkRunningForTest();
            serverClient.ReplaceHeartTimerForTest(heartTimer);
            serverClient.ReplaceSendQueueForTest(sendQueue);
            serverClient.PrepareCloseStateForTest();

            await serverClient.CloseAsync();

            Assert.IsTrue(heartTimer.DisposeCalled);
            Assert.IsTrue(sendQueue.DisposeCalled);
            Assert.IsFalse(serverClient.IsRunning);
            Assert.IsFalse(serverClient.HasHeartTimerForTest());
            Assert.IsFalse(serverClient.HasSendQueueForTest());
            Assert.IsTrue(socket.SafeHandle.IsClosed);
            CollectionAssert.Contains(serverClient.Errors.ConvertAll(ex => ex.Message), "TcpServerClient close heart timer failed.");
            CollectionAssert.Contains(serverClient.Errors.ConvertAll(ex => ex.Message), "TcpServerClient close send queue failed.");
        }

        [TestMethod]
        public void BaseTcpServerClient_OnReceiveDataEventThrow_ShouldReportErrorWithoutClosing()
        {
            var serverClient = new ThrowingReceiveDataTcpServerClient();

            serverClient.TriggerReceiveData();

            Assert.IsNotNull(serverClient.LastError);
            Assert.AreEqual("receive data callback failed", serverClient.LastError.Message);
            Assert.AreEqual(0, serverClient.CloseAsyncCallCount);
        }

        [TestMethod]
        public async Task BaseTcpClient_SendAsync_WhenQueueThrows_ShouldReportErrorAndClose()
        {
            var client = new ThrowingSendTcpClient();
            client.ConfigureSendQueueFailure(new InvalidOperationException("send queue failed"));

            await client.SendAsync(new byte[] { 0x2A });

            Assert.IsNotNull(client.LastError);
            Assert.AreEqual("send queue failed", client.LastError.Message);
            Assert.AreEqual(1, client.CloseAsyncCallCount);
        }

        [TestMethod]
        public void BaseTcpClient_Send_WhenSendAsyncThrowsSynchronously_ShouldReportErrorWithoutEscalating()
        {
            var client = new ThrowingSyncSendTcpClient();

            client.Send(new byte[] { 0x2A });

            Assert.IsNotNull(client.LastError);
            Assert.AreEqual("sync send failed", client.LastError.Message);
            Assert.AreEqual(1, client.CloseAsyncCallCount);
        }

        [TestMethod]
        public async Task BaseTcpClient_SendAsync_WhenQueueAndCloseThrow_ShouldReportBothWithoutEscalating()
        {
            var client = new ThrowingSendTcpClient
            {
                ThrowOnCloseAsync = true,
            };
            client.ConfigureSendQueueFailure(new InvalidOperationException("send queue failed"));

            await client.SendAsync(new byte[] { 0x2A });

            Assert.AreEqual(1, client.CloseAsyncCallCount);
            Assert.AreEqual(2, client.Errors.Count);
            Assert.AreEqual("send queue failed", client.Errors[0].Message);
            Assert.AreEqual("TcpClient close after error failed.", client.Errors[1].Message);
            Assert.IsNotNull(client.Errors[1].InnerException);
            Assert.AreEqual("tcp client close failed", client.Errors[1].InnerException.Message);
        }

        [TestMethod]
        public async Task BaseTcpServerClient_SendAsync_WhenQueueThrows_ShouldReportErrorAndClose()
        {
            var serverClient = new ThrowingSendTcpServerClient();
            serverClient.ConfigureSendQueueFailure(new InvalidOperationException("send queue failed"));

            await serverClient.SendAsync(new byte[] { 0x5A });

            Assert.IsNotNull(serverClient.LastError);
            Assert.AreEqual("send queue failed", serverClient.LastError.Message);
            Assert.AreEqual(1, serverClient.CloseAsyncCallCount);
        }

        [TestMethod]
        public void BaseTcpServerClient_Send_WhenSendAsyncThrowsSynchronously_ShouldReportErrorWithoutEscalating()
        {
            var serverClient = new ThrowingSyncSendTcpServerClient();

            serverClient.Send(new byte[] { 0x5A });

            Assert.IsNotNull(serverClient.LastError);
            Assert.AreEqual("sync send failed", serverClient.LastError.Message);
            Assert.AreEqual(1, serverClient.CloseAsyncCallCount);
        }

        [TestMethod]
        public async Task BaseTcpServerClient_SendAsync_WhenQueueAndCloseThrow_ShouldReportBothWithoutEscalating()
        {
            var serverClient = new ThrowingSendTcpServerClient
            {
                ThrowOnCloseAsync = true,
            };
            serverClient.ConfigureSendQueueFailure(new InvalidOperationException("send queue failed"));

            await serverClient.SendAsync(new byte[] { 0x5A });

            Assert.AreEqual(1, serverClient.CloseAsyncCallCount);
            Assert.AreEqual(2, serverClient.Errors.Count);
            Assert.AreEqual("send queue failed", serverClient.Errors[0].Message);
            Assert.AreEqual("TcpServerClient close after error failed.", serverClient.Errors[1].Message);
            Assert.IsNotNull(serverClient.Errors[1].InnerException);
            Assert.AreEqual("tcp server client close failed", serverClient.Errors[1].InnerException.Message);
        }

        [TestMethod]
        public void BaseTcpServerClient_OnReceive_WhenNetworkStreamAlreadyCleared_ShouldIgnore()
        {
            var serverClient = new ReceiveRaceTcpServerClient();

            serverClient.PrepareReceiveStateForTest(isRunning: true, currentNetworkStream: null);
            serverClient.TriggerReceiveForTest(new DummyAsyncResult());

            Assert.IsNull(serverClient.LastError);
            Assert.AreEqual(0, serverClient.CloseAsyncCallCount);
        }

        [TestMethod]
        public async Task BaseTcpServerClient_CloseAsync_WhenOnCloseEventThrows_ShouldStillInvokeCloseEventAndClearHandlers()
        {
            var serverClient = new CloseFinalizationTcpServerClient
            {
                OnCloseEventException = new InvalidOperationException("close callback failed"),
            };
            serverClient.AttachCloseObserver();
            serverClient.PrepareCloseStateForTest();

            await serverClient.CloseAsync();

            Assert.IsNotNull(serverClient.LastError);
            Assert.AreEqual("TcpServerClient close finalization failed.", serverClient.LastError.Message);
            Assert.AreEqual(1, serverClient.ExternalCloseEventCallCount);
            Assert.IsFalse(serverClient.HasCloseEventHandlerForTest());
        }

        [TestMethod]
        public async Task BaseTcpServerClient_CloseAsync_WhenCloseEventThrows_ShouldReportAndClearHandlers()
        {
            var serverClient = new CloseFinalizationTcpServerClient
            {
                CloseEventException = new InvalidOperationException("external close event failed"),
            };
            serverClient.AttachCloseObserver();
            serverClient.PrepareCloseStateForTest();

            await serverClient.CloseAsync();

            Assert.IsNotNull(serverClient.LastError);
            Assert.AreEqual("TcpServerClient close event failed.", serverClient.LastError.Message);
            Assert.AreEqual(1, serverClient.ExternalCloseEventCallCount);
            Assert.IsFalse(serverClient.HasCloseEventHandlerForTest());
        }

        [TestMethod]
        public async Task BaseTcpClient_CloseAsync_WhenOnCloseEventThrows_ShouldReportCloseFinalizationError()
        {
            var client = new CloseFinalizationTcpClient
            {
                OnCloseEventException = new InvalidOperationException("close callback failed"),
            };
            client.PrepareCloseStateForTest();

            await client.CloseAsync();

            Assert.IsNotNull(client.LastError);
            Assert.AreEqual("TcpClient close finalization failed.", client.LastError.Message);
            Assert.IsFalse(client.IsRunning);
        }

        [TestMethod]
        public void BaseTcpClient_Close_WhenCloseAsyncThrows_ShouldReportSyncCloseErrorWithoutEscalating()
        {
            var client = new ThrowingCloseDisposeTcpClient();

            client.Close();

            Assert.AreEqual(1, client.CloseAsyncCallCount);
            CollectionAssert.Contains(client.Errors.ConvertAll(ex => ex.Message), "TcpClient sync close failed.");
        }

        [TestMethod]
        public void BaseTcpServerClient_Close_WhenCloseAsyncThrows_ShouldReportSyncCloseErrorWithoutEscalating()
        {
            var serverClient = new ThrowingSendTcpServerClient
            {
                ThrowOnCloseAsync = true,
            };

            serverClient.Close();

            Assert.AreEqual(1, serverClient.CloseAsyncCallCount);
            CollectionAssert.Contains(serverClient.Errors.ConvertAll(ex => ex.Message), "TcpServerClient sync close failed.");
        }

        [TestMethod]
        public void BaseTcpClient_SendPackage_WhenEncodeThrows_ShouldReportErrorWithoutClosing()
        {
            var client = new ThrowingEncodePackageTcpClient();

            client.Send(new object());

            Assert.IsNotNull(client.LastError);
            Assert.AreEqual("encode package failed", client.LastError.Message);
            Assert.AreEqual(0, client.CloseAsyncCallCount);
        }
    }
}
