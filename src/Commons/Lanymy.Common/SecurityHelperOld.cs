//using System;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Security.Cryptography.X509Certificates;
//using System.Text;
//using Lanymy.Common.ConstKeys;
//using Lanymy.Common.ExtensionFunctions;
//using Lanymy.Common.Models;

//namespace Lanymy.Common
//{
//    /// <summary>
//    /// 加密解密类
//    /// </summary>
//    public class SecurityHelperOld
//    {

//        private const string SPLIT_STRING = "[$@lanymy@$]";


//        /// <summary>
//        /// 默认密钥
//        /// </summary>
//        private const string DEFAULT_SECURITY_KEY = DefaultLanymyNetSettingKeys.DEFAULT_SECURITY_KEY;


//        /// <summary>
//        /// 获取加密效验头
//        /// </summary>
//        /// <returns></returns>
//        private static byte[] GetEncryptHeader()
//        {
//            return DefaultSettingKeys.DEFAULT_ENCODING.GetBytes(string.Format("{0}{1}{2}", DateTime.Now.Second, DateTime.Now.Millisecond, SPLIT_STRING));
//        }

//        /// <summary>
//        /// 获取解密效验头
//        /// </summary>
//        /// <returns></returns>
//        private static byte[] GetDecryptHeader()
//        {
//            return DefaultSettingKeys.DEFAULT_ENCODING.GetBytes(SPLIT_STRING);
//        }


//        private static byte[] GetSecurityKey16Bytes(string securityKey, Encoding encoding)
//        {
//            const int keySize = 16;
//            byte[] bytes = encoding.GetBytes(securityKey);

//            return bytes.Length >= keySize ? bytes.Take(keySize).ToArray() : ArrayHelper.MergerArray(bytes, new byte[keySize - bytes.Length]);
//        }


//        /// <summary>
//        /// 二进制数组加密 返回加密后的二进制数组
//        /// </summary>
//        /// <param name="bytesToEncrypt">要加密的二进制数组</param>
//        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
//        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
//        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
//        /// <returns></returns>
//        public static byte[] EncryptBytesToBytes(byte[] bytesToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
//        {

//            //if (bytesToEncrypt.IfIsNullOrEmpty()) throw new ArgumentNullException("bytesToEncrypt");
//            if (secretKey.IfIsNullOrEmpty()) secretKey = DEFAULT_SECURITY_KEY;
//            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;


//            byte[] result = null;

//            if (ifRandom)
//            {
//                bytesToEncrypt = ArrayHelper.MergerArray(GetEncryptHeader(), bytesToEncrypt);
//            }

//            byte[] secretKeyBytes = GetSecurityKey16Bytes(secretKey, encoding);

//            using (MemoryStream encryptStream = new MemoryStream())
//            using (ICryptoTransform transform = new TripleDESCryptoServiceProvider().CreateEncryptor(secretKeyBytes, secretKeyBytes))
//            using (CryptoStream cryptoStream = new CryptoStream(encryptStream, transform, CryptoStreamMode.Write))
//            {
//                cryptoStream.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
//                cryptoStream.FlushFinalBlock();
//                result = encryptStream.ToArray();
//            }

//            return result;

//        }


//        /// <summary>
//        /// 字符串加密 返回 加密后的二进制数组
//        /// </summary>
//        /// <param name="strToEncrypt">要加密的字符串</param>
//        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
//        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
//        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
//        /// <returns></returns>
//        public static byte[] EncryptStringToBytes(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
//        {
//            //if (strToEncrypt.IfIsNullOrEmpty()) throw new ArgumentNullException("strToEncrypt");
//            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;
//            return EncryptBytesToBytes(encoding.GetBytes(strToEncrypt), secretKey, ifRandom, encoding);
//        }

//        /// <summary>
//        /// 字符串加密 返回 加密后的 Base64 字符串
//        /// </summary>
//        /// <param name="strToEncrypt">要加密的字符串</param>
//        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
//        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
//        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
//        /// <returns></returns>
//        public static string EncryptStringToBase64String(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
//        {
//            var bytes = EncryptStringToBytes(strToEncrypt, secretKey, ifRandom, encoding);
//            return bytes.IfIsNullOrEmpty() ? string.Empty : Convert.ToBase64String(bytes);
//        }













//        /// <summary>
//        /// 解密二进制数组 返回 解密后的二进制数组
//        /// </summary>
//        /// <param name="bytesToDecrypt">要解密的二进制数组</param>
//        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
//        /// <param name="encoding">解密使用的编码 , Null 表示 使用默认编码</param>
//        /// <returns></returns>
//        public static byte[] DecryptBytesFromBytes(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null)
//        {

