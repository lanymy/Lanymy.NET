using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{
    /// <summary>
    /// 定义针对“加密文件本身”的摘要读取与文件到文件转换能力。
    /// </summary>
    public interface ICryptoFile
    {
        /// <summary>
        /// 从加密文件读取摘要信息，而不解密正文。
        /// </summary>
        EncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedFile(string encryptedFileFullPath, Encoding encoding = null);

        /// <summary>
        /// 把原文件加密到目标文件。
        /// </summary>
        EncryptStringFileDigestInfoModel EncryptFileToFile(string sourceFileFullPath, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null);

        /// <summary>
        /// 把加密文件解密到目标文件。
        /// </summary>
        EncryptStringFileDigestInfoModel DecryptFileFromFile(string encryptedFileFullPath, string sourceFileFullPath, string secretKey = null, Encoding encoding = null);
    }
}
