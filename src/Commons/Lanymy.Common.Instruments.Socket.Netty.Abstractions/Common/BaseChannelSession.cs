using System;
using System.Net;

namespace Lanymy.Common.Instruments.Common
{
    /// <summary>
    /// 表示 Netty 通道会话的基础身份和状态信息。
    /// </summary>
    public abstract class BaseChannelSession
    {
        /// <summary>
        /// 当前会话唯一标识。
        /// </summary>
        public Guid SessionID { get; } = Guid.NewGuid();

        /// <summary>
        /// 对端 IP 终结点。
        /// </summary>
        public IPEndPoint RemoteIpEndPoint { get; set; }

        /// <summary>
        /// 业务层登录态标记。
        /// </summary>
        public bool IsLogin { get; set; }

        /// <summary>
        /// 当前会话缓存的心跳包。
        /// </summary>
        public abstract byte[] CacheHeartBytes { get; }
    }
}
