using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{


    public interface ITcpServer : IBaseTcpServer, ITcp
    {

        void Start();

    }

}