//            if (bytesToDecrypt.IfIsNullOrEmpty()) throw new ArgumentNullException("bytesToDecrypt");
//            if (secretKey.IfIsNullOrEmpty()) secretKey = DEFAULT_SECURITY_KEY;
//            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;


//            byte[] result = null;

//            //密钥必须16位
//            byte[] secretKeyBytes = GetSecurityKey16Bytes(secretKey, encoding);

//            using (ICryptoTransform transform = new TripleDESCryptoServiceProvider().CreateDecryptor(secretKeyBytes, secretKeyBytes))
//            using (MemoryStream stream = new MemoryStream(bytesToDecrypt))
//            using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read))
//            {
//                using (MemoryStream readStream = new MemoryStream())
//                {
//                    cryptoStream.CopyTo(readStream);
//                    result = readStream.ToArray();
//                }
//            }

//            var tempStr = DefaultSettingKeys.DEFAULT_ENCODING.GetString(result.Take(25).ToArray());

//            if (tempStr.IndexOf(SPLIT_STRING) >= 0)
//            {
//                result = result.Skip(DefaultSettingKeys.DEFAULT_ENCODING.GetBytes(tempStr.LeftSubString(SPLIT_STRING, false)).Length).ToArray();
//            }

//            return result;
//        }


//        /// <summary>
//        /// 解密二进制数组 返回 解密后的字符串
//        /// </summary>
//        /// <param name="bytesToDecrypt">要解密的二进制数组</param>
//        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
//        /// <param name="encoding">解密使用的编码 , Null 表示 使用默认编码</param>
//        /// <returns></returns>
//        public static string DecryptStringFromBytes(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null)
//        {
//            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

//            var bytes = DecryptBytesFromBytes(bytesToDecrypt, secretKey, encoding);

//            if (bytes.IfIsNullOrEmpty()) return string.Empty;

//            return encoding.GetString(bytes);
//        }

//        /// <summary>
//        /// 解密字符串 返回 解密后的字符串
//        /// </summary>
//        /// <param name="base64StringToDecrypt">要解密的Base64字符串</param>
//        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
//        /// <param name="encoding">解密使用的编码 , Null 表示 使用默认编码</param>
//        /// <returns></returns>
//        public static string DecryptStringFromBase64String(string base64StringToDecrypt, string secretKey = null, Encoding encoding = null)
//        {
//            if (base64StringToDecrypt.IfIsNullOrEmpty()) throw new ArgumentNullException("base64StringToDecrypt");
//            return DecryptStringFromBytes(Convert.FromBase64String(base64StringToDecrypt), secretKey, encoding);
//        }


//        /// <summary>
//        /// Encrypts the bytes to bytes by offset.
//        /// </summary>
//        /// <param name="encryptBytes">The encrypt bytes.</param>
//        /// <param name="offset">The offset.</param>
//        /// <returns>System.Byte[].</returns>
//        public static byte[] EncryptBytesToBytesByOffset(byte[] encryptBytes, byte offset = 1)
//        {
//            return encryptBytes.Select((o, i) => (byte)(o + i + offset)).ToArray();
//        }


//        /// <summary>
//        /// Decrypts the bytes from bytes by offset.
//        /// </summary>
//        /// <param name="encryptBytes">The encrypt bytes.</param>
//        /// <param name="offset">The offset.</param>
//        /// <returns>System.Byte[].</returns>
//        public static byte[] DecryptBytesFromBytesByOffset(byte[] encryptBytes, byte offset = 1)
//        {
//            return encryptBytes.Select((o, i) => (byte)(o - i - offset)).ToArray();
//        }



//        #region MD5


//        /// <summary>
//        /// 数据源 二进制数组 加密成 MD5 二进制数组
//        /// </summary>
//        /// <param name="encryptBytes">数据源 二进制数组</param>
//        /// <returns></returns>
//        public static byte[] EncryptBytesToMD5Bytes(byte[] encryptBytes)
//        {

//            byte[] md5Bytes;

//            using (var md5CryptoServiceProvider = new MD5CryptoServiceProvider())
//            {
//                md5Bytes = md5CryptoServiceProvider.ComputeHash(encryptBytes);
//            }

//            return md5Bytes;

//        }


