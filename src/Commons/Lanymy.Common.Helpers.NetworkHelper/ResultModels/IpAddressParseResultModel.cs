using System;
using System.Net;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// IP 地址字符串解析结果。
    /// </summary>
    public class IpAddressParseResultModel
    {
        /// <summary>
        /// 原始输入字符串。
        /// </summary>
        public string Input { get; set; }

        /// <summary>
        /// 解析后的 IP 地址。
        /// </summary>
        public IPAddress Address { get; set; }

        /// <summary>
        /// 是否解析成功。
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 解析异常。
        /// </summary>
        public Exception Exception { get; set; }
    }
}
