using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
#if NET8_0_OR_GREATER
using System.Runtime.Versioning;
#endif
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{
    public interface ICryptoBitmapCore
    {


#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        TEncryptDigestInfoModel EncryptBytesToBitmap<TEncryptDigestInfoModel>(byte[] bytesToEncrypt, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringBitmapDigestInfoModel, new();

#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        TEncryptDigestInfoModel DecryptBytesFromBitmap<TEncryptDigestInfoModel>(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringBitmapDigestInfoModel, new();

#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        TEncryptDigestInfoModel EncryptStringToBitmap<TEncryptDigestInfoModel>(string strToEncrypt, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringBitmapDigestInfoModel, new();

        /// <summary>
        /// Decrypts the string from bitmap.
        /// </summary>
        /// <param name="encryptedBitmap">The encrypted bitmap.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentException">不是有效的加密位图数据源</exception>
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        TEncryptDigestInfoModel DecryptStringFromBitmap<TEncryptDigestInfoModel>(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringBitmapDigestInfoModel, new();


        /// <summary>
        /// Encrypts the bytes to image file.
        /// </summary>
        /// <param name="bytesToEncrypt">The bytes to encrypt.</param>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        TEncryptDigestInfoModel EncryptBytesToImageFile<TEncryptDigestInfoModel>(byte[] bytesToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringImageFileDigestInfoModel, new();

        /// <summary>
        /// Decrypts the bytes from image file.
        /// </summary>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>System.Byte[].</returns>
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        TEncryptDigestInfoModel DecryptBytesFromImageFile<TEncryptDigestInfoModel>(string imageFileFullPath, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringImageFileDigestInfoModel, new();

        /// <summary>
        /// Encrypts the string to image file.
        /// </summary>
        /// <param name="strToEncrypt">The string to encrypt.</param>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        TEncryptDigestInfoModel EncryptStringToImageFile<TEncryptDigestInfoModel>(string strToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringImageFileDigestInfoModel, new();

        /// <summary>
        /// Decrypts the string from image file.
        /// </summary>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>System.String.</returns>
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        TEncryptDigestInfoModel DecryptStringFromImageFile<TEncryptDigestInfoModel>(string imageFileFullPath, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringImageFileDigestInfoModel, new();

    }
}
