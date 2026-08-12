using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.Common.Instruments.Common
{
    /// <summary>
    /// TCP 服务端基础状态接口。
    /// </summary>
    public interface IBaseTcpServer
    {
        /// <summary>
        /// 当前是否处于接入监听状态。
        /// </summary>
        bool IsAccept { get; }

        /// <summary>
        /// 当前监听端口。
        /// </summary>
        int Port { get; }
    }

}
