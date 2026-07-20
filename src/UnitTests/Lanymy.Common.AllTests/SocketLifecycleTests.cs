using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
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
                return true;
            }

            public override byte[] EncodePackage(object sendPackage)
            {
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
        public void BaseTcpServerClient_OnStartReceiveEventThrow_ShouldReportErrorWithoutClosing()
        {
            var serverClient = new ThrowingStartReceiveTcpServerClient();

            serverClient.TriggerStartReceive();

            Assert.IsNotNull(serverClient.LastError);
            Assert.AreEqual("start receive callback failed", serverClient.LastError.Message);
            Assert.AreEqual(0, serverClient.CloseAsyncCallCount);
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
    }
}