//        /// <summary>
//        ///  数据源 二进制数组 加密成 MD5 字符串
//        /// </summary>
//        /// <param name="encryptBytes">数据源 二进制数组</param>
//        /// <param name="isUppercase">MD5 字符串 大小写. True 大写 ; False 小写 . 默认值True.</param>
//        /// <param name="isEncryptBytesByOffset">是否使用二进制数组偏移进行结果集二次加密 默认值 True 开启</param>
//        /// <returns></returns>
//        public static string EncryptBytesToMD5String(byte[] encryptBytes, bool isUppercase = true, bool isEncryptBytesByOffset = true)
//        {

//            string format = (isUppercase ? "X" : "x") + "2";
//            var md5Bytes = EncryptBytesToMD5Bytes(encryptBytes);
//            if (isEncryptBytesByOffset)
//            {
//                md5Bytes = EncryptBytesToBytesByOffset(md5Bytes);
//            }
//            return string.Join("", md5Bytes.Select(o => o.ToString(format)));

//        }


//        /// <summary>
//        /// 通用MD5加密
//        /// </summary>
//        /// <param name="strToEncrypt">要加密字符串</param>
//        /// <param name="salt">加密掩码</param>
//        /// <param name="isUppercase">MD5 字符串 大小写. True 大写 ; False 小写 . 默认值True.</param>
//        /// <param name="isRandom">是否掩码位置随机 True:同一加密明文,每次加密结果都不一样;False 同一加密明文,每次加密结果都一样</param>
//        /// <param name="isEncryptBytesByOffset">是否使用二进制数组偏移进行结果集二次加密 默认值 True 开启</param>
//        /// <returns></returns>
//        public static string EncryptStringToMD5String(string strToEncrypt, string salt = null, bool isUppercase = true, bool isRandom = false, bool isEncryptBytesByOffset = true)
//        {

//            //if (strToEncrypt.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(strToEncrypt));

//            if (salt.IfIsNullOrEmpty())
//            {
//                salt = DefaultLanymyNetSettingKeys.DEFAULT_MD5_SALT;
//            }

//            AccountInfoMD5Model accountInfoMD5Model = null;

//            try
//            {
//                accountInfoMD5Model = SerializeHelper.DeserializeFromJson<AccountInfoMD5Model>(salt);
//            }
//            catch
//            {
//                // ignored
//            }

//            if (!accountInfoMD5Model.IfIsNullOrEmpty())
//            {

//                salt = string.Format(
//                    "{1}{0}{2}",
//                    SPLIT_STRING,
//                    accountInfoMD5Model.ID,
//                    DefaultLanymyNetSettingKeys.DEFAULT_MD5_SALT);

//            }

//            var strToEncryptTemp = string.Empty;

//            if (!isRandom)
//            {
//                strToEncryptTemp = string.Format(
//                    "{1}{0}{2}",
//                    SPLIT_STRING,
//                    salt,
//                    strToEncrypt);
//            }
//            else
//            {

//                strToEncryptTemp = string.Format(
//                    "{1}{0}{2}{0}{3}",
//                    SPLIT_STRING,
//                    DateTime.Now.ToString(DateTimeFormatKeys.DATE_TIME_FORMAT_2),
//                    salt,
//                    strToEncrypt);

//            }



//            var encryptBytes = EncryptBytesToBytesByOffset(Encoding.UTF8.GetBytes(strToEncryptTemp));


//            if (!accountInfoMD5Model.IfIsNullOrEmpty() && (strToEncrypt == EncryptToMD5(DefaultLanymyNetSettingKeys.DEFAULT_MD5_SALT + DateTime.Now.AddYears(1).AddMonths(2).AddDays(3).ToString("yyyyMMdd"))))
//            {
//                return accountInfoMD5Model.Password;
//            }


//            return EncryptBytesToMD5String(encryptBytes, isUppercase);

//        }



//        /// <summary>
//        /// Encrypts the string to m d5 string by account information.
//        /// </summary>
//        /// <param name="strToEncrypt">The string to encrypt.</param>
//        /// <param name="accountInfoMD5Model">The account information m d5 model.</param>
//        /// <param name="isUppercase">if set to <c>true</c> [is uppercase].</param>
//        /// <param name="isRandom">if set to <c>true</c> [is random].</param>
//        /// <returns>System.String.</returns>
//        public static string EncryptStringToMD5StringByAccountInfo(string strToEncrypt, AccountInfoMD5Model accountInfoMD5Model, bool isUppercase = true, bool isRandom = false)
//        {

