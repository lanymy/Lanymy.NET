using System.Drawing;
#if NET8_0_OR_GREATER
using System.Runtime.Versioning;
#endif
using System.Text;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{
    /// <summary>
    /// 定义返回具体摘要模型的位图与图片文件加解密泛型入口。
    /// </summary>
    public interface ICryptoBitmapCore
    {
        /// <summary>
        /// 把字节数组加密为位图，并返回指定摘要模型。
        /// </summary>
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        TEncryptDigestInfoModel EncryptBytesToBitmap<TEncryptDigestInfoModel>(byte[] bytesToEncrypt, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringBitmapDigestInfoModel, new();

        /// <summary>
        /// 把位图解密为字节数组，并返回指定摘要模型。
        /// </summary>
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        TEncryptDigestInfoModel DecryptBytesFromBitmap<TEncryptDigestInfoModel>(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringBitmapDigestInfoModel, new();

        /// <summary>
        /// 把字符串加密为位图，并返回指定摘要模型。
        /// </summary>
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        TEncryptDigestInfoModel EncryptStringToBitmap<TEncryptDigestInfoModel>(string strToEncrypt, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringBitmapDigestInfoModel, new();

        /// <summary>
        /// 把加密位图解密为字符串，并返回指定摘要模型。
        /// </summary>
        /// <exception cref="System.ArgumentException">不是有效的加密位图数据源。</exception>
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        TEncryptDigestInfoModel DecryptStringFromBitmap<TEncryptDigestInfoModel>(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringBitmapDigestInfoModel, new();

        /// <summary>
        /// 把字节数组加密为图片文件，并返回指定摘要模型。
        /// </summary>
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        TEncryptDigestInfoModel EncryptBytesToImageFile<TEncryptDigestInfoModel>(byte[] bytesToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringImageFileDigestInfoModel, new();

        /// <summary>
        /// 从加密图片文件解密出字节数组，并返回指定摘要模型。
        /// </summary>
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        TEncryptDigestInfoModel DecryptBytesFromImageFile<TEncryptDigestInfoModel>(string imageFileFullPath, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringImageFileDigestInfoModel, new();

        /// <summary>
        /// 把字符串加密为图片文件，并返回指定摘要模型。
        /// </summary>
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        TEncryptDigestInfoModel EncryptStringToImageFile<TEncryptDigestInfoModel>(string strToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringImageFileDigestInfoModel, new();

        /// <summary>
        /// 从加密图片文件解密出字符串，并返回指定摘要模型。
        /// </summary>
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        TEncryptDigestInfoModel DecryptStringFromImageFile<TEncryptDigestInfoModel>(string imageFileFullPath, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringImageFileDigestInfoModel, new();
    }
}
