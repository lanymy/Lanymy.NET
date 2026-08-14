using System;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments.Client
{
    /// <summary>
    /// 客户端通道配置，额外包含目标服务端 IP。
    /// </summary>
    public class ClientChannelOptions : BaseChannelOptions
    {
        /// <summary>
        /// 目标服务端 IP。
        /// </summary>
        public string ServerIP { get; }

        /// <summary>
        /// 初始化客户端通道配置。
        /// </summary>
        public ClientChannelOptions(string serverIp, int port, int lengthFieldOffset, int lengthFieldLength, int lengthAdjustment, int initialBytesToStrip, bool isUseSingleThreadEventLoop, int receiveBufferSize = BufferSizeKeys.BUFFER_SIZE_4K, int sendBufferSize = BufferSizeKeys.BUFFER_SIZE_4K, int sendDataIntervalMilliseconds = 100, int intervalHeartTotalMilliseconds = 3000, int heartTimeOutCount = 3, int backlog = 100) : base(port, lengthFieldOffset, lengthFieldLength, lengthAdjustment, initialBytesToStrip, isUseSingleThreadEventLoop, receiveBufferSize, sendBufferSize, sendDataIntervalMilliseconds, intervalHeartTotalMilliseconds, heartTimeOutCount, backlog)
        {
            ServerIP = serverIp;
        }
    }
}
