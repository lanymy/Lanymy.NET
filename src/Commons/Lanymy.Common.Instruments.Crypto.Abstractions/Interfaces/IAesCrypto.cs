using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Interfaces
{
    /// <summary>
    /// 定义基于 AES 的轻量字符串、字节和模型加解密能力。
    /// </summary>
    public interface IAesCrypto
    {
        /// <summary>
        /// 把字节数组加密为字节数组。
        /// </summary>
        byte[] EncryptBytesToBteys(byte[] sourceBytes, string key = null, string iv = null, Encoding encoding = null);

        /// <summary>
        /// 把 AES 密文字节解密为原始字节数组。
        /// </summary>
        byte[] DecryptBytesFromBteys(byte[] encryptBytes, string key = null, string iv = null, Encoding encoding = null);

        /// <summary>
        /// 把字符串加密为字节数组。
        /// </summary>
        byte[] EncryptStringToBteys(string sourceString, string key = null, string iv = null, Encoding encoding = null);

        /// <summary>
        /// 把密文字节解密为字符串。
        /// </summary>
        string DecryptStringFromBteys(byte[] encrypBytes, string key = null, string iv = null, Encoding encoding = null);

        /// <summary>
        /// 把字符串加密为 Base64 文本。
        /// </summary>
        string EncryptStringToString(string sourceString, string key = null, string iv = null, Encoding encoding = null);

        /// <summary>
        /// 把 Base64 密文解密为原始字符串。
        /// </summary>
        string DecryptStringFromString(string encryptString, string key = null, string iv = null, Encoding encoding = null);

        /// <summary>
        /// 把模型序列化并加密为 Base64 文本。
        /// </summary>
        string EncryptModelToString<T>(T t, string key = null, string iv = null, Encoding encoding = null) where T : class;

        /// <summary>
        /// 把 Base64 密文解密并反序列化为模型。
        /// </summary>
        T DecryptModelFromString<T>(string encryptString, string key = null, string iv = null, Encoding encoding = null) where T : class;

        /// <summary>
        /// 把模型序列化并加密到文件。
        /// </summary>
        void EncryptModelToFile<T>(T t, string fileFullPath, string key = null, string iv = null, Encoding encoding = null) where T : class;

        /// <summary>
        /// 从加密文件解密并反序列化模型。
        /// </summary>
        T DecryptModelFromFile<T>(string fileFullPath, string key = null, string iv = null, Encoding encoding = null) where T : class;
    }
}
