using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
#if NET8_0_OR_GREATER
using System.Runtime.Versioning;
#endif
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.Enums;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Helpers.ResultModels;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.CryptoModels;
using Lanymy.Common.Instruments.Interfaces;

namespace Lanymy.Common.Helpers
{




    /// <summary>
    /// 安全加密类 辅助方法
    /// </summary>
    public class SecurityHelper
    {



        public static readonly ICrypto DefaultLanymyCrypto = new LanymyCrypto();

        #region Stream

        public static EncryptDigestInfoModel EncryptStreamToStream(Stream sourceStream, Stream encryptStream, string secretKey = null, bool ifRandom = true, Encoding encoding = null, ICryptoStream cryptoStream = null)
        {
            return GenericityHelper.GetInterface(cryptoStream, DefaultLanymyCrypto).EncryptStreamToStream(sourceStream, encryptStream, secretKey, ifRandom, encoding);
        }

        public static EncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedStream(Stream encryptedStream, Encoding encoding = null, ICryptoStream cryptoStream = null)
        {
            return GenericityHelper.GetInterface(cryptoStream, DefaultLanymyCrypto).GetEncryptDigestInfoModelFromEncryptedStream(encryptedStream, encoding);
        }


        public static EncryptDigestInfoModel DencryptStreamFromStream(Stream encryptedStream, Stream sourceStream, string secretKey = null, Encoding encoding = null, ICryptoStream cryptoStream = null)
        {
            return GenericityHelper.GetInterface(cryptoStream, DefaultLanymyCrypto).DencryptStreamFromStream(encryptedStream, sourceStream, secretKey, encoding);
        }


        #endregion


        #region Bytes


        public static EncryptBytesDigestInfoModel EncryptBytesToBytes(byte[] bytesToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null, ICryptoBytes cryptoBytes = null)
        {
            return GenericityHelper.GetInterface(cryptoBytes, DefaultLanymyCrypto).EncryptBytesToBytes(bytesToEncrypt, secretKey, ifRandom, encoding);
        }

        public static EncryptBytesDigestInfoModel DecryptBytesFromBytes(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null, ICryptoBytes cryptoBytes = null)
        {
            return GenericityHelper.GetInterface(cryptoBytes, DefaultLanymyCrypto).DecryptBytesFromBytes(bytesToDecrypt, secretKey, encoding);
        }

        public static EncryptStringFileDigestInfoModel EncryptBytesToFile(byte[] sourceBytes, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null, ICryptoBytes cryptoBytes = null)
        {
            return GenericityHelper.GetInterface(cryptoBytes, DefaultLanymyCrypto).EncryptBytesToFile(sourceBytes, encryptFileFullPath, secretKey, ifRandom, encoding);
        }

        public static EncryptStringFileDigestInfoModel DecryptBytesFromFile(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null, ICryptoBytes cryptoBytes = null)
        {
            return GenericityHelper.GetInterface(cryptoBytes, DefaultLanymyCrypto).DecryptBytesFromFile(encryptedFileFullPath, secretKey, encoding);
        }

        #endregion



        #region String


        public static EncryptStringDigestInfoModel EncryptStringToBytes(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null, ICryptoString cryptoString = null)
        {
            return GenericityHelper.GetInterface(cryptoString, DefaultLanymyCrypto).EncryptStringToBytes(strToEncrypt, secretKey, ifRandom, encoding);
        }

        public static EncryptStringDigestInfoModel DecryptStringFromBytes(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null, ICryptoString cryptoString = null)
        {
            return GenericityHelper.GetInterface(cryptoString, DefaultLanymyCrypto).DecryptStringFromBytes(bytesToDecrypt, secretKey, encoding);
        }



        public static EncryptBase64StringDigestInfoModel EncryptStringToBase64String(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null, ICryptoString cryptoString = null)
        {
            return GenericityHelper.GetInterface(cryptoString, DefaultLanymyCrypto).EncryptStringToBase64String(strToEncrypt, secretKey, ifRandom, encoding);
        }



        public static EncryptBase64StringDigestInfoModel DecryptStringFromBase64String(string base64StringToDecrypt, string secretKey = null, Encoding encoding = null, ICryptoString cryptoString = null)
        {
            return GenericityHelper.GetInterface(cryptoString, DefaultLanymyCrypto).DecryptStringFromBase64String(base64StringToDecrypt, secretKey, encoding);
        }



        public static EncryptStringFileDigestInfoModel EncryptStringToFile(string sourceString, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null, ICryptoString cryptoString = null)
        {
            return GenericityHelper.GetInterface(cryptoString, DefaultLanymyCrypto).EncryptStringToFile(sourceString, encryptFileFullPath, secretKey, ifRandom, encoding);
        }




