using System;
using System.Net.Sockets;
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

            public TestFixedHeaderPackageFilter(byte[] packageBytes = null, bool checkPackageResult = true)
                : base(4)
            {
                _packageBytes = packageBytes;
                _checkPackageResult = checkPackageResult;
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

        private sealed class TestTcpServerClient : BaseTcpServerClient
        {
            public int CloseAsyncCallCount { get; private set; }

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
        }

        private sealed class TestTcpServer : BaseTcpServer<TestTcpServerClient, TestSessionToken, TestFixedHeaderPackageFilter, object, TestSendPackage>
        {
            public int ErrorCallbackCount { get; private set; }
            public Exception LastError { get; private set; }

            public TestTcpServer(TestFixedHeaderPackageFilter fixedHeaderPackageFilter)
                : base(fixedHeaderPackageFilter, port: 9527, receiveBufferSize: 16, sendBufferSize: 16, sendDataIntervalMilliseconds: 1, intervalHeartTotalMilliseconds: 1, heartTimeOutCount: 1)
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
                return false;
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
                throw new AssertFailedException($"Unexpected server error: {ex.Message}");
            }

            public void TrackClient(TestTcpServerClient client)
            {
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

            public void TriggerReceiveLoop(TestTcpServerClient client, BufferModel buffer, CacheModel cache)
            {
                OnServerClientReceiveDataLoopEvent(client, buffer, cache);
            }
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
    }
}
