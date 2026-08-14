using System;
using Lanymy.Common.Enums;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// RSA Blob 二进制密钥创建结果。
    /// </summary>
    public class SecurityRsaKeyBlobBytesResultModel
    {
        /// <summary>
        /// 密钥长度。
        /// </summary>
        public RsaKeySizeTypeEnum KeySizeType { get; set; }

        /// <summary>
        /// 公钥 Blob 二进制数据。
        /// </summary>
        public byte[] PublicKeyBlobBytes { get; set; }

        /// <summary>
        /// 私钥 Blob 二进制数据。
        /// </summary>
        public byte[] PrivateKeyBlobBytes { get; set; }

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
