using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Lanymy.Common.Instruments.Common
{


    /// <summary>
    /// TCP 服务端单连接基础状态接口。
    /// </summary>
    public interface IBaseTcpServerClient
    {


        /// <summary>
        /// 当前本地终结点。
        /// </summary>
        EndPoint LocalEndPoint { get; }

        /// <summary>
        /// 当前远端终结点。
        /// </summary>
        EndPoint RemoteEndPoint { get; }

        /// <summary>
        /// 当前会话令牌。
        /// 由服务端在接入链上绑定，用于表达托管态与业务会话状态。
        /// </summary>
        ISessionToken CurrentSessionToken { get; set; }

    }


}
