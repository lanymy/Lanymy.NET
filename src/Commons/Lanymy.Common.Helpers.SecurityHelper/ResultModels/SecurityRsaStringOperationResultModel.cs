using System;
using Lanymy.Common.Enums;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// RSA 字符串加解密结果。
    /// </summary>
    public class SecurityRsaStringOperationResultModel
    {
        /// <summary>
        /// 是否加密操作。
        /// </summary>
        public bool IsEncryptOperation { get; set; }

        /// <summary>
        /// RSA 密钥长度。
        /// </summary>
        public RsaKeySizeTypeEnum KeySizeType { get; set; }

        /// <summary>
        /// 结果字符串。
        /// </summary>
        public string ResultString { get; set; }

        /// <summary>
        /// 是否执行成功。
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 相关异常。
        /// </summary>
        public Exception Exception { get; set; }
    }
}
