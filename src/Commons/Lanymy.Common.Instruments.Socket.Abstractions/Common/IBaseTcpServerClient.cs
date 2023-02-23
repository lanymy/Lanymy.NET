using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Lanymy.Common.Instruments.Common
{


    public interface IBaseTcpServerClient
    {


        /// <summary>
        /// LocalEndPoint
        /// </summary>
        EndPoint LocalEndPoint { get; }

        /// <summary>
        /// RemoteEndPoint
        /// </summary>
        EndPoint RemoteEndPoint { get; }

        /// <summary>
        /// 用户定义的对象
        /// </summary>
        ISessionToken CurrentSessionToken { get; set; }

    }


}
