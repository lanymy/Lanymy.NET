using System.Drawing;
#if NET8_0_OR_GREATER
using System.Runtime.Versioning;
#endif
using System.Text;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{
    /// <summary>
    /// 定义位图和图片文件介质上的加解密入口。
    /// </summary>
    public interface ICryptoBitmap
    {
#if NET8_0_OR_GREATER
        /// <summary>
        /// 把字节数组加密为位图。
        /// </summary>
        [SupportedOSPlatform("windows")]
#endif
        EncryptStringBitmapDigestInfoModel EncryptBytesToBitmap(byte[] bytesToEncrypt, string secretKey = null, Encoding encoding = null);

#if NET8_0_OR_GREATER
        /// <summary>
        /// 把加密位图解密为字节数组。
        /// </summary>
        [SupportedOSPlatform("windows")]
#endif
        EncryptStringBitmapDigestInfoModel DecryptBytesFromBitmap(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null);

#if NET8_0_OR_GREATER
        /// <summary>
        /// 把字符串加密为位图。
        /// </summary>
        [SupportedOSPlatform("windows")]
#endif
        EncryptStringBitmapDigestInfoModel EncryptStringToBitmap(string strToEncrypt, string secretKey = null, Encoding encoding = null);

#if NET8_0_OR_GREATER
        /// <summary>
        /// 把加密位图解密为字符串。
        /// </summary>
        [SupportedOSPlatform("windows")]
#endif
        EncryptStringBitmapDigestInfoModel DecryptStringFromBitmap(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null);

#if NET8_0_OR_GREATER
        /// <summary>
        /// 把模型序列化并加密为位图。
        /// </summary>
        [SupportedOSPlatform("windows")]
#endif
        EncryptModelBitmapDigestInfoModel<T> EncryptModelToBitmap<T>(T t, string secretKey = null, Encoding encoding = null) where T : class;

#if NET8_0_OR_GREATER
        /// <summary>
        /// 把加密位图解密并反序列化为模型。
        /// </summary>
        [SupportedOSPlatform("windows")]
#endif
        EncryptModelBitmapDigestInfoModel<T> DecryptModelFromBitmap<T>(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null) where T : class;

#if NET8_0_OR_GREATER
        /// <summary>
        /// 把字节数组加密并保存为图片文件。
        /// </summary>
        [SupportedOSPlatform("windows")]
#endif
        EncryptStringImageFileDigestInfoModel EncryptBytesToImageFile(byte[] bytesToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null);

#if NET8_0_OR_GREATER
        /// <summary>
        /// 从加密图片文件解密出字节数组。
        /// </summary>
        [SupportedOSPlatform("windows")]
#endif
        EncryptStringImageFileDigestInfoModel DecryptBytesFromImageFile(string imageFileFullPath, string secretKey = null, Encoding encoding = null);

#if NET8_0_OR_GREATER
        /// <summary>
        /// 把字符串加密并保存为图片文件。
        /// </summary>
        [SupportedOSPlatform("windows")]
#endif
        EncryptStringImageFileDigestInfoModel EncryptStringToImageFile(string strToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null);

#if NET8_0_OR_GREATER
        /// <summary>
        /// 从加密图片文件解密出字符串。
        /// </summary>
        [SupportedOSPlatform("windows")]
#endif
        EncryptStringImageFileDigestInfoModel DecryptStringFromImageFile(string imageFileFullPath, string secretKey = null, Encoding encoding = null);

#if NET8_0_OR_GREATER
        /// <summary>
        /// 把模型序列化并加密为图片文件。
        /// </summary>
        [SupportedOSPlatform("windows")]
#endif
        EncryptModelImageFileDigestInfoModel<T> EncryptModelToImageFile<T>(T t, string imageFileFullPath, string secretKey = null, Encoding encoding = null) where T : class;

#if NET8_0_OR_GREATER
        /// <summary>
        /// 从加密图片文件解密并反序列化模型。
        /// </summary>
        [SupportedOSPlatform("windows")]
#endif
        EncryptModelImageFileDigestInfoModel<T> DecryptModelFromImageFile<T>(string imageFileFullPath, string secretKey = null, Encoding encoding = null) where T : class;
    }
}
