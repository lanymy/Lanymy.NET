using System;
using System.IO;
using System.Text;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class StreamReadRobustnessTests
    {
        [TestMethod]
        public void FileHelper_ReadExactly_WhenStreamReturnsPartialReads_ShouldFillWholeBuffer()
        {
            var sourceBytes = Encoding.UTF8.GetBytes("lanymy-read-exactly");
            var buffer = new byte[sourceBytes.Length];

            using var stream = new ChunkedReadStream(new MemoryStream(sourceBytes), 3);

            FileHelper.ReadExactly(stream, buffer, 0, buffer.Length);

            CollectionAssert.AreEqual(sourceBytes, buffer);
        }

        [TestMethod]
        public void LanymyIsolatedStorage_GetStringFromStream_WhenStreamReturnsPartialReads_ShouldRoundTripContent()
        {
            const string sourceString = "lanymy-isolated-storage";
            const string securityKey = "lanymy-key";
            var encoding = Encoding.UTF8;
            using var sourceStream = new MemoryStream();
            var isolatedStorage = new TestableIsolatedStorage();

            isolatedStorage.SaveStringToStreamForTest(sourceStream, sourceString, securityKey, encoding);

            using var readStream = new ChunkedReadStream(new MemoryStream(sourceStream.ToArray()), 2);

            var result = isolatedStorage.GetStringFromStreamForTest(readStream, securityKey, encoding);

            Assert.AreEqual(sourceString, result);
        }

        [TestMethod]
        public void LanymyCrypto_DencryptStreamFromStream_WhenHeaderStreamReturnsPartialReads_ShouldStillSucceed()
        {
            const string sourceString = "lanymy-crypto-stream";
            const string securityKey = "lanymy-key";
            var encoding = Encoding.UTF8;
            var crypto = new LanymyCrypto();
            var encryptModel = crypto.EncryptStringToBytes(sourceString, securityKey, false, encoding);

            using var encryptedStream = new ChunkedReadStream(new MemoryStream(encryptModel.EncryptedBytes), 2);
            using var sourceStream = new MemoryStream();

            var decryptModel = crypto.DencryptStreamFromStream(encryptedStream, sourceStream, securityKey, encoding);
            var result = encoding.GetString(sourceStream.ToArray());

            Assert.IsTrue(decryptModel.IsSuccess);
            Assert.AreEqual(sourceString, result);
        }

        private sealed class TestableIsolatedStorage : LanymyIsolatedStorage
        {
            public void SaveStringToStreamForTest(Stream stream, string sourceString, string securityKey, Encoding encoding)
            {
                SaveStringToStream(stream, sourceString, securityKey, encoding);
            }

            public string GetStringFromStreamForTest(Stream stream, string securityKey, Encoding encoding)
            {
                return GetStringFromStream(stream, securityKey, encoding);
            }
        }

        private sealed class ChunkedReadStream : Stream
        {
            private readonly Stream _innerStream;
            private readonly int _maxChunkSize;

            public ChunkedReadStream(Stream innerStream, int maxChunkSize)
            {
                _innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
                _maxChunkSize = maxChunkSize > 0 ? maxChunkSize : throw new ArgumentOutOfRangeException(nameof(maxChunkSize));
            }

            public override bool CanRead => _innerStream.CanRead;

            public override bool CanSeek => _innerStream.CanSeek;

            public override bool CanWrite => _innerStream.CanWrite;

            public override long Length => _innerStream.Length;

            public override long Position
            {
                get => _innerStream.Position;
                set => _innerStream.Position = value;
            }

            public override void Flush()
            {
                _innerStream.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _innerStream.Read(buffer, offset, Math.Min(count, _maxChunkSize));
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _innerStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _innerStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _innerStream.Write(buffer, offset, count);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _innerStream.Dispose();
                }

                base.Dispose(disposing);
            }
        }
    }
}
