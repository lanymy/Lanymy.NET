using System;
using System.Security.Cryptography.X509Certificates;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// 证书加载结果。
    /// </summary>
    public class SecurityCertificateLoadResultModel
    {
        /// <summary>
        /// 证书文件路径。
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 证书存储位置。
        /// </summary>
        public StoreLocation StoreLocation { get; set; }

        /// <summary>
        /// 证书指纹。
        /// </summary>
        public string CertificateThumbprint { get; set; }

        /// <summary>
        /// 证书主题。
        /// </summary>
        public string CertificateSubject { get; set; }

        /// <summary>
        /// 受信任证书存储名称。
        /// </summary>
        public StoreName TrustedStoreName { get; set; }

        /// <summary>
        /// 个人证书存储名称。
        /// </summary>
        public StoreName PersonalStoreName { get; set; }

        /// <summary>
        /// 是否新增到受信任证书存储。
        /// </summary>
        public bool AddedToTrustedStore { get; set; }

        /// <summary>
        /// 是否新增到个人证书存储。
        /// </summary>
        public bool AddedToPersonalStore { get; set; }

        /// <summary>
        /// 受信任证书存储中是否已存在证书。
        /// </summary>
        public bool ExistsInTrustedStore { get; set; }

        /// <summary>
        /// 个人证书存储中是否已存在证书。
        /// </summary>
        public bool ExistsInPersonalStore { get; set; }

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
