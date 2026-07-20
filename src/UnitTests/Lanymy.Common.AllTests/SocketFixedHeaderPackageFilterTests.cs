using System;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class SocketFixedHeaderPackageFilterTests
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

        private sealed class TestFixedHeaderPackageFilter : BaseFixedHeaderPackageFilter<object, object, TestSessionToken>
        {
            public TestFixedHeaderPackageFilter()
                : base(4)
            {
            }

            public override int GetBodyLengthFromHeader(int cursorIndex, byte[] bufferBytes)
            {
                return bufferBytes[cursorIndex + 3];
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
        }

        [TestMethod]
        public void GetPackageBytes_WithCachedPartialHeader_ShouldKeepWaitingForMoreBytes()
        {
            var filter = new TestFixedHeaderPackageFilter();
            var buffer = new BufferModel();
            var cache = new CacheModel();

            cache.Data[0] = 0xAA;
            cache.Data[1] = 0x01;
            cache.Position = 2;

            buffer.BufferData[0] = 0x02;
            buffer.Position = 1;

            var packageBytes = filter.GetPackageBytes(buffer, cache);

            Assert.IsNull(packageBytes);
            Assert.AreEqual(3, cache.Position);
            CollectionAssert.AreEqual(new byte[] { 0xAA, 0x01, 0x02 }, new ArraySegment<byte>(cache.Data, 0, cache.Position).ToArray());
            Assert.AreEqual(0, buffer.Position);
            Assert.AreEqual(0, buffer.CursorIndex);
        }

        [TestMethod]
        public void GetPackageBytes_WithCachedHeaderButIncompleteBody_ShouldKeepMergedBytesInCache()
        {
            var filter = new TestFixedHeaderPackageFilter();
            var buffer = new BufferModel();
            var cache = new CacheModel();

            cache.Data[0] = 0xAA;
            cache.Data[1] = 0x01;
            cache.Data[2] = 0x02;
            cache.Data[3] = 0x03;
            cache.Position = 4;

            buffer.BufferData[0] = 0x10;
            buffer.BufferData[1] = 0x11;
            buffer.Position = 2;

            var packageBytes = filter.GetPackageBytes(buffer, cache);

            Assert.IsNull(packageBytes);
            Assert.AreEqual(6, cache.Position);
            CollectionAssert.AreEqual(new byte[] { 0xAA, 0x01, 0x02, 0x03, 0x10, 0x11 }, new ArraySegment<byte>(cache.Data, 0, cache.Position).ToArray());
            Assert.AreEqual(0, buffer.Position);
            Assert.AreEqual(0, buffer.CursorIndex);
        }

        [TestMethod]
        public void GetPackageBytes_WithCachedPackageAndRemainingBytes_ShouldReturnCompletePackage()
        {
            var filter = new TestFixedHeaderPackageFilter();
            var buffer = new BufferModel();
            var cache = new CacheModel();

            cache.Data[0] = 0xAA;
            cache.Data[1] = 0x01;
            cache.Data[2] = 0x02;
            cache.Data[3] = 0x03;
            cache.Position = 4;

            buffer.BufferData[0] = 0x10;
            buffer.BufferData[1] = 0x11;
            buffer.BufferData[2] = 0x12;
            buffer.BufferData[3] = 0xFF;
            buffer.Position = 4;

            var packageBytes = filter.GetPackageBytes(buffer, cache);

            CollectionAssert.AreEqual(new byte[] { 0xAA, 0x01, 0x02, 0x03, 0x10, 0x11, 0x12 }, packageBytes);
            Assert.AreEqual(0, cache.Position);
            Assert.AreEqual(3, buffer.CursorIndex);
            Assert.AreEqual(4, buffer.Position);
        }
    }
}
