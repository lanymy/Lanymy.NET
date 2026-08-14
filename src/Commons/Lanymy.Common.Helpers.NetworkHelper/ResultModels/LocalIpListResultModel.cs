using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// 本地 IP 列表查询结果。
    /// </summary>
    public class LocalIpListResultModel
    {
        /// <summary>
        /// 查询的地址族；为 null 表示不过滤。
        /// </summary>
        public AddressFamily? AddressFamily { get; set; }

        /// <summary>
        /// 查询到的地址列表。
        /// </summary>
        public IReadOnlyList<IPAddress> Addresses { get; set; } = Array.Empty<IPAddress>();

        /// <summary>
        /// 查询是否执行成功。
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 查询异常。
        /// </summary>
        public Exception Exception { get; set; }
    }
}
