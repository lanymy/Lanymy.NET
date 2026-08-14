using System.Text;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{
    /// <summary>
    /// 定义返回具体摘要模型的字节加解密泛型入口。
    /// </summary>
    public interface ICryptoBytesCore
    {
        /// <summary>
        /// 把字节数组加密为字节数组，并返回指定摘要模型。
        /// </summary>
        TEncryptDigestInfoModel EncryptBytesToBytes<TEncryptDigestInfoModel>(byte[] bytesToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptBytesDigestInfoModel, new();

        /// <summary>
        /// 把密文字节解密为原始字节数组，并返回指定摘要模型。
        /// </summary>
        TEncryptDigestInfoModel DecryptBytesFromBytes<TEncryptDigestInfoModel>(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptBytesDigestInfoModel, new();

        /// <summary>
        /// 把字节数组加密到文件，并返回指定摘要模型。
        /// </summary>
        TEncryptDigestInfoModel EncryptBytesToFile<TEncryptDigestInfoModel>(byte[] sourceBytes, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringFileDigestInfoModel, new();

        /// <summary>
        /// 从加密文件解密出字节数组，并返回指定摘要模型。
        /// </summary>
        TEncryptDigestInfoModel DecryptBytesFromFile<TEncryptDigestInfoModel>(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringFileDigestInfoModel, new();
    }
}
