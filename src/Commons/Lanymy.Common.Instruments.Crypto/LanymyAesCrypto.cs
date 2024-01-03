using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments.Interfaces;


namespace Lanymy.Common.Instruments
{
    public class LanymyAesCrypto : IAesCrypto
    {


        private const int DEFAULT_SECURITY_KEY_SIZE = 32;
        private const int DEFAULT_SECURITY_IV_SIZE = 16;

        /// <summary>
        /// 默认密钥
        /// </summary>
        private const string DEFAULT_CRYPTO_KEY = DefaultSettingKeys.DEFAULT_CRYPTO_KEY;

        private static readonly byte[] DEFAULT_SECURITY_KEY_BYTES = new byte[DEFAULT_SECURITY_KEY_SIZE]
        {
            222,
            84,
            240,
            10,
            8,
            100,
            145,
            123,
            207,
            19,
            151,
            245,
            229,
            64,
            215,
            157,
            92,
            176,
            244,
            125,
            103,
            188,
            138,
            183,
            61,
            120,
            110,
            188,
            252,
            101,
            192,
            113,
        };


        private static readonly byte[] DEFAULT_SECURITY_IV_BYTES = new byte[DEFAULT_SECURITY_IV_SIZE]
        {
            51,
            188,
            99,
            231,
            133,
            54,
            72,
            118,
            220,
            137,
            12,
            93,
            172,
            201,
            251,
            146,

        };



        private byte[] GetSecurityKeyBytes(string securityKey, Encoding encoding)
        {

            byte[] keyBytes;

            byte[] bytes = encoding.GetBytes(securityKey);

            if (bytes.Length >= DEFAULT_SECURITY_KEY_SIZE)
            {
                keyBytes = bytes.Take(DEFAULT_SECURITY_KEY_SIZE).ToArray();
            }
            else
            {

                keyBytes = new byte[DEFAULT_SECURITY_KEY_SIZE];
                Array.Copy(DEFAULT_SECURITY_KEY_BYTES, keyBytes, DEFAULT_SECURITY_KEY_SIZE);
                Array.Copy(bytes, keyBytes, bytes.Length);

            }

            return keyBytes;

        }


        private byte[] GetSecurityIvBytes(string iv, Encoding encoding)
        {

            byte[] ivBytes;

            byte[] bytes = encoding.GetBytes(iv);

            if (bytes.Length >= DEFAULT_SECURITY_IV_SIZE)
            {
                ivBytes = bytes.Take(DEFAULT_SECURITY_IV_SIZE).ToArray();
            }
            else
            {

                ivBytes = new byte[DEFAULT_SECURITY_IV_SIZE];
                Array.Copy(DEFAULT_SECURITY_IV_BYTES, ivBytes, DEFAULT_SECURITY_IV_SIZE);
                Array.Copy(bytes, ivBytes, bytes.Length);

            }

            return ivBytes;

        }


        public byte[] EncryptBytesToBteys(byte[] sourceBytes, string key = null, string iv = null, Encoding encoding = null)
        {

            if (key.IfIsNullOrEmpty()) key = DEFAULT_CRYPTO_KEY;
            if (iv.IfIsNullOrEmpty()) iv = DEFAULT_CRYPTO_KEY;
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

            byte[] keyBytes = GetSecurityKeyBytes(key, encoding);
            byte[] ivBytes = GetSecurityIvBytes(iv, encoding);

            using var aes = Aes.Create();

            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7; // 使用PKCS7填充方式，适用于AES加密算法
            aes.KeySize = 256; // 设置密钥长度
            aes.BlockSize = 128; // 设置块大小

            aes.Key = keyBytes; // 设置密钥
            aes.IV = ivBytes; // 设置初始化向量



            using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using MemoryStream msEncrypt = new MemoryStream();
            using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            csEncrypt.Write(sourceBytes, 0, sourceBytes.Length);
            csEncrypt.FlushFinalBlock();


            //return msEncrypt.ToArray();
            return CompressionHelper.CompressBytesToBytes(msEncrypt.ToArray());

        }


        public byte[] DecryptBytesFromBteys(byte[] encryptBytes, string key = null, string iv = null, Encoding encoding = null)
        {

            if (key.IfIsNullOrEmpty()) key = DEFAULT_CRYPTO_KEY;
            if (iv.IfIsNullOrEmpty()) iv = DEFAULT_CRYPTO_KEY;
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

            byte[] keyBytes = GetSecurityKeyBytes(key, encoding);
            byte[] ivBytes = GetSecurityIvBytes(iv, encoding);

            using var aes = Aes.Create();



            aes.KeySize = 256; // 设置密钥长度
            aes.BlockSize = 128; // 设置块大小
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7; // 使用PKCS7填充方式，适用于AES加密算法


            aes.Key = keyBytes; // 设置密钥
            aes.IV = ivBytes; // 设置初始化向量


            using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using MemoryStream msDecrypt = new MemoryStream();
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write);

            var bytes = CompressionHelper.DecompressBytesFromBytes(encryptBytes);

            csDecrypt.Write(bytes, 0, bytes.Length);
            csDecrypt.FlushFinalBlock();


            return msDecrypt.ToArray();


        }


        public byte[] EncryptStringToBteys(string sourceString, string key = null, string iv = null, Encoding encoding = null)
        {
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;
            return EncryptBytesToBteys(encoding.GetBytes(sourceString), key, iv, encoding);
        }

        public string DecryptStringFromBteys(byte[] encrypBytes, string key = null, string iv = null, Encoding encoding = null)
        {
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;
            var bytes = DecryptBytesFromBteys(encrypBytes, key, iv, encoding);
            return encoding.GetString(bytes);
        }

        public string EncryptStringToString(string sourceString, string key = null, string iv = null, Encoding encoding = null)
        {

            var bytes = EncryptStringToBteys(sourceString, key, iv, encoding);

            return Convert.ToBase64String(bytes);

        }

        public string DecryptStringFromString(string encryptString, string key = null, string iv = null, Encoding encoding = null)
        {

            var bytes = Convert.FromBase64String(encryptString);
            return DecryptStringFromBteys(bytes, key, iv, encoding);

        }

    }

}