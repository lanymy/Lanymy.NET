using System;
using Lanymy.Common.ConstKeys;

namespace Lanymy.Common.Instruments
{
    public class LanymyTcpServerClient : BaseTcpServerClient
    {
        public LanymyTcpServerClient(System.Net.Sockets.Socket socket, int receiveBufferSize = BufferSizeKeys.BUFFER_SIZE_8K, int sendBufferSize = BufferSizeKeys.BUFFER_SIZE_8K, int sendDataIntervalMilliseconds = 500) : base(socket, receiveBufferSize, sendBufferSize, sendDataIntervalMilliseconds)
        {
        }

        protected override void OnStartReceiveEvent()
        {
            throw new NotImplementedException();
        }

        protected override void OnCloseEvent()
        {
            throw new NotImplementedException();
        }

        protected override void OnServerClientErrorEvent(Exception ex)
        {
            throw new NotImplementedException();
        }

        protected override void OnReceiveDataEvent(BufferModel buffer, CacheModel cache)
        {
            throw new NotImplementedException();
        }
    }

}
