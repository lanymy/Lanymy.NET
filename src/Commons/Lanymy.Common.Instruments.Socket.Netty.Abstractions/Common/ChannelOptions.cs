using System;

namespace Lanymy.Common.Instruments.Common
{


    public class ChannelOptions
    {

        public string ServerIP { get; }
        public int Port { get; }
        public int ReceiveBufferSize { get; }
        public int SendBufferSize { get; }
        public int SendDataIntervalMilliseconds { get; }
        public int IntervalHeartTotalMilliseconds { get; }
        public int HeartTimeOutCount { get; }

        public int LengthFieldOffset { get; }
        public int LengthFieldLength { get; }
        public int LengthAdjustment { get; }
        public int InitialBytesToStrip { get; }

        public bool IsUseSingleThreadEventLoop { get; }

        public int Backlog { get; }

        //public bool IsDebug { get; set; }

        public ChannelOptions(string serverIP, int port, int lengthFieldOffset, int lengthFieldLength, int lengthAdjustment, int initialBytesToStrip, bool isUseSingleThreadEventLoop, int receiveBufferSize = ConstKeys.BufferSizeKeys.BUFFER_SIZE_4K, int sendBufferSize = ConstKeys.BufferSizeKeys.BUFFER_SIZE_4K, int sendDataIntervalMilliseconds = 100, int intervalHeartTotalMilliseconds = 3 * 1000, int heartTimeOutCount = 3, int backlog = 100)
        {

            if (port <= 0)
            {
                throw new Exception("server port is <= 0");
            }

            ServerIP = serverIP;
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
