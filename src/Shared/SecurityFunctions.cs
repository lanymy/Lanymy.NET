/********************************************************************

时间: 2015年01月15日, PM 05:01:16

作者: lanyanmiyu@qq.com

描述: 加密解密类

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Lanymy.General.Extension.ExtensionFunctions;
using System.Security.Cryptography.X509Certificates;

namespace Lanymy.General.Extension
{
    /// <summary>
    /// 加密解密类
    /// </summary>
    public class SecurityFunctions
    {

        private const string SPLIT_STRING = "[$@lanymy@$]";


        /// <summary>
        /// 默认密钥
        /// </summary>
        private const string DEFAULT_SECURITY_KEY = GlobalSettings.DEFAULT_SECURITY_KEY;


        /// <summary>
        /// 获取加密效验头
        /// </summary>
        /// <returns></returns>
        private static byte[] GetEncryptHeader()
        {
            return GlobalSettings.DEFAULT_ENCODING.GetBytes(string.Format("{0}{1}{2}", DateTime.Now.Second, DateTime.Now.Millisecond, SPLIT_STRING));
        }

        /// <summary>
        /// 获取解密效验头
        /// </summary>
        /// <returns></returns>
        private static byte[] GetDecryptHeader()
        {
            return GlobalSettings.DEFAULT_ENCODING.GetBytes(SPLIT_STRING);
        }


        private static byte[] GetSecurityKey16Bytes(string securityKey, Encoding encoding)
        {
            const int keySize = 16;
            byte[] bytes = encoding.GetBytes(securityKey);

            return bytes.Length >= keySize? bytes.Take(keySize).ToArray(): ArrayFunctions.MergerArray(bytes, new byte[keySize - bytes.Length]);
        }


        /// <summary>
        /// 二进制数组加密 返回加密后的二进制数组
        /// </summary>
        /// <param name="bytesToEncrypt">要加密的二进制数组</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static byte[] EncryptBytesToBytes(byte[] bytesToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {

            if (bytesToEncrypt.IfIsNullOrEmpty()) throw new ArgumentNullException("bytesToEncrypt");
            if (secretKey.IfIsNullOrEmpty()) secretKey = DEFAULT_SECURITY_KEY;
            if (encoding.IfIsNullOrEmpty()) encoding = GlobalSettings.DEFAULT_ENCODING;


            byte[] result = null;

            if (ifRandom)
            {
                bytesToEncrypt = ArrayFunctions.MergerArray(GetEncryptHeader(), bytesToEncrypt);
            }

            byte[] secretKeyBytes = GetSecurityKey16Bytes(secretKey, encoding);

            using (MemoryStream encryptStream = new MemoryStream())
            {
                using (ICryptoTransform transform = new TripleDESCryptoServiceProvider().CreateEncryptor(secretKeyBytes, secretKeyBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(encryptStream, transform, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
                        cryptoStream.FlushFinalBlock();
                        result = encryptStream.ToArray();
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// 字符串加密 返回 加密后的二进制数组
        /// </summary>
        /// <param name="strToEncrypt">要加密的字符串</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static byte[] EncryptStringToBytes(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {
            if (strToEncrypt.IfIsNullOrEmpty()) throw new ArgumentNullException("strToEncrypt");
            if (encoding.IfIsNullOrEmpty()) encoding = GlobalSettings.DEFAULT_ENCODING;
            return EncryptBytesToBytes(encoding.GetBytes(strToEncrypt), secretKey, ifRandom, encoding);
        }

        /// <summary>
        /// 字符串加密 返回 加密后的 Base64 字符串
        /// </summary>
        /// <param name="strToEncrypt">要加密的字符串</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static string EncryptStringToBase64String(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {
            var bytes = EncryptStringToBytes(strToEncrypt, secretKey, ifRandom, encoding);
            return bytes.IfIsNullOrEmpty() ? string.Empty : Convert.ToBase64String(bytes);
        }













        /// <summary>
        /// 解密二进制数组 返回 解密后的二进制数组
        /// </summary>
        /// <param name="bytesToDecrypt">要解密的二进制数组</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="encoding">解密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static byte[] DecryptBytesFromBytes(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null)
        {

            if (bytesToDecrypt.IfIsNullOrEmpty()) throw new ArgumentNullException("bytesToDecrypt");
            if (secretKey.IfIsNullOrEmpty()) secretKey = DEFAULT_SECURITY_KEY;
            if (encoding.IfIsNullOrEmpty()) encoding = GlobalSettings.DEFAULT_ENCODING;


            byte[] result = null;

            //密钥必须16位
            byte[] secretKeyBytes = GetSecurityKey16Bytes(secretKey, encoding);

            using (ICryptoTransform transform = new TripleDESCryptoServiceProvider().CreateDecryptor(secretKeyBytes, secretKeyBytes))
            {
                using (MemoryStream stream = new MemoryStream(bytesToDecrypt))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read))
                    {
                        using (MemoryStream readStream = new MemoryStream())
                        {
                            cryptoStream.CopyTo(readStream);
                            result = readStream.ToArray();
                        }
                    }
                }
            }

            var tempStr = GlobalSettings.DEFAULT_ENCODING.GetString(result.Take(25).ToArray());

            if (tempStr.IndexOf(SPLIT_STRING) >= 0)
            {
                result =result.Skip(GlobalSettings.DEFAULT_ENCODING.GetBytes(tempStr.LeftSubString(SPLIT_STRING, false)).Length).ToArray();
            }

            return result;
        }


        /// <summary>
        /// 解密二进制数组 返回 解密后的字符串
        /// </summary>
        /// <param name="bytesToDecrypt">要解密的二进制数组</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="encoding">解密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static string DecryptStringFromBytes(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null)
        {
            if (encoding.IfIsNullOrEmpty()) encoding = GlobalSettings.DEFAULT_ENCODING;

            var bytes = DecryptBytesFromBytes(bytesToDecrypt, secretKey, encoding);

            if (bytes.IfIsNullOrEmpty()) return string.Empty;

            return encoding.GetString(bytes);
        }

        /// <summary>
        /// 解密字符串 返回 解密后的字符串
        /// </summary>
        /// <param name="base64StringToDecrypt">要解密的Base64字符串</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="encoding">解密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static string DecryptStringFromBase64String(string base64StringToDecrypt, string secretKey = null, Encoding encoding = null)
        {
            if (base64StringToDecrypt.IfIsNullOrEmpty()) throw new ArgumentNullException("base64StringToDecrypt");
            return DecryptStringFromBytes(Convert.FromBase64String(base64StringToDecrypt), secretKey, encoding);
        }




        /// <summary>
        /// MD5加密 可以和PHP通用
        /// </summary>
        /// <param name="strToEncrypt"></param>
        /// <returns></returns>
        public static string EncryptToMD5(string strToEncrypt)
        {

            if (strToEncrypt.IfIsNullOrEmpty()) throw new ArgumentNullException("strToEncrypt");
            var md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(strToEncrypt.ToLower());
            byte[] hashedBytes = md5.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            foreach (var b in hashedBytes)
            {
                sb.Append(b.ToString("x2").ToLower());
            }

            return sb.ToString();

        }



        /// <summary>
        /// 动态加载证书 必须有管理员权限 才能调用成功
        /// </summary>
        /// <param name="rawData">证书二进制数据</param>
        public static void LoadCertificate(byte[] rawData)
        {

            if (rawData.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(rawData));


            LoadCertificate(new X509Certificate2(rawData));

        }


        /// <summary>
        /// 动态加载证书 必须有管理员权限 才能调用成功
        /// </summary>
        /// <param name="rawData">证书二进制数据</param>
        /// <param name="password">证书密码</param>
        public static void LoadCertificate(byte[] rawData, string password)
        {
            if (rawData.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(rawData));
            if (password.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(password));
            LoadCertificate(new X509Certificate2(rawData, password));
        }

        /// <summary>
        /// 动态加载证书 必须有管理员权限 才能调用成功
        /// </summary>
        /// <param name="certFileFullPath">证书文件全路径</param>
        public static void LoadCertificate(string certFileFullPath)
        {
            if (!File.Exists(certFileFullPath)) throw new FileNotFoundException(certFileFullPath);
            LoadCertificate(new X509Certificate2(certFileFullPath));
        }

        /// <summary>
        /// 动态加载证书 必须有管理员权限 才能调用成功
        /// </summary>
        /// <param name="certFileFullPath">证书文件全路径</param>
        /// <param name="password">证书密码</param>
        public static void LoadCertificate(string certFileFullPath, string password)
        {
            if (!File.Exists(certFileFullPath)) throw new FileNotFoundException(certFileFullPath);
            if (password.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(password));
            LoadCertificate(new X509Certificate2(certFileFullPath, password));
        }

        /// <summary>
        /// 动态加载证书 必须有管理员权限 才能调用成功
        /// </summary>
        /// <param name="certificate">证书实例</param>
        public static void LoadCertificate(X509Certificate2 certificate)
        {

            X509Store storeRoot = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            storeRoot.Open(OpenFlags.ReadWrite);
            if (!storeRoot.Certificates.Contains(certificate))
            {
                storeRoot.Add(certificate);
            }
            storeRoot.Close();


            X509Store storeMy = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            storeMy.Open(OpenFlags.ReadWrite);
            if (!storeMy.Certificates.Contains(certificate))
            {
                storeMy.Add(certificate);
            }
            storeMy.Close();

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
        public static string EncryptFileNameToBase64String
        (
            string fileName,
            string encryptFileExtension = ".base64",
            string secretKey = null,
            bool ifRandom = true,
            Encoding encoding = null)
        {
            if (fileName.IfIsNullOrEmpty()) return string.Empty;
            return FormatFunctions.FormatBase64StringToFileNameBase64String(EncryptStringToBase64String(fileName, secretKey, ifRandom, encoding)) + encryptFileExtension;
        }


        /// <summary>
        /// 解密 文件名合法的Base64加密字符串编码 成 原文件名
        /// </summary>
        /// <param name="encryptFileNameBase64String">文件名合法的Base64加密字符串编码</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static string DecryptFileNameFromBase64String(string encryptFileNameBase64String, string secretKey = null, Encoding encoding = null)
        {

            if (encryptFileNameBase64String.IfIsNullOrEmpty()) return string.Empty;

            if (Path.HasExtension(encryptFileNameBase64String))
                encryptFileNameBase64String = Path.GetFileNameWithoutExtension(encryptFileNameBase64String);


            return DecryptStringFromBase64String(FormatFunctions.FormatBase64StringFromFileNameBase64String(encryptFileNameBase64String), secretKey, encoding);

        }



        /// <summary>
        /// 加密 文件夹名称 成 文件夹名称 合法的Base64字符串编码
        /// </summary>
        /// <param name="directoryName">要加密的文件夹名称</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static string EncryptDirectoryNameToBase64String(string directoryName, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {
            return EncryptFileNameToBase64String(directoryName, string.Empty, secretKey, ifRandom, encoding);
        }


        /// <summary>
        /// 解密 文件夹名合法的Base64加密字符串编码 成 原文件夹名称
        /// </summary>
        /// <param name="directoryName">文件夹名合法的Base64加密字符串编码</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static string DecryptDirectoryNameFromBase64String(string directoryName, string secretKey = null, Encoding encoding = null)
        {
            return DecryptFileNameFromBase64String(directoryName, secretKey, encoding);
        }



        /// <summary>
        /// 加密并序列化Model成字节数组
        /// </summary>
        /// <typeparam name="T">要加密序列化的实体类型</typeparam>
        /// <param name="t">要加密序列化的实体实例</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static byte[] EncryptModelToBytes<T>(T t, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where T : class
        {
            return EncryptStringToBytes(SerializeFunctions.SerializeToJson(t), secretKey, ifRandom, encoding);
        }


        /// <summary>
        /// 解密并反序列化字节数组 返回 Model
        /// </summary>
        /// <typeparam name="T">要解密并反序列化的实体类型</typeparam>
        /// <param name="bytes">要解密并反序列化的实体实例</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="encoding">编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static T DecryptModelFromBytes<T>(byte[] bytes, string secretKey = null, Encoding encoding = null) where T : class
        {
            try
            {
                return SerializeFunctions.DeserializeFromJson<T>(DecryptStringFromBytes(bytes, secretKey, encoding));
            }
            catch
            {
                return default(T);
            }
        }


        /// <summary>
        /// 加密并序列化Model成Base64字符串
        /// </summary>
        /// <typeparam name="T">要序列化的实体类型</typeparam>
        /// <param name="t">要序列化的实体 实例</param>
        /// <param name="secretKey">密钥 Null 使用默认密钥</param>
        /// <returns></returns>
        public static string EncryptModelToBase64String<T>(T t, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where T : class
        {
            return Convert.ToBase64String(EncryptModelToBytes(t, secretKey, ifRandom, encoding));
        }


        /// <summary>
        /// 解密并反序列化Base64字符串成实体实例
        /// </summary>
        /// <typeparam name="T">要反序列化成的实体类型</typeparam>
        /// <param name="encryptBase64String">要解密的Base64字符串</param>
        /// <param name="secretKey">密钥 Null 使用默认密钥</param>
        /// <returns></returns>
        public static T DecryptModelFromBase64String<T>(string encryptBase64String, string secretKey = null, Encoding encoding = null) where T : class
        {
            try
            {
                return DecryptModelFromBytes<T>(Convert.FromBase64String(encryptBase64String));
            }
            catch
            {
                return default(T);
            }
        }
    }
}
