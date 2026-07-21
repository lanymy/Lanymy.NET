using System.Net;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{

    public interface ITcpServerClient : IBaseTcpServerClient, ITcp
    {

        event TcpServerClientErrorEvent ServerClientErrorEvent;

        event TcpReceiveDataEvent ReceiveDataEvent;

        event TcpStartReceiveEvent StartReceiveEvent;

        event TcpCloseEvent CloseEvent;

        event TcpHeartEvent HeartEvent;

        //bool Send(byte[] data);
        void Send(byte[] sendDataBytes);

        Task SendAsync(byte[] sendDataBytes);

    }

}
