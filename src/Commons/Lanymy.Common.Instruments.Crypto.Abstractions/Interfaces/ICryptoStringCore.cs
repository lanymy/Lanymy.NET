using System.Text;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{
    /// <summary>
    /// 定义返回具体摘要模型的字符串加解密泛型入口。
    /// </summary>
    public interface ICryptoStringCore
    {
        /// <summary>
        /// 把字符串加密为字节数组，并返回指定摘要模型。
        /// </summary>
        TEncryptDigestInfoModel EncryptStringToBytes<TEncryptDigestInfoModel>(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringDigestInfoModel, new();

        /// <summary>
        /// 把密文字节解密为字符串，并返回指定摘要模型。
        /// </summary>
        TEncryptDigestInfoModel DecryptStringFromBytes<TEncryptDigestInfoModel>(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringDigestInfoModel, new();

        /// <summary>
        /// 把字符串加密为 Base64 文本，并返回指定摘要模型。
        /// </summary>
        TEncryptDigestInfoModel EncryptStringToBase64String<TEncryptDigestInfoModel>(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptBase64StringDigestInfoModel, new();

        /// <summary>
        /// 把 Base64 密文解密为字符串，并返回指定摘要模型。
        /// </summary>
        TEncryptDigestInfoModel DecryptStringFromBase64String<TEncryptDigestInfoModel>(string base64StringToDecrypt, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptBase64StringDigestInfoModel, new();

        /// <summary>
        /// 把字符串加密到文件，并返回指定摘要模型。
        /// </summary>
        TEncryptDigestInfoModel EncryptStringToFile<TEncryptDigestInfoModel>(string sourceString, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringFileDigestInfoModel, new();

        /// <summary>
        /// 从加密文件解密出字符串，并返回指定摘要模型。
        /// </summary>
        TEncryptDigestInfoModel DecryptStringFromFile<TEncryptDigestInfoModel>(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringFileDigestInfoModel, new();
    }
}