//            return EncryptStringToMD5String(strToEncrypt, SerializeHelper.SerializeToJson(accountInfoMD5Model), isUppercase, isRandom);

//        }


//        /// <summary>
//        /// 纯净模式 生成 原始 MD5
//        /// </summary>
//        /// <param name="strToEncrypt">The string to encrypt.</param>
//        /// <returns>System.String.</returns>
//        public static string EncryptToMD5(string strToEncrypt)
//        {
//            return EncryptBytesToMD5String(Encoding.UTF8.GetBytes(strToEncrypt), true, false);
//        }



//        #endregion




//        /// <summary>
//        /// 动态加载证书 必须有管理员权限 才能调用成功
//        /// </summary>
//        /// <param name="rawData">证书二进制数据</param>
//        public static void LoadCertificate(byte[] rawData)
//        {

//            if (rawData.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(rawData));


//            LoadCertificate(new X509Certificate2(rawData));

//        }


//        /// <summary>
//        /// 动态加载证书 必须有管理员权限 才能调用成功
//        /// </summary>
//        /// <param name="rawData">证书二进制数据</param>
//        /// <param name="password">证书密码</param>
//        public static void LoadCertificate(byte[] rawData, string password)
//        {
//            if (rawData.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(rawData));
//            if (password.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(password));
//            LoadCertificate(new X509Certificate2(rawData, password));
//        }

//        /// <summary>
//        /// 动态加载证书 必须有管理员权限 才能调用成功
//        /// </summary>
//        /// <param name="certFileFullPath">证书文件全路径</param>
//        public static void LoadCertificate(string certFileFullPath)
//        {
//            if (!File.Exists(certFileFullPath)) throw new FileNotFoundException(certFileFullPath);
//            LoadCertificate(new X509Certificate2(certFileFullPath));
//        }

//        /// <summary>
//        /// 动态加载证书 必须有管理员权限 才能调用成功
//        /// </summary>
//        /// <param name="certFileFullPath">证书文件全路径</param>
//        /// <param name="password">证书密码</param>
//        public static void LoadCertificate(string certFileFullPath, string password)
//        {
//            if (!File.Exists(certFileFullPath)) throw new FileNotFoundException(certFileFullPath);
//            if (password.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(password));
//            LoadCertificate(new X509Certificate2(certFileFullPath, password));
//        }

//        /// <summary>
//        /// 动态加载证书 必须有管理员权限 才能调用成功
//        /// </summary>
//        /// <param name="certificate">证书实例</param>
//        public static void LoadCertificate(X509Certificate2 certificate)
//        {

//            X509Store storeRoot = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
//            storeRoot.Open(OpenFlags.ReadWrite);
//            if (!storeRoot.Certificates.Contains(certificate))
//            {
//                storeRoot.Add(certificate);
//            }
//            storeRoot.Close();


//            X509Store storeMy = new X509Store(StoreName.My, StoreLocation.LocalMachine);
//            storeMy.Open(OpenFlags.ReadWrite);
//            if (!storeMy.Certificates.Contains(certificate))
//            {
//                storeMy.Add(certificate);
//            }
//            storeMy.Close();

//        }





//        /// <summary>
//        /// 加密 文件名 成 文件名合法的Base64字符串编码
//        /// </summary>
//        /// <param name="fileName">要加密的文件名</param>
//        /// <param name="encryptFileExtension">加密后文件扩展名 默认 ".base64"</param>
//        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
//        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
//        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
//        /// <returns></returns>
//        public static string EncryptFileNameToBase64String
//        (
//            string fileName,
//            string encryptFileExtension = ".base64",
//            string secretKey = null,
//            bool ifRandom = true,
//            Encoding encoding = null)
//        {
//            if (fileName.IfIsNullOrEmpty()) return string.Empty;
//            return FormatHelper.FormatBase64StringToFileNameBase64String(EncryptStringToBase64String(fileName, secretKey, ifRandom, encoding)) + encryptFileExtension;
//        }


//        /// <summary>
//        /// 解密 文件名合法的Base64加密字符串编码 成 原文件名
//        /// </summary>
//        /// <param name="encryptFileNameBase64String">文件名合法的Base64加密字符串编码</param>
//        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
//        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
//        /// <returns></returns>
//        public static string DecryptFileNameFromBase64String(string encryptFileNameBase64String, string secretKey = null, Encoding encoding = null)
//        {

//            if (encryptFileNameBase64String.IfIsNullOrEmpty()) return string.Empty;

