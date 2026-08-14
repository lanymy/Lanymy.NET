using System.Text;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{
    /// <summary>
    /// 定义字符串与 Base64 文本之间的加解密入口。
    /// </summary>
    public interface ICryptoString
    {
        /// <summary>
        /// 把字符串加密为字节数组。
        /// </summary>
        EncryptStringDigestInfoModel EncryptStringToBytes(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null);

        /// <summary>
        /// 把密文字节解密为字符串。
        /// </summary>
        EncryptStringDigestInfoModel DecryptStringFromBytes(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null);

        /// <summary>
        /// 把字符串加密为 Base64 文本。
        /// </summary>
        EncryptBase64StringDigestInfoModel EncryptStringToBase64String(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null);

        /// <summary>
        /// 把 Base64 密文解密为字符串。
        /// </summary>
        EncryptBase64StringDigestInfoModel DecryptStringFromBase64String(string base64StringToDecrypt, string secretKey = null, Encoding encoding = null);

        /// <summary>
        /// 把字符串加密并写入文件。
        /// </summary>
        EncryptStringFileDigestInfoModel EncryptStringToFile(string sourceString, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null);

        /// <summary>
        /// 从加密文件解密出字符串。
        /// </summary>
        EncryptStringFileDigestInfoModel DecryptStringFromFile(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null);
    }
}
