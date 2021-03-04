using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{
    public interface ICryptoBitmapCore
    {


        TEncryptDigestInfoModel EncryptBytesToBitmap<TEncryptDigestInfoModel>(byte[] bytesToEncrypt, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringBitmapDigestInfoModel, new();

        TEncryptDigestInfoModel DecryptBytesFromBitmap<TEncryptDigestInfoModel>(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringBitmapDigestInfoModel, new();

        TEncryptDigestInfoModel EncryptStringToBitmap<TEncryptDigestInfoModel>(string strToEncrypt, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringBitmapDigestInfoModel, new();

        /// <summary>
        /// Decrypts the string from bitmap.
        /// </summary>
        /// <param name="decryptBitmap">The decrypt bitmap.</param>
        /// <param name="isDecryptStringFromBase64String">是否 从 Base64String 字符串中 解密 出 原始字符串</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentException">不是有效的加密位图数据源</exception>
        TEncryptDigestInfoModel DecryptStringFromBitmap<TEncryptDigestInfoModel>(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringBitmapDigestInfoModel, new();


        /// <summary>
        /// Encrypts the bytes to image file.
        /// </summary>
        /// <param name="bytesToEncrypt">The bytes to encrypt.</param>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        TEncryptDigestInfoModel EncryptBytesToImageFile<TEncryptDigestInfoModel>(byte[] bytesToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringImageFileDigestInfoModel, new();

        /// <summary>
        /// Decrypts the bytes from image file.
        /// </summary>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <returns>System.Byte[].</returns>
        TEncryptDigestInfoModel DecryptBytesFromImageFile<TEncryptDigestInfoModel>(string imageFileFullPath, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringImageFileDigestInfoModel, new();

        /// <summary>
        /// Encrypts the string to image file.
        /// </summary>
        /// <param name="strToEncrypt">The string to encrypt.</param>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        TEncryptDigestInfoModel EncryptStringToImageFile<TEncryptDigestInfoModel>(string strToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringImageFileDigestInfoModel, new();

        /// <summary>
        /// Decrypts the string from image file.
        /// </summary>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <returns>System.String.</returns>
        TEncryptDigestInfoModel DecryptStringFromImageFile<TEncryptDigestInfoModel>(string imageFileFullPath, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringImageFileDigestInfoModel, new();

    }
}