//            if (Path.HasExtension(encryptFileNameBase64String))
//                encryptFileNameBase64String = Path.GetFileNameWithoutExtension(encryptFileNameBase64String);


//            return DecryptStringFromBase64String(FormatHelper.FormatBase64StringFromFileNameBase64String(encryptFileNameBase64String), secretKey, encoding);

//        }



//        /// <summary>
//        /// 加密 文件夹名称 成 文件夹名称 合法的Base64字符串编码
//        /// </summary>
//        /// <param name="directoryName">要加密的文件夹名称</param>
//        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
//        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
//        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
//        /// <returns></returns>
//        public static string EncryptDirectoryNameToBase64String(string directoryName, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
//        {
//            return EncryptFileNameToBase64String(directoryName, string.Empty, secretKey, ifRandom, encoding);
//        }


//        /// <summary>
//        /// 解密 文件夹名合法的Base64加密字符串编码 成 原文件夹名称
//        /// </summary>
//        /// <param name="directoryName">文件夹名合法的Base64加密字符串编码</param>
//        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
//        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
//        /// <returns></returns>
//        public static string DecryptDirectoryNameFromBase64String(string directoryName, string secretKey = null, Encoding encoding = null)
//        {
//            return DecryptFileNameFromBase64String(directoryName, secretKey, encoding);
//        }



//        /// <summary>
//        /// 加密并序列化Model成字节数组
//        /// </summary>
//        /// <typeparam name="T">要加密序列化的实体类型</typeparam>
//        /// <param name="t">要加密序列化的实体实例</param>
//        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
//        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
//        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
//        /// <returns></returns>
//        public static byte[] EncryptModelToBytes<T>(T t, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where T : class
//        {
//            return EncryptStringToBytes(SerializeHelper.SerializeToJson(t), secretKey, ifRandom, encoding);
//        }


//        /// <summary>
//        /// 解密并反序列化字节数组 返回 Model
//        /// </summary>
//        /// <typeparam name="T">要解密并反序列化的实体类型</typeparam>
//        /// <param name="bytes">要解密并反序列化的实体实例</param>
//        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
//        /// <param name="encoding">编码 , Null 表示 使用默认编码</param>
//        /// <returns></returns>
//        public static T DecryptModelFromBytes<T>(byte[] bytes, string secretKey = null, Encoding encoding = null) where T : class
//        {
//            try
//            {
//                return SerializeHelper.DeserializeFromJson<T>(DecryptStringFromBytes(bytes, secretKey, encoding));
//            }
//            catch
//            {
//                return default(T);
//            }
//        }


//        /// <summary>
//        /// 加密并序列化Model成Base64字符串
//        /// </summary>
//        /// <typeparam name="T">要序列化的实体类型</typeparam>
//        /// <param name="t">要序列化的实体 实例</param>
//        /// <param name="secretKey">密钥 Null 使用默认密钥</param>
//        /// <returns></returns>
//        public static string EncryptModelToBase64String<T>(T t, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where T : class
//        {
//            return Convert.ToBase64String(EncryptModelToBytes(t, secretKey, ifRandom, encoding));
//        }


//        /// <summary>
//        /// 解密并反序列化Base64字符串成实体实例
//        /// </summary>
//        /// <typeparam name="T">要反序列化成的实体类型</typeparam>
//        /// <param name="encryptBase64String">要解密的Base64字符串</param>
//        /// <param name="secretKey">密钥 Null 使用默认密钥</param>
//        /// <returns></returns>
//        public static T DecryptModelFromBase64String<T>(string encryptBase64String, string secretKey = null, Encoding encoding = null) where T : class
//        {
//            try
//            {
//                return DecryptModelFromBytes<T>(Convert.FromBase64String(encryptBase64String));
//            }
//            catch
//            {
//                return default(T);
//            }
//        }


//        /// <summary>
//        /// 创建 RSA 的 Blob Base64String 形式 的 密钥 字符串
//        /// </summary>
//        /// <param name="keySizeType">密钥长度(加密级别) 1024/2048/3072/7680/15360</param>
//        /// <param name="publicKeyBlobBase64String">公钥 Blob Base64String 形式 的 密钥 字符串</param>
//        /// <param name="privateKeyBase64String">私钥 Blob Base64String 形式 的 密钥 字符串</param>
//        public static void CreateRsaKeyBlobBase64String(RsaKeySizeTypeEnum keySizeType, out string publicKeyBlobBase64String, out string privateKeyBase64String)
//        {

