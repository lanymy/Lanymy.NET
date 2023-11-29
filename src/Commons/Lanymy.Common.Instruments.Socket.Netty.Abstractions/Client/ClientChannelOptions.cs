using System;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments.Client
{


    public class ClientChannelOptions : BaseChannelOptions
    {

        public string ServerIP { get; }


        public ClientChannelOptions(string serverIp, int port, int lengthFieldOffset, int lengthFieldLength, int lengthAdjustment, int initialBytesToStrip, bool isUseSingleThreadEventLoop, int receiveBufferSize = BufferSizeKeys.BUFFER_SIZE_4K, int sendBufferSize = BufferSizeKeys.BUFFER_SIZE_4K, int sendDataIntervalMilliseconds = 100, int intervalHeartTotalMilliseconds = 3000, int heartTimeOutCount = 3, int backlog = 100) : base(port, lengthFieldOffset, lengthFieldLength, lengthAdjustment, initialBytesToStrip, isUseSingleThreadEventLoop, receiveBufferSize, sendBufferSize, sendDataIntervalMilliseconds, intervalHeartTotalMilliseconds, heartTimeOutCount, backlog)
        {
            ServerIP = serverIp;
        }

    }


}