        public static EncryptStringFileDigestInfoModel DecryptStringFromFile(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null, ICryptoString cryptoString = null)
        {
            return GenericityHelper.GetInterface(cryptoString, DefaultLanymyCrypto).DecryptStringFromFile(encryptedFileFullPath, secretKey, encoding);
        }


        #endregion


        #region File

        public static EncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedFile(string encryptedFileFullPath, Encoding encoding = null, ICryptoFile cryptoFile = null)
        {
            return GenericityHelper.GetInterface(cryptoFile, DefaultLanymyCrypto).GetEncryptDigestInfoModelFromEncryptedFile(encryptedFileFullPath, encoding);
        }


        public static EncryptStringFileDigestInfoModel EncryptFileToFile(string sourceFileFullPath, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null, ICryptoFile cryptoFile = null)
        {
            return GenericityHelper.GetInterface(cryptoFile, DefaultLanymyCrypto).EncryptFileToFile(sourceFileFullPath, encryptFileFullPath, secretKey, ifRandom, encoding);
        }


        public static EncryptStringFileDigestInfoModel DecryptFileFromFile(string encryptedFileFullPath, string sourceFileFullPath, string secretKey = null, Encoding encoding = null, ICryptoFile cryptoFile = null)
        {
            return GenericityHelper.GetInterface(cryptoFile, DefaultLanymyCrypto).DecryptFileFromFile(encryptedFileFullPath, sourceFileFullPath, secretKey, encoding);
        }


        #endregion


        #region Bitmap


#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public static EncryptStringBitmapDigestInfoModel EncryptBytesToBitmap(byte[] bytesToEncrypt, string secretKey = null, Encoding encoding = null, ICryptoBitmap cryptoBitmap = null)
        {
            return GenericityHelper.GetInterface(cryptoBitmap, DefaultLanymyCrypto).EncryptBytesToBitmap(bytesToEncrypt, secretKey, encoding);
        }


#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public static EncryptStringBitmapDigestInfoModel DecryptBytesFromBitmap(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null, ICryptoBitmap cryptoBitmap = null)
        {
            return GenericityHelper.GetInterface(cryptoBitmap, DefaultLanymyCrypto).DecryptBytesFromBitmap(encryptedBitmap, secretKey, encoding);
        }


#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public static EncryptStringBitmapDigestInfoModel EncryptStringToBitmap(string strToEncrypt, string secretKey = null, Encoding encoding = null, ICryptoBitmap cryptoBitmap = null)
        {
            return GenericityHelper.GetInterface(cryptoBitmap, DefaultLanymyCrypto).EncryptStringToBitmap(strToEncrypt, secretKey, encoding);
        }


#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public static EncryptStringBitmapDigestInfoModel DecryptStringFromBitmap(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null, ICryptoBitmap cryptoBitmap = null)

        {
            return GenericityHelper.GetInterface(cryptoBitmap, DefaultLanymyCrypto).DecryptStringFromBitmap(encryptedBitmap, secretKey, encoding);
        }




#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public static EncryptModelBitmapDigestInfoModel<T> EncryptModelToBitmap<T>(T t, string secretKey = null, Encoding encoding = null, ICryptoBitmap cryptoBitmap = null) where T : class
        {
            return GenericityHelper.GetInterface(cryptoBitmap, DefaultLanymyCrypto).EncryptModelToBitmap(t, secretKey, encoding);
        }

#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public static EncryptModelBitmapDigestInfoModel<T> DecryptModelFromBitmap<T>(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null, ICryptoBitmap cryptoBitmap = null) where T : class
        {
            return GenericityHelper.GetInterface(cryptoBitmap, DefaultLanymyCrypto).DecryptModelFromBitmap<T>(encryptedBitmap, secretKey, encoding);
        }










#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public static EncryptStringImageFileDigestInfoModel EncryptBytesToImageFile(byte[] bytesToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null, ICryptoBitmap cryptoBitmap = null)
        {
            return GenericityHelper.GetInterface(cryptoBitmap, DefaultLanymyCrypto).EncryptBytesToImageFile(bytesToEncrypt, imageFileFullPath, secretKey, encoding);
        }



#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public static EncryptStringImageFileDigestInfoModel DecryptBytesFromImageFile(string imageFileFullPath, string secretKey = null, Encoding encoding = null, ICryptoBitmap cryptoBitmap = null)
        {
            return GenericityHelper.GetInterface(cryptoBitmap, DefaultLanymyCrypto).DecryptBytesFromImageFile(imageFileFullPath, secretKey, encoding);
        }




#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public static EncryptStringImageFileDigestInfoModel EncryptStringToImageFile(string strToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null, ICryptoBitmap cryptoBitmap = null)
        {
            return GenericityHelper.GetInterface(cryptoBitmap, DefaultLanymyCrypto).EncryptStringToImageFile(strToEncrypt, imageFileFullPath, secretKey, encoding);
        }





#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public static EncryptStringImageFileDigestInfoModel DecryptStringFromImageFile(string imageFileFullPath, string secretKey = null, Encoding encoding = null, ICryptoBitmap cryptoBitmap = null)
        {
            return GenericityHelper.GetInterface(cryptoBitmap, DefaultLanymyCrypto).DecryptStringFromImageFile(imageFileFullPath, secretKey, encoding);
        }



#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public static EncryptModelImageFileDigestInfoModel<T> EncryptModelToImageFile<T>(T t, string imageFileFullPath, string secretKey = null, Encoding encoding = null, ICryptoBitmap cryptoBitmap = null) where T : class
        {
            return GenericityHelper.GetInterface(cryptoBitmap, DefaultLanymyCrypto).EncryptModelToImageFile(t, imageFileFullPath, secretKey, encoding);
        }

#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public static EncryptModelImageFileDigestInfoModel<T> DecryptModelFromImageFile<T>(string imageFileFullPath, string secretKey = null, Encoding encoding = null, ICryptoBitmap cryptoBitmap = null) where T : class
        {
            return GenericityHelper.GetInterface(cryptoBitmap, DefaultLanymyCrypto).DecryptModelFromImageFile<T>(imageFileFullPath, secretKey, encoding);
        }


