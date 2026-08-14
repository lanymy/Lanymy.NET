using System;
using Lanymy.Common.Enums;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// RSA 二进制加解密结果。
    /// </summary>
    public class SecurityRsaBytesOperationResultModel
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
        /// 结果二进制数据。
        /// </summary>
        public byte[] ResultBytes { get; set; }

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