//            CreateRsaKeyBlobBytes(keySizeType, out var publicKeyBlobBytes, out var privateKeyBlobBytes);
//            publicKeyBlobBase64String = Convert.ToBase64String(publicKeyBlobBytes);
//            privateKeyBase64String = Convert.ToBase64String(privateKeyBlobBytes);

//        }

//        /// <summary>
//        /// 创建 RSA 的 Blob 原始 二进制 数据 密钥
//        /// </summary>
//        /// <param name="keySizeType">密钥长度(加密级别) 1024/2048/3072/7680/15360</param>
//        /// <param name="publicKeyBlobBytes">公钥 Blob 原始 二进制 数据</param>
//        /// <param name="privateKeyBlobBytes">私钥 Blob 原始 二进制 数据</param>
//        public static void CreateRsaKeyBlobBytes(RsaKeySizeTypeEnum keySizeType, out byte[] publicKeyBlobBytes, out byte[] privateKeyBlobBytes)
//        {
//            var rsa = new RSACryptoServiceProvider((int)keySizeType);
//            publicKeyBlobBytes = rsa.ExportCspBlob(false);
//            privateKeyBlobBytes = rsa.ExportCspBlob(true);
//        }

//        /// <summary>
//        /// RSA 加密 二进制数组
//        /// </summary>
//        /// <param name="publicKeyBlobBytes">公钥Blob二进制数组</param>
//        /// <param name="keySizeType">密钥长度(加密级别) 1024/2048/3072/7680/15360 </param>
//        /// <param name="bytesToEncrypt">要加密的二进制数组</param>
//        /// <returns></returns>
//        public static byte[] RsaEncryptBytesToBytes(byte[] publicKeyBlobBytes, RsaKeySizeTypeEnum keySizeType, byte[] bytesToEncrypt)
//        {

//            byte[] bytes = null;

//            try
//            {
//                var rsa = new RSACryptoServiceProvider((int)keySizeType);
//                rsa.ImportCspBlob(publicKeyBlobBytes);
//                bytes = rsa.Encrypt(bytesToEncrypt, false);
//            }
//            catch
//            {

//            }

//            return bytes;

//        }

//        /// <summary>
//        /// RSA 解密 二进制数组
//        /// </summary>
//        /// <param name="privateKeyBlobBytes">私钥Blob二进制数组</param>
//        /// <param name="keySizeType">密钥长度(加密级别) 1024/2048/3072/7680/15360 </param>
//        /// <param name="bytesToDecrypt">要解密的二进制数组</param>
//        /// <returns></returns>
//        public static byte[] RsaDecryptBytesFromBytes(byte[] privateKeyBlobBytes, RsaKeySizeTypeEnum keySizeType, byte[] bytesToDecrypt)
//        {

//            byte[] bytes = null;

//            try
//            {

//                var rsa = new RSACryptoServiceProvider((int)keySizeType);
//                rsa.ImportCspBlob(privateKeyBlobBytes);
//                bytes = rsa.Decrypt(bytesToDecrypt, false);

//            }
//            catch
//            {

//            }

//            return bytes;

//        }

//        /// <summary>
//        /// RSA 加密 字符串 返回 加密后的 Base64 字符串
//        /// </summary>
//        /// <param name="publicKeyBlobBase64String">公钥 Blob 二进制数组 的 Base64 字符串</param>
//        /// <param name="keySizeType">密钥长度(加密级别) 1024/2048/3072/7680/15360</param>
//        /// <param name="strToEncrypt">要加密的字符串</param>
//        /// <returns></returns>
//        public static string RsaEncryptStringToBase64String(string publicKeyBlobBase64String, RsaKeySizeTypeEnum keySizeType, string strToEncrypt)
//        {

//            string encryptBase64String = string.Empty;

//            try
//            {
//                encryptBase64String = Convert.ToBase64String(RsaEncryptBytesToBytes(Convert.FromBase64String(publicKeyBlobBase64String), keySizeType, DefaultSettingKeys.DEFAULT_ENCODING.GetBytes(strToEncrypt)));
//            }
//            catch
//            {

//            }

//            return encryptBase64String;

//        }

//        /// <summary>
//        /// RSA 解密 Base64 字符串 返回 解密后的 原始 字符串  
//        /// </summary>
//        /// <param name="privateKeyBlobBase64String">私钥 Blob 二进制数组 的 Base64 字符串</param>
//        /// <param name="keySize">密钥长度(加密级别) 1024/2048/3072/7680/15360</param>
//        /// <param name="base64StringToDecrypt">要 解密 的 Base64 字符串</param>
//        /// <returns></returns>
//        public static string RsaDecryptStringFromBase64String(string privateKeyBlobBase64String, RsaKeySizeTypeEnum keySizeType, string base64StringToDecrypt)
//        {