        #endregion


        #region Model



        public static EncryptModelDigestInfoModel<T> EncryptModelToBytes<T>(T t, string secretKey = null, bool ifRandom = true, Encoding encoding = null, ICryptoModel cryptoModel = null) where T : class
        {
            return GenericityHelper.GetInterface(cryptoModel, DefaultLanymyCrypto).EncryptModelToBytes(t, secretKey, ifRandom, encoding);
        }


        public static EncryptModelDigestInfoModel<T> DecryptModelFromBytes<T>(byte[] ecryptedBytes, string secretKey = null, Encoding encoding = null, ICryptoModel cryptoModel = null) where T : class
        {
            return GenericityHelper.GetInterface(cryptoModel, DefaultLanymyCrypto).DecryptModelFromBytes<T>(ecryptedBytes, secretKey, encoding);
        }


        public static EncryptModelDigestInfoModel<T> EncryptModelToBase64String<T>(T t, string secretKey = null, bool ifRandom = true, Encoding encoding = null, ICryptoModel cryptoModel = null) where T : class
        {
            return GenericityHelper.GetInterface(cryptoModel, DefaultLanymyCrypto).EncryptModelToBase64String(t, secretKey, ifRandom, encoding);
        }


        public static EncryptModelDigestInfoModel<T> DecryptModelFromBase64String<T>(string encryptBase64String, string secretKey = null, Encoding encoding = null, ICryptoModel cryptoModel = null) where T : class
        {
            return GenericityHelper.GetInterface(cryptoModel, DefaultLanymyCrypto).DecryptModelFromBase64String<T>(encryptBase64String, secretKey, encoding);
        }


        public static EncryptModelFileDigestInfoModel<T> EncryptModelToFile<T>(T t, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null, ICryptoModel cryptoModel = null) where T : class
        {
            return GenericityHelper.GetInterface(cryptoModel, DefaultLanymyCrypto).EncryptModelToFile(t, encryptFileFullPath, secretKey, ifRandom, encoding);
        }

        public static EncryptModelFileDigestInfoModel<T> DecryptModelFromFile<T>(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null, ICryptoModel cryptoModel = null) where T : class
        {
            return GenericityHelper.GetInterface(cryptoModel, DefaultLanymyCrypto).DecryptModelFromFile<T>(encryptedFileFullPath, secretKey, encoding);
        }


        #endregion






        #region MD5


        /// <summary>
        /// 字节数组转MD5
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToMD5(byte[] bytes)
        {

            if (bytes.IfIsNull()) throw new ArgumentNullException(nameof(bytes));

            using var md5 = MD5.Create();

            var hashedBytes = md5.ComputeHash(bytes);

            var sb = new StringBuilder();

            foreach (var b in hashedBytes)
            {
                //sb.Append(b.ToString("x2").ToLower());
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();

        }




        /// <summary>
        /// 字符串转成MD5
        /// </summary>
        /// <param name="str">要计算 MD5 的字符串。</param>
        /// <param name="encoding">字符串转字节时使用的编码，Null 表示使用默认编码。</param>
        /// <returns></returns>
        public static string StringToMD5(string str, Encoding encoding = null)
        {


            if (str.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(str));
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

            return BytesToMD5(encoding.GetBytes(str));

        }


        #endregion



        /// <summary>
        /// 动态加载证书 必须有管理员权限 才能调用成功
        /// </summary>
        /// <param name="rawData">证书二进制数据</param>
        public static void LoadCertificate(byte[] rawData)
        {
            var result = LoadCertificateWithResult(rawData);
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }

        }

