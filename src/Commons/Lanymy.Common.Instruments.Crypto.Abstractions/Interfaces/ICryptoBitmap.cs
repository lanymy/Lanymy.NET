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
    public interface ICryptoBitmap
    {

#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        EncryptStringBitmapDigestInfoModel EncryptBytesToBitmap(byte[] bytesToEncrypt, string secretKey = null, Encoding encoding = null);



#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        EncryptStringBitmapDigestInfoModel DecryptBytesFromBitmap(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null);



#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        EncryptStringBitmapDigestInfoModel EncryptStringToBitmap(string strToEncrypt, string secretKey = null, Encoding encoding = null);



#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        EncryptStringBitmapDigestInfoModel DecryptStringFromBitmap(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null);






#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        EncryptModelBitmapDigestInfoModel<T> EncryptModelToBitmap<T>(T t, string secretKey = null, Encoding encoding = null) where T : class;


#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        EncryptModelBitmapDigestInfoModel<T> DecryptModelFromBitmap<T>(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null) where T : class;











#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        EncryptStringImageFileDigestInfoModel EncryptBytesToImageFile(byte[] bytesToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null);




#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        EncryptStringImageFileDigestInfoModel DecryptBytesFromImageFile(string imageFileFullPath, string secretKey = null, Encoding encoding = null);





#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        EncryptStringImageFileDigestInfoModel EncryptStringToImageFile(string strToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null);






#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        EncryptStringImageFileDigestInfoModel DecryptStringFromImageFile(string imageFileFullPath, string secretKey = null, Encoding encoding = null);




#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        EncryptModelImageFileDigestInfoModel<T> EncryptModelToImageFile<T>(T t, string imageFileFullPath, string secretKey = null, Encoding encoding = null) where T : class;


#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        EncryptModelImageFileDigestInfoModel<T> DecryptModelFromImageFile<T>(string imageFileFullPath, string secretKey = null, Encoding encoding = null) where T : class;



    }
}
