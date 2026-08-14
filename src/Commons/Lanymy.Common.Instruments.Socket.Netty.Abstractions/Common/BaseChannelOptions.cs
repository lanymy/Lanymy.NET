using System;

namespace Lanymy.Common.Instruments.Common
{
    /// <summary>
    /// 定义 Netty 通道的公共配置项，包括端口、缓冲区、心跳和长度字段协议参数。
    /// </summary>
    public abstract class BaseChannelOptions
    {
        /// <summary>
        /// 监听或连接端口。
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Socket 接收缓冲区大小。
        /// </summary>
        public int ReceiveBufferSize { get; }

        /// <summary>
        /// Socket 发送缓冲区大小。
        /// </summary>
        public int SendBufferSize { get; }

        /// <summary>
        /// 连续发送之间的节流间隔。
        /// </summary>
        public int SendDataIntervalMilliseconds { get; }

        /// <summary>
        /// 心跳发送间隔。
        /// </summary>
        public int IntervalHeartTotalMilliseconds { get; }

        /// <summary>
        /// 允许连续丢失心跳的次数。
        /// </summary>
        public int HeartTimeOutCount { get; }

        /// <summary>
        /// 长度字段起始偏移。
        /// </summary>
        public int LengthFieldOffset { get; }

        /// <summary>
        /// 长度字段占用字节数。
        /// </summary>
        public int LengthFieldLength { get; }

        /// <summary>
        /// 长度修正值。
        /// </summary>
        public int LengthAdjustment { get; }

        /// <summary>
        /// 解码后需要剥离的前导字节数。
        /// </summary>
        public int InitialBytesToStrip { get; }

        /// <summary>
        /// 是否为最终业务处理器单独挂单线程事件循环。
        /// </summary>
        public bool IsUseSingleThreadEventLoop { get; }

        /// <summary>
        /// 服务端监听队列长度。
        /// </summary>
        public int Backlog { get; }

        //public bool IsDebug { get; set; }
        /// <summary>
        /// 初始化通道配置。
        /// </summary>
        protected BaseChannelOptions(int port, int lengthFieldOffset, int lengthFieldLength, int lengthAdjustment, int initialBytesToStrip, bool isUseSingleThreadEventLoop, int receiveBufferSize = ConstKeys.BufferSizeKeys.BUFFER_SIZE_4K, int sendBufferSize = ConstKeys.BufferSizeKeys.BUFFER_SIZE_4K, int sendDataIntervalMilliseconds = 100, int intervalHeartTotalMilliseconds = 3 * 1000, int heartTimeOutCount = 3, int backlog = 100)
        {
            if (port <= 0)
            {
                throw new Exception("server port is <= 0");
            }

            Port = port;
            LengthFieldOffset = lengthFieldOffset;
            LengthFieldLength = lengthFieldLength;
            LengthAdjustment = lengthAdjustment;
            InitialBytesToStrip = initialBytesToStrip;
            IsUseSingleThreadEventLoop = isUseSingleThreadEventLoop;
            Backlog = backlog;
            ReceiveBufferSize = receiveBufferSize;
            SendBufferSize = sendBufferSize;
            SendDataIntervalMilliseconds = sendDataIntervalMilliseconds;
            IntervalHeartTotalMilliseconds = intervalHeartTotalMilliseconds;
            HeartTimeOutCount = heartTimeOutCount;
        }
    }
}
