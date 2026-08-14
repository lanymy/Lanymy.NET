using System;

namespace Lanymy.Common.Instruments.Common
{
    /// <summary>
    /// 定义 Netty 固定包头协议的心跳、编码和解码基础能力。
    /// </summary>
    public abstract class BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession
    {
        /// <summary>
        /// 获取当前会话的心跳包字节。
        /// </summary>
        /// <param name="channelSession">当前会话。</param>
        /// <returns>心跳包字节数组。</returns>
        public virtual byte[] GetHeartBytes(TChannelSession channelSession)
        {
            return channelSession.CacheHeartBytes;
        }


        //public virtual byte[] GetConnectBytes(TChannelSession channelSession)
        //{
        //    return channelSession.CacheConnectBytes;
        //}


        //public virtual byte[] GetConnectLogoutBytes(TChannelSession channelSession)
        //{
        //    return channelSession.CacheConnectLogoutBytes;
        //}
        /// <summary>
        /// 校验一帧完整包字节是否合法。
        /// </summary>
        public abstract bool CheckPackage(ReadOnlySpan<byte> packageBytes);

        /// <summary>
        /// 把发送模型编码为包字节。
        /// </summary>
        public abstract byte[] EncodePackage(TSendPackage sendPackage);

        /// <summary>
        /// 把完整包字节解码为接收模型。
        /// </summary>
        public abstract TReceivePackage DecodePackage(ReadOnlySpan<byte> packageBytes);
    }
}
