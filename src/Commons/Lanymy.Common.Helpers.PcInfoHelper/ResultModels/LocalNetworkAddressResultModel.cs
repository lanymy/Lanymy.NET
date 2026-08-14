using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// 本机网络地址查询结果。
    /// </summary>
    public class LocalNetworkAddressResultModel
    {
        /// <summary>
        /// 主机名称。
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// 查询的地址族。
        /// </summary>
        public AddressFamily? AddressFamily { get; set; }

        /// <summary>
        /// 最终选中的地址。
        /// </summary>
        public IPAddress Address { get; set; }

        /// <summary>
        /// 最终选中的地址文本。
        /// </summary>
        public string AddressText { get; set; }

        /// <summary>
        /// 查询过程中命中的候选地址列表。
        /// </summary>
        public IReadOnlyList<IPAddress> CandidateAddresses { get; set; } = Array.Empty<IPAddress>();

        /// <summary>
        /// 查询是否成功。
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 查询异常。
        /// </summary>
        public Exception Exception { get; set; }
    }
}
