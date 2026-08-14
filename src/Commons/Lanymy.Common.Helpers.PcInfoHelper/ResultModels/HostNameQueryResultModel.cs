using System;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// 主机名称查询结果。
    /// </summary>
    public class HostNameQueryResultModel
    {
        /// <summary>
        /// 主机名称。
        /// </summary>
        public string HostName { get; set; }

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
