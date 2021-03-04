using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{
    public interface ICryptoBitmap
    {

        EncryptStringBitmapDigestInfoModel EncryptBytesToBitmap(byte[] bytesToEncrypt, string secretKey = null, Encoding encoding = null);



        EncryptStringBitmapDigestInfoModel DecryptBytesFromBitmap(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null);



        EncryptStringBitmapDigestInfoModel EncryptStringToBitmap(string strToEncrypt, string secretKey = null, Encoding encoding = null);



        EncryptStringBitmapDigestInfoModel DecryptStringFromBitmap(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null);






        EncryptModelBitmapDigestInfoModel<T> EncryptModelToBitmap<T>(T t, string secretKey = null, Encoding encoding = null) where T : class;


        EncryptModelBitmapDigestInfoModel<T> DecryptModelFromBitmap<T>(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null) where T : class;











        EncryptStringImageFileDigestInfoModel EncryptBytesToImageFile(byte[] bytesToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null);




        EncryptStringImageFileDigestInfoModel DecryptBytesFromImageFile(string imageFileFullPath, string secretKey = null, Encoding encoding = null);





        EncryptStringImageFileDigestInfoModel EncryptStringToImageFile(string strToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null);






        EncryptStringImageFileDigestInfoModel DecryptStringFromImageFile(string imageFileFullPath, string secretKey = null, Encoding encoding = null);




        EncryptModelImageFileDigestInfoModel<T> EncryptModelToImageFile<T>(T t, string imageFileFullPath, string secretKey = null, Encoding encoding = null) where T : class;


        EncryptModelImageFileDigestInfoModel<T> DecryptModelFromImageFile<T>(string imageFileFullPath, string secretKey = null, Encoding encoding = null) where T : class;



    }
}
