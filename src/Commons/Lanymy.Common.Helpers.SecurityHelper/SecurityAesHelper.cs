using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.Interfaces;
using Lanymy.Common.Helpers.ResultModels;


namespace Lanymy.Common.Helpers
{


    public class SecurityAesHelper
    {


        public static readonly IAesCrypto DefaultLanymyCrypto = new LanymyAesCrypto();



        public static byte[] EncryptBytesToBteys(byte[] sourceBytes, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null)
        {
            return GenericityHelper.GetInterface(crypto, DefaultLanymyCrypto).EncryptBytesToBteys(sourceBytes, key, iv, encoding);
        }

        public static byte[] DecryptBytesFromBteys(byte[] encryptBytes, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null)
        {
            return GenericityHelper.GetInterface(crypto, DefaultLanymyCrypto).DecryptBytesFromBteys(encryptBytes, key, iv, encoding);
        }




        public static byte[] EncryptStringToBteys(string sourceString, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null)
        {
            return GenericityHelper.GetInterface(crypto, DefaultLanymyCrypto).EncryptStringToBteys(sourceString, key, iv, encoding);
        }
        public static string DecryptStringFromBteys(byte[] encrypBytes, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null)
        {
            return GenericityHelper.GetInterface(crypto, DefaultLanymyCrypto).DecryptStringFromBteys(encrypBytes, key, iv, encoding);
        }




        public static string EncryptStringToString(string sourceString, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null)
        {
            return GenericityHelper.GetInterface(crypto, DefaultLanymyCrypto).EncryptStringToString(sourceString, key, iv, encoding);
        }
        public static string DecryptStringFromString(string encryptString, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null)
        {
            return GenericityHelper.GetInterface(crypto, DefaultLanymyCrypto).DecryptStringFromString(encryptString, key, iv, encoding);
        }




        public static string EncryptModelToString<T>(T t, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null) where T : class
        {
            return GenericityHelper.GetInterface(crypto, DefaultLanymyCrypto).EncryptModelToString(t, key, iv, encoding);
        }

        public static T DecryptModelFromString<T>(string encryptString, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null) where T : class
        {
            return GenericityHelper.GetInterface(crypto, DefaultLanymyCrypto).DecryptModelFromString<T>(encryptString, key, iv, encoding);
        }
        public static void EncryptModelToFile<T>(T t, string fileFullPath, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null) where T : class
        {
            var result = EncryptModelToFileWithResult(t, fileFullPath, key, iv, encoding, crypto);
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }
        }

        /// <summary>
        /// 加密模型并写入文件，同时返回详细结果。
        /// </summary>
        /// <typeparam name="T">模型类型。</typeparam>
        /// <param name="t">源模型。</param>
        /// <param name="fileFullPath">输出文件路径。</param>
        /// <param name="key">密钥。</param>
        /// <param name="iv">初始向量。</param>
        /// <param name="encoding">编码。</param>
        /// <param name="crypto">AES 实现。</param>
        /// <returns>AES 模型文件加密结果。</returns>
        public static SecurityAesFileOperationResultModel<T> EncryptModelToFileWithResult<T>(T t, string fileFullPath, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null) where T : class
        {
            var result = new SecurityAesFileOperationResultModel<T>
            {
                FilePath = fileFullPath,
                IsEncryptOperation = true,
                Model = t,
                ModelTypeName = typeof(T).Name,
                ModelTypeFullName = typeof(T).FullName,
            };

            try
            {
                GenericityHelper.GetInterface(crypto, DefaultLanymyCrypto).EncryptModelToFile(t, fileFullPath, key, iv, encoding);
                result.TargetFileExists = File.Exists(fileFullPath);
                result.IsSuccess = result.TargetFileExists;
                if (!result.IsSuccess)
                {
                    result.ErrorMessage = "Encrypted file was not created.";
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
        public static T DecryptModelFromFile<T>(string fileFullPath, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null) where T : class
        {
            var result = DecryptModelFromFileWithResult<T>(fileFullPath, key, iv, encoding, crypto);
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }

            return result.Model;
        }

        /// <summary>
        /// 从文件解密模型，同时返回详细结果。
        /// </summary>
        /// <typeparam name="T">模型类型。</typeparam>
        /// <param name="fileFullPath">输入文件路径。</param>
        /// <param name="key">密钥。</param>
        /// <param name="iv">初始向量。</param>
        /// <param name="encoding">编码。</param>
        /// <param name="crypto">AES 实现。</param>
        /// <returns>AES 模型文件解密结果。</returns>
        public static SecurityAesFileOperationResultModel<T> DecryptModelFromFileWithResult<T>(string fileFullPath, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null) where T : class
        {
            var result = new SecurityAesFileOperationResultModel<T>
            {
                FilePath = fileFullPath,
                IsEncryptOperation = false,
                TargetFileExists = File.Exists(fileFullPath),
                ModelTypeName = typeof(T).Name,
                ModelTypeFullName = typeof(T).FullName,
            };

            try
            {
                result.Model = GenericityHelper.GetInterface(crypto, DefaultLanymyCrypto).DecryptModelFromFile<T>(fileFullPath, key, iv, encoding);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }




    }

}
