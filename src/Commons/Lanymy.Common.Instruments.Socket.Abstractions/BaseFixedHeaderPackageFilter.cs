using System;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 定义固定包头协议的拆包、验包和编解码基础能力。
    /// </summary>
    public abstract class BaseFixedHeaderPackageFilter<TPackage, TSendPackage, TSessionToken> : IFixedHeaderPackageFilter<TPackage, TSendPackage, TSessionToken>
        where TPackage : class
        where TSendPackage : class
        where TSessionToken : ISessionToken
    {
        /// <summary>
        /// 固定包头长度。
        /// </summary>
        public byte HeaderSize { get; }

        /// <summary>
        /// 初始化包过滤器。
        /// </summary>
        /// <param name="headerSize">固定包头长度。</param>
        protected BaseFixedHeaderPackageFilter(byte headerSize)
        {
            HeaderSize = headerSize;
        }

        /// <summary>
        /// 从包头中解析包体长度。
        /// </summary>
        /// <param name="cursorIndex">当前读取游标。</param>
        /// <param name="bufferBytes">当前可见的缓冲区数据。</param>
        /// <returns>包体长度。</returns>
        public abstract int GetBodyLengthFromHeader(int cursorIndex, byte[] bufferBytes);

        /// <summary>
        /// 校验完整包数据是否合法。
        /// </summary>
        /// <param name="packageBytes">完整包字节。</param>
        public abstract bool CheckPackage(byte[] packageBytes);

        /// <summary>
        /// 把发送模型编码为二进制包。
        /// </summary>
        /// <param name="sendPackage">待发送模型。</param>
        public abstract byte[] EncodePackage(TSendPackage sendPackage);

        /// <summary>
        /// 把完整包字节解码为业务模型。
        /// </summary>
        /// <param name="packageBytes">完整包字节。</param>
        public abstract TPackage DecodePackage(byte[] packageBytes);

        /// <summary>
        /// 计算当前缓存条件下允许的最大包长度。
        /// </summary>
        protected virtual int GetMaxPackageLength(BufferModel buffer, CacheModel cache)
        {
            return buffer.BufferSize + cache.Data.Length;
        }

        /// <summary>
        /// 计算并校验完整包长度。
        /// </summary>
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

        /// <summary>
        /// 校验缓存区是否还能容纳当前半包。
        /// </summary>
        protected virtual void EnsureCacheCapacity(int cacheLength, CacheModel cache)
        {
            if (cacheLength > cache.Data.Length)
            {
                throw new InvalidOperationException($"cache length {cacheLength} exceeded capacity {cache.Data.Length}.");
            }
        }

        /// <summary>
        /// 从当前缓冲区和半包缓存中提取一帧完整包数据。
        /// </summary>
        /// <param name="buffer">当前接收缓冲区。</param>
        /// <param name="cache">跨次接收的半包缓存。</param>
        /// <returns>提取到完整包时返回完整包字节；否则返回 <see langword="null"/>。</returns>
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
                    // 半包长度连包头都不够时，直接整体缓存等待下次接收。
                    EnsureCacheCapacity(position, cache);
                    cache.Position = position;
                    Array.Copy(dataBytesTemp, 0, cache.Data, 0, cache.Position);
                    buffer.Clear();
                    return null;
                }

                packageLength = GetPackageLength(0, dataBytesTemp, buffer, cache);

                if (packageLength > position)
                {
                    // 包头已到齐但包体还没收完时，继续保留在 cache 中。
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
                        // 当前 buffer 不足以拼出整包时，把尾部残片移入 cache。
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

        /// <summary>
        /// 获取当前会话的心跳包字节。
        /// </summary>
        /// <param name="sessionToken">当前会话令牌。</param>
        public abstract byte[] GetHeartBytes(ISessionToken sessionToken);
    }
}
