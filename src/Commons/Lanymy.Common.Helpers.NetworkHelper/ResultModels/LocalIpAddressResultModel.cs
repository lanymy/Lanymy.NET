using System;
using System.Collections.Generic;
using System.Net;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// 本地首选 IP 地址查询结果。
    /// </summary>
    public class LocalIpAddressResultModel
    {
        /// <summary>
        /// 选中的地址。
        /// </summary>
        public IPAddress Address { get; set; }

        /// <summary>
        /// 选中的地址文本。
        /// </summary>
        public string AddressText { get; set; }

        /// <summary>
        /// 本次查询过程中拿到的候选地址列表。
        /// </summary>
        public IReadOnlyList<IPAddress> Addresses { get; set; } = Array.Empty<IPAddress>();

        /// <summary>
        /// 是否成功找到可用地址。
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 查询异常。
        /// </summary>
        public Exception Exception { get; set; }
    }
}
