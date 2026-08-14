using System;
using Lanymy.Common.Enums;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// RSA Blob Base64 字符串密钥创建结果。
    /// </summary>
    public class SecurityRsaKeyBlobStringResultModel
    {
        /// <summary>
        /// 密钥长度。
        /// </summary>
        public RsaKeySizeTypeEnum KeySizeType { get; set; }

        /// <summary>
        /// 公钥 Blob Base64 字符串。
        /// </summary>
        public string PublicKeyBlobBase64String { get; set; }

        /// <summary>
        /// 私钥 Blob Base64 字符串。
        /// </summary>
        public string PrivateKeyBlobBase64String { get; set; }

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
