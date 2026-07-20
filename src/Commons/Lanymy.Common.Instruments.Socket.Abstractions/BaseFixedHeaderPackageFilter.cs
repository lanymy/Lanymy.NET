using System;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{

    public abstract class BaseFixedHeaderPackageFilter<TPackage, TSendPackage, TSessionToken> : IFixedHeaderPackageFilter<TPackage, TSendPackage, TSessionToken>
        where TPackage : class
        where TSendPackage : class
        where TSessionToken : ISessionToken
    {

        public byte HeaderSize { get; }

        protected BaseFixedHeaderPackageFilter(byte headerSize)
        {
            HeaderSize = headerSize;
        }


        public abstract int GetBodyLengthFromHeader(int cursorIndex, byte[] bufferBytes);

        public abstract bool CheckPackage(byte[] packageBytes);

        public abstract byte[] EncodePackage(TSendPackage sendPackage);

        public abstract TPackage DecodePackage(byte[] packageBytes);

        protected virtual int GetMaxPackageLength(BufferModel buffer, CacheModel cache)
        {
            return buffer.BufferSize + cache.Data.Length;
        }

        protected virtual int GetPackageLength(int cursorIndex, byte[] bufferBytes, BufferModel buffer, CacheModel cache)
        {
            var bodyLength = GetBodyLengthFromHeader(cursorIndex, bufferBytes);
            if (bodyLength < 0)
            {
                throw new InvalidOperationException("body length is invalid.");
            }

            var packageLength = HeaderSize + bodyLength;
            if (packageLength < HeaderSize)
            {
                throw new InvalidOperationException("package length is invalid.");
            }

            var maxPackageLength = GetMaxPackageLength(buffer, cache);
            if (packageLength > maxPackageLength)
            {
                throw new InvalidOperationException($"package length {packageLength} exceeded max {maxPackageLength}.");
            }

            return packageLength;
        }

        protected virtual void EnsureCacheCapacity(int cacheLength, CacheModel cache)
        {
            if (cacheLength > cache.Data.Length)
            {
                throw new InvalidOperationException($"cache length {cacheLength} exceeded capacity {cache.Data.Length}.");
            }
        }


        public virtual byte[] GetPackageBytes(BufferModel buffer, CacheModel cache)
        {

            byte[] dataBytes = null;
            var packageLength = 0;


            if (cache.Position > 0)
            {

                var position = cache.Position + buffer.Position;

                var dataBytesTemp = new byte[position];

                Array.Copy(cache.Data, 0, dataBytesTemp, 0, cache.Position);
                Array.Copy(buffer.BufferData, 0, dataBytesTemp, cache.Position, buffer.Position);

                if (HeaderSize > position)
                {
                    EnsureCacheCapacity(position, cache);
                    cache.Position = position;
                    Array.Copy(dataBytesTemp, 0, cache.Data, 0, cache.Position);
                    buffer.Clear();
                    return null;
                }

                packageLength = GetPackageLength(0, dataBytesTemp, buffer, cache);

                if (packageLength > position)
                {
                    EnsureCacheCapacity(position, cache);
                    cache.Position = position;
                    Array.Copy(dataBytesTemp, 0, cache.Data, 0, cache.Position);
                    buffer.Clear();
                    return null;
                }

                dataBytes = new byte[packageLength];

                Array.Copy(dataBytesTemp, 0, dataBytes, 0, packageLength);

                buffer.CursorIndex = packageLength - cache.Position;

                cache.Clear();


            }
            else
            {

                if (buffer.CursorIndex >= buffer.Position)
                {
                    buffer.Clear();
                }
                else
                {

                    if (HeaderSize > buffer.Position - buffer.CursorIndex)
                    {
                        packageLength = buffer.Position + 1;
                    }
                    else
                    {
                        packageLength = GetPackageLength(buffer.CursorIndex, buffer.BufferData, buffer, cache);
                    }


                    if (buffer.CursorIndex + packageLength > buffer.Position)//数据未接收完整，先缓存
                    {
                        cache.Position = buffer.Position - buffer.CursorIndex;
                        EnsureCacheCapacity(cache.Position, cache);
                        Array.Copy(buffer.BufferData, buffer.CursorIndex, cache.Data, 0, cache.Position);
                        buffer.Clear();
                    }
                    else
                    {
                        dataBytes = new byte[packageLength];
                        Array.Copy(buffer.BufferData, buffer.CursorIndex, dataBytes, 0, packageLength);
                        buffer.CursorIndex += packageLength;

                    }
                }

            }

            return dataBytes;

        }

        public abstract byte[] GetHeartBytes(ISessionToken sessionToken);


    }

}
