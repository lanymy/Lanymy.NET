using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{
    /// <summary>
    /// 定义字节数组与文件之间的加解密入口。
    /// </summary>
    public interface ICryptoBytes
    {
        /// <summary>
        /// 把字节数组加密为字节数组，并返回摘要信息。
        /// </summary>
        EncryptBytesDigestInfoModel EncryptBytesToBytes(byte[] bytesToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null);
        
        /// <summary>
        /// 把密文字节解密为原始字节数组，并返回摘要信息。
        /// </summary>
        EncryptBytesDigestInfoModel DecryptBytesFromBytes(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null);
        
        /// <summary>
        /// 把字节数组加密并写入文件。
        /// </summary>
        EncryptStringFileDigestInfoModel EncryptBytesToFile(byte[] sourceBytes, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null);
        
        /// <summary>
        /// 从加密文件解密出原始字节数组。
        /// </summary>
        EncryptStringFileDigestInfoModel DecryptBytesFromFile(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null);
    }
}
