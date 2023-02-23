using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.Common.Instruments.Common
{
    public interface IBaseTcpServer
    {
        bool IsAccept { get; }

        int Port { get; }
    }

}