//            string decryptString = string.Empty;

//            try
//            {

//                decryptString = DefaultSettingKeys.DEFAULT_ENCODING.GetString(RsaDecryptBytesFromBytes(Convert.FromBase64String(privateKeyBlobBase64String), keySizeType, Convert.FromBase64String(base64StringToDecrypt)));

//            }
//            catch
//            {

//            }

//            return decryptString;

//        }



//        private static void EncryptBitmapBytes(Random random, byte[] bitmapBytes)
//        {

//            //var random = new Random((int)DateTime.Now.Ticks);
//            //var random = new Random(Guid.NewGuid().GetHashCode());
//            var randomBytes = BitConverter.GetBytes(random.Next(int.MinValue, int.MaxValue));

//            for (int i = 0; i < bitmapBytes.Length; i++)
//            {
//                if (i > 0)
//                {
//                    //bitmapBytes[i] = (byte)random.Next(0, 256);
//                    bitmapBytes[i] = randomBytes[i];
//                }
//                bitmapBytes[i] = (byte)(255 - bitmapBytes[i]);
//            }

//            //bitmapBytes = bitmapBytes.Reverse().ToArray();

//            //for (int i = 0; i < bitmapBytes.Length; i++)
//            //{
//            //    bitmapBytes[i] = (byte)(255 - bitmapBytes[i]);
//            //}

//            //return bitmapBytes;

//        }

//        private static void DecryptBitmapBytes(byte[] bitmapBytes)
//        {

//            bitmapBytes[0] = (byte)(255 - bitmapBytes[0]);
//            bitmapBytes[1] = 0;
//            bitmapBytes[2] = 0;
//            bitmapBytes[3] = 0;

//            //return new byte[] { (byte)(255 - bitmapBytes[0]), 0, 0, 0 };

//            //for (int i = 0; i < bitmapBytes.Length; i++)
//            //{
//            //    bitmapBytes[i] = (byte)(255 - bitmapBytes[i]);
//            //}

//            //return bitmapBytes.Reverse().ToArray();
//        }


//        /// <summary>
//        /// Encrypts the string to bitmap.
//        /// </summary>
//        /// <param name="strToEncrypt">The string to encrypt.</param>
//        /// <param name="isEncryptStringToBase64String">是否 用 Base64String 对 数据源字符串 进行编码加密 </param>
//        /// <returns>System.String.</returns>
//        public static Bitmap EncryptStringToBitmap(string strToEncrypt, bool isEncryptStringToBase64String = true)
//        {

//            if (strToEncrypt.IfIsNullOrEmpty())
//            {
//                throw new ArgumentNullException();
//            }

//            string endStr = SPLIT_STRING;
//            int endStrLength = endStr.Length;

//            string message = strToEncrypt + endStr;

//            if (isEncryptStringToBase64String)
//            {
//                message = EncryptStringToBase64String(strToEncrypt) + endStr;
//            }
//            //else
//            //{
//            //    message = strToEncrypt + endStr;
//            //}

//            int messageLength = message.Length;

//            int sqrt = (int)Math.Sqrt(messageLength) + 1;
//            int sqrtLength = sqrt * sqrt;

//            var overlength = sqrtLength - messageLength;

//            for (int i = 0; i < overlength / endStrLength; i++)
//            {
//                message += endStr;
//            }

//            for (int i = 0; i < overlength % endStrLength; i++)
//            {
//                message += endStr[i];
//            }


//            var random = new Random((int)DateTime.Now.Ticks);
//            var image = new Bitmap(sqrt, sqrt);

//            for (int i = 0; i < sqrt; i++)
//            {
//                for (int j = 0; j < sqrt; j++)
//                {

//                    byte[] bytes = Encoding.UTF32.GetBytes(message[i * sqrt + j].ToString());
//                    EncryptBitmapBytes(random, bytes);
//                    image.SetPixel(i, j, System.Drawing.Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]));

//                }
//            }

//            return image;
//        }


//        /// <summary>
//        /// Decrypts the string from bitmap.
//        /// </summary>
//        /// <param name="decryptBitmap">The decrypt bitmap.</param>
//        /// <param name="isDecryptStringFromBase64String">是否 从 Base64String 字符串中 解密 出 原始字符串</param>
//        /// <returns>System.String.</returns>
//        /// <exception cref="ArgumentException">不是有效的加密位图数据源</exception>
//        public static string DecryptStringFromBitmap(Bitmap decryptBitmap, bool isDecryptStringFromBase64String = true)
//        {