        /// <summary>
        /// 动态加载证书，并返回详细结果。
        /// </summary>
        /// <param name="rawData">证书二进制数据。</param>
        /// <param name="storeLocation">证书存储位置，默认写入 LocalMachine。</param>
        /// <param name="trustedStoreName">受信任证书存储名称，默认写入 Root。</param>
        /// <param name="personalStoreName">个人证书存储名称，默认写入 My。</param>
        /// <returns>证书加载结果。</returns>
        public static SecurityCertificateLoadResultModel LoadCertificateWithResult(byte[] rawData, StoreLocation storeLocation = StoreLocation.LocalMachine, StoreName trustedStoreName = StoreName.Root, StoreName personalStoreName = StoreName.My)
        {
            var result = CreateCertificateLoadResultModel(storeLocation, null, trustedStoreName, personalStoreName);

            try
            {
                if (rawData.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(rawData));

                using var certificate = new X509Certificate2(rawData);
                result = LoadCertificateWithResult(certificate, storeLocation, trustedStoreName, personalStoreName);
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }


        /// <summary>
        /// 动态加载证书 必须有管理员权限 才能调用成功
        /// </summary>
        /// <param name="rawData">证书二进制数据</param>
        /// <param name="password">证书密码</param>
        public static void LoadCertificate(byte[] rawData, string password)
        {
            var result = LoadCertificateWithResult(rawData, password);
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }
        }

