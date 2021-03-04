using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CryptoModels;
using Lanymy.Common.Instruments.Interfaces;

namespace Lanymy.Common.Instruments
{

    public abstract class BaseCrypto : ICrypto
    {
        public abstract EncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedStream(Stream encryptedStream, Encoding encoding = null);
        public abstract EncryptDigestInfoModel DencryptStreamFromStream(Stream encryptedStream, Stream sourceStream, string secretKey = null, Encoding encoding = null);
        public abstract EncryptDigestInfoModel EncryptStreamToStream(Stream sourceStream, Stream encryptStream, string secretKey = null, bool ifRandom = true, Encoding encoding = null);

        public abstract TEncryptDigestInfoModel EncryptStreamToStream<TEncryptDigestInfoModel>(Stream sourceStream, Stream encryptStream, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptDigestInfoModel, new();
        public abstract TEncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedStream<TEncryptDigestInfoModel>(Stream encryptedStream, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptDigestInfoModel, new();
        public abstract TEncryptDigestInfoModel DencryptStreamFromStream<TEncryptDigestInfoModel>(Stream encryptedStream, Stream sourceStream, string secretKey = null, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptDigestInfoModel, new();
        public abstract EncryptBytesDigestInfoModel EncryptBytesToBytes(byte[] bytesToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null);
        public abstract EncryptBytesDigestInfoModel DecryptBytesFromBytes(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null);
        public abstract EncryptStringFileDigestInfoModel EncryptBytesToFile(byte[] sourceBytes, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null);
        public abstract EncryptStringFileDigestInfoModel DecryptBytesFromFile(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null);
        public abstract TEncryptDigestInfoModel EncryptBytesToBytes<TEncryptDigestInfoModel>(byte[] bytesToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptBytesDigestInfoModel, new();
        public abstract TEncryptDigestInfoModel DecryptBytesFromBytes<TEncryptDigestInfoModel>(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptBytesDigestInfoModel, new();
        public abstract TEncryptDigestInfoModel EncryptBytesToFile<TEncryptDigestInfoModel>(byte[] sourceBytes, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptStringFileDigestInfoModel, new();
        public abstract TEncryptDigestInfoModel DecryptBytesFromFile<TEncryptDigestInfoModel>(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptStringFileDigestInfoModel, new();
        public abstract EncryptStringDigestInfoModel EncryptStringToBytes(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null);
        public abstract EncryptStringDigestInfoModel DecryptStringFromBytes(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null);
        public abstract EncryptBase64StringDigestInfoModel EncryptStringToBase64String(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null);
        public abstract EncryptBase64StringDigestInfoModel DecryptStringFromBase64String(string base64StringToDecrypt, string secretKey = null, Encoding encoding = null);
        public abstract EncryptStringFileDigestInfoModel EncryptStringToFile(string sourceString, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null);
        public abstract EncryptStringFileDigestInfoModel DecryptStringFromFile(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null);
        public abstract TEncryptDigestInfoModel EncryptStringToBytes<TEncryptDigestInfoModel>(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptStringDigestInfoModel, new();
        public abstract TEncryptDigestInfoModel DecryptStringFromBytes<TEncryptDigestInfoModel>(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptStringDigestInfoModel, new();
        public abstract TEncryptDigestInfoModel EncryptStringToBase64String<TEncryptDigestInfoModel>(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptBase64StringDigestInfoModel, new();
        public abstract TEncryptDigestInfoModel DecryptStringFromBase64String<TEncryptDigestInfoModel>(string base64StringToDecrypt, string secretKey = null, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptBase64StringDigestInfoModel, new();
        public abstract TEncryptDigestInfoModel EncryptStringToFile<TEncryptDigestInfoModel>(string sourceString, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptStringFileDigestInfoModel, new();
        public abstract TEncryptDigestInfoModel DecryptStringFromFile<TEncryptDigestInfoModel>(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptStringFileDigestInfoModel, new();
        public abstract EncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedFile(string encryptedFileFullPath, Encoding encoding = null);
        public abstract EncryptStringFileDigestInfoModel EncryptFileToFile(string sourceFileFullPath, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null);
        public abstract EncryptStringFileDigestInfoModel DecryptFileFromFile(string encryptedFileFullPath, string sourceFileFullPath, string secretKey = null, Encoding encoding = null);
        public abstract TEncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedFile<TEncryptDigestInfoModel>(string encryptedFileFullPath, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptDigestInfoModel, new();
        public abstract EncryptStringBitmapDigestInfoModel EncryptBytesToBitmap(byte[] bytesToEncrypt, string secretKey = null, Encoding encoding = null);
        public abstract EncryptStringBitmapDigestInfoModel DecryptBytesFromBitmap(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null);
        public abstract EncryptStringBitmapDigestInfoModel EncryptStringToBitmap(string strToEncrypt, string secretKey = null, Encoding encoding = null);
        public abstract EncryptStringBitmapDigestInfoModel DecryptStringFromBitmap(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null);
        public abstract EncryptModelBitmapDigestInfoModel<T> EncryptModelToBitmap<T>(T t, string secretKey = null, Encoding encoding = null) where T : class;
        public abstract EncryptModelBitmapDigestInfoModel<T> DecryptModelFromBitmap<T>(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null) where T : class;
        public abstract EncryptStringImageFileDigestInfoModel EncryptBytesToImageFile(byte[] bytesToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null);
        public abstract EncryptStringImageFileDigestInfoModel DecryptBytesFromImageFile(string imageFileFullPath, string secretKey = null, Encoding encoding = null);
        public abstract EncryptStringImageFileDigestInfoModel EncryptStringToImageFile(string strToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null);
        public abstract EncryptStringImageFileDigestInfoModel DecryptStringFromImageFile(string imageFileFullPath, string secretKey = null, Encoding encoding = null);
        public abstract EncryptModelImageFileDigestInfoModel<T> EncryptModelToImageFile<T>(T t, string imageFileFullPath, string secretKey = null, Encoding encoding = null) where T : class;
        public abstract EncryptModelImageFileDigestInfoModel<T> DecryptModelFromImageFile<T>(string imageFileFullPath, string secretKey = null, Encoding encoding = null) where T : class;
        public abstract TEncryptDigestInfoModel EncryptBytesToBitmap<TEncryptDigestInfoModel>(byte[] bytesToEncrypt, string secretKey = null, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptStringBitmapDigestInfoModel, new();
        public abstract TEncryptDigestInfoModel DecryptBytesFromBitmap<TEncryptDigestInfoModel>(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptStringBitmapDigestInfoModel, new();
        public abstract TEncryptDigestInfoModel EncryptStringToBitmap<TEncryptDigestInfoModel>(string strToEncrypt, string secretKey = null, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptStringBitmapDigestInfoModel, new();
        public abstract TEncryptDigestInfoModel DecryptStringFromBitmap<TEncryptDigestInfoModel>(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptStringBitmapDigestInfoModel, new();
        public abstract TEncryptDigestInfoModel EncryptBytesToImageFile<TEncryptDigestInfoModel>(byte[] bytesToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptStringImageFileDigestInfoModel, new();
        public abstract TEncryptDigestInfoModel DecryptBytesFromImageFile<TEncryptDigestInfoModel>(string imageFileFullPath, string secretKey = null, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptStringImageFileDigestInfoModel, new();
        public abstract TEncryptDigestInfoModel EncryptStringToImageFile<TEncryptDigestInfoModel>(string strToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptStringImageFileDigestInfoModel, new();
        public abstract TEncryptDigestInfoModel DecryptStringFromImageFile<TEncryptDigestInfoModel>(string imageFileFullPath, string secretKey = null, Encoding encoding = null) where TEncryptDigestInfoModel : EncryptStringImageFileDigestInfoModel, new();
        public abstract EncryptModelDigestInfoModel<T> EncryptModelToBytes<T>(T t, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where T : class;
        public abstract EncryptModelDigestInfoModel<T> DecryptModelFromBytes<T>(byte[] ecryptedBytes, string secretKey = null, Encoding encoding = null) where T : class;
        public abstract EncryptModelDigestInfoModel<T> EncryptModelToBase64String<T>(T t, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where T : class;
        public abstract EncryptModelDigestInfoModel<T> DecryptModelFromBase64String<T>(string encryptBase64String, string secretKey = null, Encoding encoding = null) where T : class;
        public abstract EncryptModelFileDigestInfoModel<T> EncryptModelToFile<T>(T t, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where T : class;
        public abstract EncryptModelFileDigestInfoModel<T> DecryptModelFromFile<T>(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null) where T : class;
    }

}