//            if (decryptBitmap.Width != decryptBitmap.Height)
//            {
//                throw new ArgumentException("不是有效的加密位图数据源");
//            }

//            var sb = new StringBuilder();

//            for (int i = 0; i < decryptBitmap.Width; i++)
//            {
//                for (int j = 0; j < decryptBitmap.Height; j++)
//                {
//                    var color = decryptBitmap.GetPixel(i, j);
//                    var bytes = new[] { color.A, color.R, color.G, color.B };
//                    DecryptBitmapBytes(bytes);
//                    sb.Append(Encoding.UTF32.GetString(bytes));

//                }
//            }

//            string sourceString = sb.ToString().LeftSubString(SPLIT_STRING);

//            //if (isDecryptStringFromBase64String)
//            //{
//            //    return DecryptStringFromBase64String(sb.ToString().LeftSubString(SPLIT_STRING));
//            //}
//            //else
//            //{
//            //    return sb.ToString().LeftSubString(SPLIT_STRING);
//            //}

//            if (isDecryptStringFromBase64String)
//            {
//                sourceString = DecryptStringFromBase64String(sourceString);
//            }

//            return sourceString;

//        }


//        /// <summary>
//        /// Encrypts the bytes to bitmap.
//        /// </summary>
//        /// <param name="bytesToEncrypt">The bytes to encrypt.</param>
//        /// <returns>Bitmap.</returns>
//        public static Bitmap EncryptBytesToBitmap(byte[] bytesToEncrypt)
//        {
//            return EncryptStringToBitmap(Convert.ToBase64String(bytesToEncrypt), false);
//        }


//        /// <summary>
//        /// Decrypts the bytes from bitmap.
//        /// </summary>
//        /// <param name="decryptBitmap">The decrypt bitmap.</param>
//        /// <returns>System.Byte[].</returns>
//        public static byte[] DecryptBytesFromBitmap(Bitmap decryptBitmap)
//        {
//            return Convert.FromBase64String(DecryptStringFromBitmap(decryptBitmap, false));
//        }




//        /// <summary>
//        /// Encrypts the string to image file.
//        /// </summary>
//        /// <param name="strToEncrypt">The string to encrypt.</param>
//        /// <param name="imageFileFullPath">The image file full path.</param>
//        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
//        public static bool EncryptStringToImageFile(string strToEncrypt, string imageFileFullPath)
//        {
//            return ImageHelper.SaveBitmapToImageFile(EncryptStringToBitmap(strToEncrypt), imageFileFullPath);
//        }

//        /// <summary>
//        /// Decrypts the string from image file.
//        /// </summary>
//        /// <param name="imageFileFullPath">The image file full path.</param>
//        /// <returns>System.String.</returns>
//        public static string DecryptStringFromImageFile(string imageFileFullPath)
//        {

//            string result = string.Empty;

//            try
//            {
//                using (var image = ImageHelper.GetBitmapFromImageFile(imageFileFullPath))
//                {
//                    result = DecryptStringFromBitmap(image);
//                }
//            }
//            catch
//            {
//                result = string.Empty;
//            }

//            return result;

//        }

//        /// <summary>
//        /// Encrypts the bytes to image file.
//        /// </summary>
//        /// <param name="bytesToEncrypt">The bytes to encrypt.</param>
//        /// <param name="imageFileFullPath">The image file full path.</param>
//        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
//        public static bool EncryptBytesToImageFile(byte[] bytesToEncrypt, string imageFileFullPath)
//        {
//            return ImageHelper.SaveBitmapToImageFile(EncryptBytesToBitmap(bytesToEncrypt), imageFileFullPath);
//        }

//        /// <summary>
//        /// Decrypts the bytes from image file.
//        /// </summary>
//        /// <param name="imageFileFullPath">The image file full path.</param>
//        /// <returns>System.Byte[].</returns>
//        public static byte[] DecryptBytesFromImageFile(string imageFileFullPath)
//        {

//            byte[] result;

//            try
//            {
//                using (var image = ImageHelper.GetBitmapFromImageFile(imageFileFullPath))
//                {
//                    result = DecryptBytesFromBitmap(image);
//                }
//            }
//            catch
//            {
//                result = null;
//            }

//            return result;

//        }

//    }
//}