        /// <summary>
        /// 动态加载带密码的证书，并返回详细结果。
        /// </summary>
        /// <param name="rawData">证书二进制数据。</param>
        /// <param name="password">证书密码。</param>
        /// <param name="storeLocation">证书存储位置，默认写入 LocalMachine。</param>
        /// <param name="trustedStoreName">受信任证书存储名称，默认写入 Root。</param>
        /// <param name="personalStoreName">个人证书存储名称，默认写入 My。</param>
        /// <returns>证书加载结果。</returns>
        public static SecurityCertificateLoadResultModel LoadCertificateWithResult(byte[] rawData, string password, StoreLocation storeLocation = StoreLocation.LocalMachine, StoreName trustedStoreName = StoreName.Root, StoreName personalStoreName = StoreName.My)
        {
            var result = CreateCertificateLoadResultModel(storeLocation, null, trustedStoreName, personalStoreName);

            try
            {
                if (rawData.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(rawData));
                if (password.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(password));

                using var certificate = new X509Certificate2(rawData, password);
                result = LoadCertificateWithResult(certificate, storeLocation, trustedStoreName, personalStoreName);
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// 动态加载证书 必须有管理员权限 才能调用成功
        /// </summary>
        /// <param name="certFileFullPath">证书文件全路径</param>
        public static void LoadCertificate(string certFileFullPath)
        {
            var result = LoadCertificateWithResult(certFileFullPath);
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }
        }

        /// <summary>
        /// 从文件动态加载证书，并返回详细结果。
        /// </summary>
        /// <param name="certFileFullPath">证书文件全路径。</param>
        /// <param name="storeLocation">证书存储位置，默认写入 LocalMachine。</param>
        /// <param name="trustedStoreName">受信任证书存储名称，默认写入 Root。</param>
        /// <param name="personalStoreName">个人证书存储名称，默认写入 My。</param>
        /// <returns>证书加载结果。</returns>
        public static SecurityCertificateLoadResultModel LoadCertificateWithResult(string certFileFullPath, StoreLocation storeLocation = StoreLocation.LocalMachine, StoreName trustedStoreName = StoreName.Root, StoreName personalStoreName = StoreName.My)
        {
            var result = CreateCertificateLoadResultModel(storeLocation, certFileFullPath, trustedStoreName, personalStoreName);

            try
            {
                if (!File.Exists(certFileFullPath)) throw new FileNotFoundException(certFileFullPath);

                using var certificate = new X509Certificate2(certFileFullPath);
                result = LoadCertificateWithResult(certificate, storeLocation, trustedStoreName, personalStoreName);
                result.FilePath = certFileFullPath;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// 动态加载证书 必须有管理员权限 才能调用成功
        /// </summary>
        /// <param name="certFileFullPath">证书文件全路径</param>
        /// <param name="password">证书密码</param>
        public static void LoadCertificate(string certFileFullPath, string password)
        {
            var result = LoadCertificateWithResult(certFileFullPath, password);
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }
        }

        /// <summary>
        /// 从带密码的证书文件动态加载证书，并返回详细结果。
        /// </summary>
        /// <param name="certFileFullPath">证书文件全路径。</param>
        /// <param name="password">证书密码。</param>
        /// <param name="storeLocation">证书存储位置，默认写入 LocalMachine。</param>
        /// <param name="trustedStoreName">受信任证书存储名称，默认写入 Root。</param>
        /// <param name="personalStoreName">个人证书存储名称，默认写入 My。</param>
        /// <returns>证书加载结果。</returns>
        public static SecurityCertificateLoadResultModel LoadCertificateWithResult(string certFileFullPath, string password, StoreLocation storeLocation = StoreLocation.LocalMachine, StoreName trustedStoreName = StoreName.Root, StoreName personalStoreName = StoreName.My)
        {
            var result = CreateCertificateLoadResultModel(storeLocation, certFileFullPath, trustedStoreName, personalStoreName);

            try
            {
                if (!File.Exists(certFileFullPath)) throw new FileNotFoundException(certFileFullPath);
                if (password.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(password));

                using var certificate = new X509Certificate2(certFileFullPath, password);
                result = LoadCertificateWithResult(certificate, storeLocation, trustedStoreName, personalStoreName);
                result.FilePath = certFileFullPath;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// 动态加载证书 必须有管理员权限 才能调用成功
        /// </summary>
        /// <param name="certificate">证书实例</param>
        public static void LoadCertificate(X509Certificate2 certificate)
        {
            var result = LoadCertificateWithResult(certificate);
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }
        }

        /// <summary>
        /// 动态加载证书，并返回详细结果。
        /// </summary>
        /// <param name="certificate">证书实例。</param>
        /// <param name="storeLocation">证书存储位置，默认写入 LocalMachine。</param>
        /// <param name="trustedStoreName">受信任证书存储名称，默认写入 Root。</param>
        /// <param name="personalStoreName">个人证书存储名称，默认写入 My。</param>
        /// <returns>证书加载结果。</returns>
        public static SecurityCertificateLoadResultModel LoadCertificateWithResult(X509Certificate2 certificate, StoreLocation storeLocation = StoreLocation.LocalMachine, StoreName trustedStoreName = StoreName.Root, StoreName personalStoreName = StoreName.My)
        {
            var result = CreateCertificateLoadResultModel(storeLocation, null, trustedStoreName, personalStoreName);

            try
            {
                if (certificate == null) throw new ArgumentNullException(nameof(certificate));

                result.CertificateThumbprint = certificate.Thumbprint;
                result.CertificateSubject = certificate.Subject;

                using (X509Store trustedStore = new X509Store(trustedStoreName, storeLocation))
                {
                    trustedStore.Open(OpenFlags.ReadWrite);
                    result.ExistsInTrustedStore = ContainsCertificate(trustedStore, certificate);
                    if (!result.ExistsInTrustedStore)
                    {
                        trustedStore.Add(certificate);
                        result.AddedToTrustedStore = true;
                        result.ExistsInTrustedStore = ContainsCertificate(trustedStore, certificate);
                    }
                }

                using (X509Store personalStore = new X509Store(personalStoreName, storeLocation))
                {
                    personalStore.Open(OpenFlags.ReadWrite);
                    result.ExistsInPersonalStore = ContainsCertificate(personalStore, certificate);
                    if (!result.ExistsInPersonalStore)
                    {
                        personalStore.Add(certificate);
                        result.AddedToPersonalStore = true;
                        result.ExistsInPersonalStore = ContainsCertificate(personalStore, certificate);
                    }
                }

                result.IsSuccess = result.ExistsInTrustedStore && result.ExistsInPersonalStore;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        private static SecurityCertificateLoadResultModel CreateCertificateLoadResultModel(StoreLocation storeLocation, string filePath = null, StoreName trustedStoreName = StoreName.Root, StoreName personalStoreName = StoreName.My)
        {
            return new SecurityCertificateLoadResultModel
            {
                FilePath = filePath,
                StoreLocation = storeLocation,
                TrustedStoreName = trustedStoreName,
                PersonalStoreName = personalStoreName,
            };
        }

        private static bool ContainsCertificate(X509Store store, X509Certificate2 certificate)
        {
            return store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, false).Count > 0;
        }






        /// <summary>
        /// 加密 文件名 成 文件名合法的Base64字符串编码
        /// </summary>
        /// <param name="fileName">要加密的文件名</param>
        /// <param name="encryptFileExtension">加密后文件扩展名 默认 ".base64"</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static string EncryptFileNameToHashCodeString
        (
            string fileName,
            string encryptFileExtension = ".base64",
            string secretKey = null,
            bool ifRandom = true,
            Encoding encoding = null)
        {


            if (fileName.IfIsNullOrEmpty()) return string.Empty;
            var encryptStringDigestInfoModel = EncryptStringToBytes(fileName, secretKey, ifRandom, encoding);
            if (!encryptStringDigestInfoModel.IsSuccess)
            {
                return string.Empty;
            }

            return encryptStringDigestInfoModel.EncryptContentBytesHashCode + encryptFileExtension;


        }


        ///// <summary>
        ///// 解密 文件名合法的Base64加密字符串编码 成 原文件名
        ///// </summary>
        ///// <param name="encryptFileNameBase64String">文件名合法的Base64加密字符串编码</param>
        ///// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        ///// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        ///// <returns></returns>
        //public static string DecryptFileNameFromBase64String(string encryptFileNameBase64String, string secretKey = null, Encoding encoding = null)
        //{

        //    if (encryptFileNameBase64String.IfIsNullOrEmpty()) return string.Empty;

        //    if (Path.HasExtension(encryptFileNameBase64String))
        //        encryptFileNameBase64String = Path.GetFileNameWithoutExtension(encryptFileNameBase64String);


        //    return DecryptStringFromBase64String(FormatHelper.FormatBase64StringFromFileNameBase64String(encryptFileNameBase64String), secretKey, encoding);

        //}



        /// <summary>
        /// 加密 文件夹名称 成 文件夹名称 合法的Base64字符串编码
        /// </summary>
        /// <param name="directoryName">要加密的文件夹名称</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static string EncryptDirectoryNameToHashCodeString(string directoryName, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {
            return EncryptFileNameToHashCodeString(directoryName, string.Empty, secretKey, ifRandom, encoding);
        }


        ///// <summary>
        ///// 解密 文件夹名合法的Base64加密字符串编码 成 原文件夹名称
        ///// </summary>
        ///// <param name="directoryName">文件夹名合法的Base64加密字符串编码</param>
        ///// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        ///// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        ///// <returns></returns>
        //public static string DecryptDirectoryNameFromBase64String(string directoryName, string secretKey = null, Encoding encoding = null)
        //{
        //    return DecryptFileNameFromBase64String(directoryName, secretKey, encoding);
        //}




        /// <summary>
        /// 创建 RSA 的 Blob Base64String 形式 的 密钥 字符串
        /// </summary>
        /// <param name="keySizeType">密钥长度(加密级别) 1024/2048/3072/7680/15360</param>
        /// <param name="publicKeyBlobBase64String">公钥 Blob Base64String 形式 的 密钥 字符串</param>
        /// <param name="privateKeyBase64String">私钥 Blob Base64String 形式 的 密钥 字符串</param>
        public static void CreateRsaKeyBlobBase64String(RsaKeySizeTypeEnum keySizeType, out string publicKeyBlobBase64String, out string privateKeyBase64String)
        {
            var result = CreateRsaKeyBlobBase64StringWithResult(keySizeType);
            publicKeyBlobBase64String = result.PublicKeyBlobBase64String;
            privateKeyBase64String = result.PrivateKeyBlobBase64String;
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }
        }

        /// <summary>
        /// 创建 RSA 的 Blob Base64String 形式密钥，并返回详细结果。
        /// </summary>
        /// <param name="keySizeType">密钥长度。</param>
        /// <returns>RSA Blob Base64 字符串密钥结果。</returns>
        public static SecurityRsaKeyBlobStringResultModel CreateRsaKeyBlobBase64StringWithResult(RsaKeySizeTypeEnum keySizeType)
        {
            var result = new SecurityRsaKeyBlobStringResultModel
            {
                KeySizeType = keySizeType,
            };

            try
            {
                var blobBytesResult = CreateRsaKeyBlobBytesWithResult(keySizeType);
                if (!blobBytesResult.IsSuccess)
                {
                    result.Exception = blobBytesResult.Exception;
                    return result;
                }

                result.PublicKeyBlobBase64String = Convert.ToBase64String(blobBytesResult.PublicKeyBlobBytes);
                result.PrivateKeyBlobBase64String = Convert.ToBase64String(blobBytesResult.PrivateKeyBlobBytes);
                result.IsSuccess = !result.PublicKeyBlobBase64String.IfIsNullOrEmpty() && !result.PrivateKeyBlobBase64String.IfIsNullOrEmpty();
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// 创建 RSA 的 Blob 原始 二进制 数据 密钥
        /// </summary>
        /// <param name="keySizeType">密钥长度(加密级别) 1024/2048/3072/7680/15360</param>
        /// <param name="publicKeyBlobBytes">公钥 Blob 原始 二进制 数据</param>
        /// <param name="privateKeyBlobBytes">私钥 Blob 原始 二进制 数据</param>
        public static void CreateRsaKeyBlobBytes(RsaKeySizeTypeEnum keySizeType, out byte[] publicKeyBlobBytes, out byte[] privateKeyBlobBytes)
        {
            var result = CreateRsaKeyBlobBytesWithResult(keySizeType);
            publicKeyBlobBytes = result.PublicKeyBlobBytes;
            privateKeyBlobBytes = result.PrivateKeyBlobBytes;
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }
        }

        /// <summary>
        /// 创建 RSA 的 Blob 原始二进制密钥，并返回详细结果。
        /// </summary>
        /// <param name="keySizeType">密钥长度。</param>
        /// <returns>RSA Blob 二进制密钥结果。</returns>
        public static SecurityRsaKeyBlobBytesResultModel CreateRsaKeyBlobBytesWithResult(RsaKeySizeTypeEnum keySizeType)
        {
            var result = new SecurityRsaKeyBlobBytesResultModel
            {
                KeySizeType = keySizeType,
            };

            try
            {
                using var rsa = new RSACryptoServiceProvider((int)keySizeType);
                result.PublicKeyBlobBytes = rsa.ExportCspBlob(false);
                result.PrivateKeyBlobBytes = rsa.ExportCspBlob(true);
                result.IsSuccess = result.PublicKeyBlobBytes != null && result.PrivateKeyBlobBytes != null;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// RSA 加密 二进制数组
        /// </summary>
        /// <param name="publicKeyBlobBytes">公钥Blob二进制数组</param>
        /// <param name="keySizeType">密钥长度(加密级别) 1024/2048/3072/7680/15360 </param>
        /// <param name="bytesToEncrypt">要加密的二进制数组</param>
        /// <returns></returns>
        public static byte[] RsaEncryptBytesToBytes(byte[] publicKeyBlobBytes, RsaKeySizeTypeEnum keySizeType, byte[] bytesToEncrypt)
        {
            return RsaEncryptBytesToBytesWithResult(publicKeyBlobBytes, keySizeType, bytesToEncrypt).ResultBytes;
        }

        /// <summary>
        /// RSA 加密二进制数组，并返回详细结果。
        /// </summary>
        /// <param name="publicKeyBlobBytes">公钥 Blob 二进制数组。</param>
        /// <param name="keySizeType">密钥长度。</param>
        /// <param name="bytesToEncrypt">要加密的二进制数组。</param>
        /// <returns>RSA 二进制加密结果。</returns>
        public static SecurityRsaBytesOperationResultModel RsaEncryptBytesToBytesWithResult(byte[] publicKeyBlobBytes, RsaKeySizeTypeEnum keySizeType, byte[] bytesToEncrypt)
        {
            var result = new SecurityRsaBytesOperationResultModel
            {
                IsEncryptOperation = true,
                KeySizeType = keySizeType,
            };

            try
            {
                using var rsa = new RSACryptoServiceProvider((int)keySizeType);
                rsa.ImportCspBlob(publicKeyBlobBytes);
                result.ResultBytes = rsa.Encrypt(bytesToEncrypt, false);
                result.IsSuccess = result.ResultBytes != null;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;

        }

        /// <summary>
        /// RSA 解密 二进制数组
        /// </summary>
        /// <param name="privateKeyBlobBytes">私钥Blob二进制数组</param>
        /// <param name="keySizeType">密钥长度(加密级别) 1024/2048/3072/7680/15360 </param>
        /// <param name="bytesToDecrypt">要解密的二进制数组</param>
        /// <returns></returns>
        public static byte[] RsaDecryptBytesFromBytes(byte[] privateKeyBlobBytes, RsaKeySizeTypeEnum keySizeType, byte[] bytesToDecrypt)
        {
            return RsaDecryptBytesFromBytesWithResult(privateKeyBlobBytes, keySizeType, bytesToDecrypt).ResultBytes;
        }

        /// <summary>
        /// RSA 解密二进制数组，并返回详细结果。
        /// </summary>
        /// <param name="privateKeyBlobBytes">私钥 Blob 二进制数组。</param>
        /// <param name="keySizeType">密钥长度。</param>
        /// <param name="bytesToDecrypt">要解密的二进制数组。</param>
        /// <returns>RSA 二进制解密结果。</returns>
        public static SecurityRsaBytesOperationResultModel RsaDecryptBytesFromBytesWithResult(byte[] privateKeyBlobBytes, RsaKeySizeTypeEnum keySizeType, byte[] bytesToDecrypt)
        {
            var result = new SecurityRsaBytesOperationResultModel
            {
                IsEncryptOperation = false,
                KeySizeType = keySizeType,
            };

            try
            {
                using var rsa = new RSACryptoServiceProvider((int)keySizeType);
                rsa.ImportCspBlob(privateKeyBlobBytes);
                result.ResultBytes = rsa.Decrypt(bytesToDecrypt, false);
                result.IsSuccess = result.ResultBytes != null;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;

        }

        /// <summary>
        /// RSA 加密 字符串 返回 加密后的 Base64 字符串
        /// </summary>
        /// <param name="publicKeyBlobBase64String">公钥 Blob 二进制数组 的 Base64 字符串</param>
        /// <param name="keySizeType">密钥长度(加密级别) 1024/2048/3072/7680/15360</param>
        /// <param name="strToEncrypt">要加密的字符串</param>
        /// <returns></returns>
        public static string RsaEncryptStringToBase64String(string publicKeyBlobBase64String, RsaKeySizeTypeEnum keySizeType, string strToEncrypt)
        {
            return RsaEncryptStringToBase64StringWithResult(publicKeyBlobBase64String, keySizeType, strToEncrypt).ResultString ?? string.Empty;
        }

        /// <summary>
        /// RSA 加密字符串，并返回详细结果。
        /// </summary>
        /// <param name="publicKeyBlobBase64String">公钥 Blob Base64 字符串。</param>
        /// <param name="keySizeType">密钥长度。</param>
        /// <param name="strToEncrypt">要加密的字符串。</param>
        /// <returns>RSA 字符串加密结果。</returns>
        public static SecurityRsaStringOperationResultModel RsaEncryptStringToBase64StringWithResult(string publicKeyBlobBase64String, RsaKeySizeTypeEnum keySizeType, string strToEncrypt)
        {
            var result = new SecurityRsaStringOperationResultModel
            {
                IsEncryptOperation = true,
                KeySizeType = keySizeType,
            };

            try
            {
                var bytesResult = RsaEncryptBytesToBytesWithResult(Convert.FromBase64String(publicKeyBlobBase64String), keySizeType, DefaultSettingKeys.DEFAULT_ENCODING.GetBytes(strToEncrypt));
                if (!bytesResult.IsSuccess)
                {
                    result.Exception = bytesResult.Exception;
                    return result;
                }

                result.ResultString = Convert.ToBase64String(bytesResult.ResultBytes);
                result.IsSuccess = !result.ResultString.IfIsNullOrEmpty();
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;

        }

        /// <summary>
        /// RSA 解密 Base64 字符串 返回 解密后的 原始 字符串  
        /// </summary>
        /// <param name="privateKeyBlobBase64String">私钥 Blob 二进制数组 的 Base64 字符串</param>
        /// <param name="keySizeType">密钥长度(加密级别) 1024/2048/3072/7680/15360</param>
        /// <param name="base64StringToDecrypt">要 解密 的 Base64 字符串</param>
        /// <returns></returns>
        public static string RsaDecryptStringFromBase64String(string privateKeyBlobBase64String, RsaKeySizeTypeEnum keySizeType, string base64StringToDecrypt)
        {
            return RsaDecryptStringFromBase64StringWithResult(privateKeyBlobBase64String, keySizeType, base64StringToDecrypt).ResultString ?? string.Empty;
        }

        /// <summary>
        /// RSA 解密 Base64 字符串，并返回详细结果。
        /// </summary>
        /// <param name="privateKeyBlobBase64String">私钥 Blob Base64 字符串。</param>
        /// <param name="keySizeType">密钥长度。</param>
        /// <param name="base64StringToDecrypt">要解密的 Base64 字符串。</param>
        /// <returns>RSA 字符串解密结果。</returns>
        public static SecurityRsaStringOperationResultModel RsaDecryptStringFromBase64StringWithResult(string privateKeyBlobBase64String, RsaKeySizeTypeEnum keySizeType, string base64StringToDecrypt)
        {
            var result = new SecurityRsaStringOperationResultModel
            {
                IsEncryptOperation = false,
                KeySizeType = keySizeType,
            };

            try
            {
                var bytesResult = RsaDecryptBytesFromBytesWithResult(Convert.FromBase64String(privateKeyBlobBase64String), keySizeType, Convert.FromBase64String(base64StringToDecrypt));
                if (!bytesResult.IsSuccess)
                {
                    result.Exception = bytesResult.Exception;
                    return result;
                }

                result.ResultString = DefaultSettingKeys.DEFAULT_ENCODING.GetString(bytesResult.ResultBytes);
                result.IsSuccess = result.ResultString != null;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;

        }



    }
}
