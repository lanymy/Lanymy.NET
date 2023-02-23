using System.Net;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{

    public interface ITcpServerClient : IBaseTcpServerClient, ITcp
    {



        //bool Send(byte[] data);
        void Send(byte[] data);

    }

}
