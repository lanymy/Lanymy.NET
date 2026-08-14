using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// Ping 操作结果。
    /// </summary>
    public class PingResultModel
    {
        /// <summary>
        /// 目标地址字符串。
        /// </summary>
        public string AddressText { get; set; }

        /// <summary>
        /// 目标地址。
        /// </summary>
        public IPAddress Address { get; set; }

        /// <summary>
        /// Ping 是否成功。
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Ping 状态。
        /// </summary>
        public IPStatus? Status { get; set; }

        /// <summary>
        /// 往返耗时（毫秒）。
        /// </summary>
        public long? RoundtripTime { get; set; }

        /// <summary>
        /// 失败异常。
        /// </summary>
        public Exception Exception { get; set; }
    }
}
