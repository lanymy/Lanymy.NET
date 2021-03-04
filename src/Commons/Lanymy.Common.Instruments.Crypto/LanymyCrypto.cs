using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.Enums;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments
{


    /// <summary>
    /// 加密/解密 操作器
    /// </summary>
    public class LanymyCrypto : BaseCrypto
    {


        private const string DEFAULT_CRYPTO_SPLIT_STRING = DefaultSettingKeys.DEFAULT_CRYPTO_SPLIT_STRING;

        /// <summary>
        /// 默认密钥
        /// </summary>
        private const string DEFAULT_CRYPTO_KEY = DefaultSettingKeys.DEFAULT_CRYPTO_KEY;


        /// <summary>
        /// 默认密钥长度
        /// </summary>
        private const int DEFAULT_CRYPTO_KEY_SIZE = DefaultSettingKeys.DEFAULT_CRYPTO_KEY_SIZE;


        /// <summary>
        /// 加密文件头摘要信息的长度 二进制数组的 长度 4
        /// </summary>
        private const int CRYPTO_HEADER_INFO_BYTES_LENGTH = sizeof(int);

        /// <summary>
        /// 加密 后 正文二进制数组 长度 8
        /// </summary>
        private const int CRYPTO_AFTER_CONTENT_BYTES_LENGTH = sizeof(long);

        /// <summary>
        /// 加密种子头部标识数据长度 17
        /// </summary>
        private const int CRYPTO_RANDOM_HEADER_FLAG_DATA_LENGTH = 17;

        /// <summary>
        /// 二进制 数组 SHA1 哈希散列算法 哈希值 长度
        /// </summary>
        private const int BYTES_HASH_CODE_LENGTH = 40;

        /// <summary>
        /// 当前 哈希散列算法 加密类型
        /// </summary>
        private readonly HashAlgorithmTypeEnum _CurrentHashAlgorithmType = HashAlgorithmTypeEnum.SHA1;

        //public LanymyCrypto(HashAlgorithmTypeEnum hashAlgorithmType = HashAlgorithmTypeEnum.SHA1)
        //{

        //    _CurrentHashAlgorithmType = hashAlgorithmType;

        //}


        private byte[] GetSecurityKey16Bytes(string securityKey, Encoding encoding)
        {

            byte[] bytes = encoding.GetBytes(securityKey);

            return bytes.Length >= DEFAULT_CRYPTO_KEY_SIZE ? bytes.Take(DEFAULT_CRYPTO_KEY_SIZE).ToArray() : ArrayHelper.MergerArray(bytes, new byte[DEFAULT_CRYPTO_KEY_SIZE - bytes.Length]);

        }




        public override EncryptDigestInfoModel EncryptStreamToStream(Stream sourceStream, Stream encryptStream, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {
            return EncryptStreamToStream<EncryptDigestInfoModel>(sourceStream, encryptStream, secretKey, ifRandom, encoding);
        }


        public override TEncryptDigestInfoModel EncryptStreamToStream<TEncryptDigestInfoModel>(Stream sourceStream, Stream encryptStream, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {


            if (secretKey.IfIsNullOrEmpty()) secretKey = DEFAULT_CRYPTO_KEY;
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

            //密钥必须16位
            byte[] secretKeyBytes = GetSecurityKey16Bytes(secretKey, encoding);

            long contentStartPosition = 0;

            var encryptDigestInfoModel = new TEncryptDigestInfoModel();

            using (var compressionStream = new GZipStream(encryptStream, CompressionMode.Compress, true))
            using (ICryptoTransform transform = new TripleDESCryptoServiceProvider().CreateEncryptor(secretKeyBytes, secretKeyBytes))
            using (CryptoStream cryptoStream = new CryptoStream(compressionStream, transform, CryptoStreamMode.Write))
            {

                //byte序列说明 byte1随机种子标识位 (----17位时间戳随机数----) utf8编码40头摘要哈希值 int4头摘要长度 byte*头摘要信息(不使用随机种子加密的时候,不能带时间日期属性,否则加密结果信息无法每次固定一致) long8正文大小 utf8编码40正文哈希值  

                //if (encryptDigestInfoModel.IfIsNullOrEmpty())
                //{
                //    encryptDigestInfoModel
                //}

                //encryptDigestInfoModel.SecurityEncryptDirectionType = securityEncryptDirectionType;
                encryptDigestInfoModel.IsRandomEncrypt = ifRandom;
                encryptDigestInfoModel.SourceBytesSize = sourceStream.Length;
                encryptDigestInfoModel.SourceBytesHashCode = FileHelper.GetStreamHashCode(sourceStream, hashAlgorithmType: _CurrentHashAlgorithmType);
                encryptDigestInfoModel.CreateDateTime = null;

                sourceStream.Position = 0;


                //写入 随机加密 标识符
                encryptStream.Write(new[] { ifRandom ? (byte)1 : (byte)0 }, 0, 1);


                if (ifRandom)
                {

                    //当不使用随机加密时,此属性没有实际意义,因为无法把不固定的时间戳编码到加密后二进制序列中,使加密后二进制序列每次加密结果都一致
                    //只有在随机加密情况下 才会 赋值时间戳
                    encryptDigestInfoModel.CreateDateTime = DateTime.Now;

                    string headerFlagDataString = new string(DateTime.Now.ToString(DateTimeFormatKeys.DATE_TIME_FORMAT_2).Reverse().ToArray());
                    var headerFlagDataStringBytes = Encoding.UTF8.GetBytes(headerFlagDataString);

                    //写入随机种子标识位
                    encryptStream.Write(headerFlagDataStringBytes, 0, CRYPTO_RANDOM_HEADER_FLAG_DATA_LENGTH);

                }


                //头摘要实体类二进制数据
                var encryptModelJson = JsonSerializeHelper.SerializeToJson(encryptDigestInfoModel);
                var encryptModelJsonBytes = CompressionHelper.CompressStringToBytes(encryptModelJson, encoding);
                //头摘要实体类二进制数据 长度的二进制数据
                int encryptModelBytesLength = encryptModelJsonBytes.Length;
                var encryptModelBytesLengthBytes = BitConverter.GetBytes(encryptModelBytesLength);
                //头摘要实体类二进制数据 哈希值 二进制数据
                var encryptModelJsonBytesHashCode = FileHelper.GetBytesHashCode(encryptModelJsonBytes, hashAlgorithmType: _CurrentHashAlgorithmType);
                var encryptModelJsonBytesHashCodeBytes = encoding.GetBytes(encryptModelJsonBytesHashCode);

                //写入 头摘要 哈希值
                encryptStream.Write(encryptModelJsonBytesHashCodeBytes, 0, BYTES_HASH_CODE_LENGTH);

                //写入头摘要长度
                encryptStream.Write(encryptModelBytesLengthBytes, 0, CRYPTO_HEADER_INFO_BYTES_LENGTH);

                //写入头摘要数据
                encryptStream.Write(encryptModelJsonBytes, 0, encryptModelJsonBytes.Length);

                //写入 正文 大小占位符
                encryptStream.Write(new byte[CRYPTO_AFTER_CONTENT_BYTES_LENGTH], 0, CRYPTO_AFTER_CONTENT_BYTES_LENGTH);

                //写入 正文 大小哈希值 占位符
                encryptStream.Write(new byte[BYTES_HASH_CODE_LENGTH], 0, BYTES_HASH_CODE_LENGTH);

                encryptStream.Flush();

                contentStartPosition = encryptStream.Position;

                //写入 正文 数据流
                //sourceStream.CopyToAsync(cryptoStream).Wait();
                sourceStream.CopyTo(cryptoStream);
                cryptoStream.FlushFinalBlock();

            }


            encryptStream.Flush();



            //加密后数据流总大小
            long encryptAfterBytesLength = encryptStream.Length;
            //目前此属性值还没用到 暂弃用
            //var encryptAfterBytesLengthBytes = BitConverter.GetBytes(encryptAfterBytesLength);

            //加密后 正文数据流 大小
            long encryptAfterContentBytesLength = encryptAfterBytesLength - contentStartPosition;
            var encryptAfterContentBytesLengthBytes = BitConverter.GetBytes(encryptAfterContentBytesLength);

            //加密后 正文数据流 哈希值
            var encryptAfterContentBytesHashCode = FileHelper.GetStreamHashCode(encryptStream, (int)contentStartPosition, _CurrentHashAlgorithmType);
            var encryptAfterContentBytesHashCodeBytes = encoding.GetBytes(encryptAfterContentBytesHashCode);

            encryptStream.Position = contentStartPosition - BYTES_HASH_CODE_LENGTH - CRYPTO_AFTER_CONTENT_BYTES_LENGTH;

            //写入正文大小二进制数据
            encryptStream.Write(encryptAfterContentBytesLengthBytes, 0, CRYPTO_AFTER_CONTENT_BYTES_LENGTH);

            //写入正文哈希值二进制数据
            encryptStream.Write(encryptAfterContentBytesHashCodeBytes, 0, BYTES_HASH_CODE_LENGTH);

            encryptStream.Flush();

            //加密后完整数据流哈希值
            var encryptAfterBytesHashCode = FileHelper.GetStreamHashCode(encryptStream, hashAlgorithmType: _CurrentHashAlgorithmType);

            encryptDigestInfoModel.EncryptBytesSize = encryptAfterBytesLength;
            encryptDigestInfoModel.EncryptBytesHashCode = encryptAfterBytesHashCode;
            encryptDigestInfoModel.EncryptContentBytesSize = encryptAfterContentBytesLength;
            encryptDigestInfoModel.EncryptContentBytesHashCode = encryptAfterContentBytesHashCode;

            encryptStream.Position = encryptAfterBytesLength;

            //var encryptResultModel = new EncryptResultModel<TEncryptDigestInfoModel>
            //{
            //    IsSuccess = true,
            //    HeaderInfoModel = encryptDigestInfoModel,
            //};

            //return encryptResultModel;

            encryptDigestInfoModel.IsSuccess = true;

            return encryptDigestInfoModel;

        }



        public override EncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedStream(Stream encryptedStream, Encoding encoding = null)
        {
            return GetEncryptDigestInfoModelFromEncryptedStream<EncryptDigestInfoModel>(encryptedStream, encoding);
        }



        public override TEncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedStream<TEncryptDigestInfoModel>(Stream encryptedStream, Encoding encoding = null)
        {

            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

            var encryptResultModel = new TEncryptDigestInfoModel
            {
                IsSuccess = false,
            };

            encryptedStream.Position = 0;

            //提取随机种子标识位
            var ifRandomBytes = new byte[1];
            encryptedStream.Read(ifRandomBytes, 0, 1);

            bool ifRandom = ifRandomBytes[0] != 0;

            if (ifRandom)
            {

                ////提取随机种子标识位
                //var headerFlagDataStringBytes = new byte[CRYPTO_RANDOM_HEADER_FLAG_DATA_LENGTH];
                //encryptedStream.Read(headerFlagDataStringBytes, 0, CRYPTO_RANDOM_HEADER_FLAG_DATA_LENGTH);
                //var headerFlagDataString = new string(Encoding.UTF8.GetString(headerFlagDataStringBytes).Reverse().ToArray());
                //encryptResultModel.CreateDateTime = DateTime.ParseExact(headerFlagDataString, DateTimeFormatKeys.DATE_TIME_FORMAT_2, null);


                //创建时间头部摘要信息中有 此处直接跳过时间戳长度即可
                encryptedStream.Position += CRYPTO_RANDOM_HEADER_FLAG_DATA_LENGTH;

            }

            //提取头摘要哈希值
            var encryptModelJsonBytesHashCodeBytes = new byte[BYTES_HASH_CODE_LENGTH];
            encryptedStream.Read(encryptModelJsonBytesHashCodeBytes, 0, BYTES_HASH_CODE_LENGTH);
            var encryptModelJsonBytesHashCode = encoding.GetString(encryptModelJsonBytesHashCodeBytes);

            //提取头摘要长度
            var encryptModelBytesLengthBytes = new byte[CRYPTO_HEADER_INFO_BYTES_LENGTH];
            encryptedStream.Read(encryptModelBytesLengthBytes, 0, CRYPTO_HEADER_INFO_BYTES_LENGTH);
            int encryptModelBytesLength = BitConverter.ToInt32(encryptModelBytesLengthBytes, 0);

            //提取头摘要信息
            var encryptModelJsonBytes = new byte[encryptModelBytesLength];
            encryptedStream.Read(encryptModelJsonBytes, 0, encryptModelBytesLength);
            var encryptModelHeaderJsonBytesHashCode = FileHelper.GetBytesHashCode(encryptModelJsonBytes, hashAlgorithmType: _CurrentHashAlgorithmType);

            if (encryptModelJsonBytesHashCode != encryptModelHeaderJsonBytesHashCode)
            {
                encryptResultModel.ErrorMessage = "指纹信息效验失败,无法继续解析";
                return encryptResultModel;
            }



            //EncryptDigestInfoModel encryptHeaderModel;

            try
            {

                var encryptModelJson = CompressionHelper.DecompressStringFromBytes(encryptModelJsonBytes);
                encryptResultModel = JsonSerializeHelper.DeserializeFromJson<TEncryptDigestInfoModel>(encryptModelJson);
                encryptResultModel.DencryptHeaderInfoModelJsonString = encryptModelJson;

            }
            catch (Exception e)
            {
                encryptResultModel.ErrorMessage = "指纹信息解析失败,无法继续解析";
                return encryptResultModel;
            }


            //提取正文大小
            var encryptAfterContentBytesLengthBytes = new byte[CRYPTO_AFTER_CONTENT_BYTES_LENGTH];
            encryptedStream.Read(encryptAfterContentBytesLengthBytes, 0, CRYPTO_AFTER_CONTENT_BYTES_LENGTH);
            long encryptAfterContentBytesLength = BitConverter.ToInt64(encryptAfterContentBytesLengthBytes, 0);
            //提取正文哈希值
            var encryptAfterContentBytesHashCodeBytes = new byte[BYTES_HASH_CODE_LENGTH];
            encryptedStream.Read(encryptAfterContentBytesHashCodeBytes, 0, BYTES_HASH_CODE_LENGTH);
            var encryptAfterContentBytesHashCode = encoding.GetString(encryptAfterContentBytesHashCodeBytes);

            var currentPosition = encryptedStream.Position;

            var encryptedAfterContentBytesHashCode = FileHelper.GetStreamHashCode(encryptedStream, (int)currentPosition, _CurrentHashAlgorithmType);

            if (encryptAfterContentBytesHashCode != encryptedAfterContentBytesHashCode)
            {
                encryptResultModel.ErrorMessage = "加密信息效验失败,无法继续解析";
                return encryptResultModel;
            }

            //只有在随机加密情况下 时间戳才有值
            //encryptHeaderModel.CreateDateTime

            encryptResultModel.EncryptBytesSize = encryptedStream.Length;
            encryptResultModel.EncryptBytesHashCode = FileHelper.GetStreamHashCode(encryptedStream, hashAlgorithmType: _CurrentHashAlgorithmType);
            encryptResultModel.EncryptContentBytesSize = encryptAfterContentBytesLength;
            encryptResultModel.EncryptContentBytesHashCode = encryptAfterContentBytesHashCode;

            dynamic json = Newtonsoft.Json.Linq.JToken.Parse(encryptResultModel.DencryptHeaderInfoModelJsonString);
            json.EncryptBytesSize = encryptResultModel.EncryptBytesSize;
            json.EncryptBytesHashCode = encryptResultModel.EncryptBytesHashCode;
            json.EncryptContentBytesSize = encryptResultModel.EncryptContentBytesSize;
            json.EncryptContentBytesHashCode = encryptResultModel.EncryptContentBytesHashCode;
            var jsonNew = json.ToString();
            encryptResultModel.DencryptHeaderInfoModelJsonString = jsonNew;

            encryptedStream.Position = currentPosition;

            //encryptResultModel.IsSuccess = true;
            //encryptResultModel.HeaderInfoModel = encryptHeaderModel;

            //return encryptResultModel;

            encryptResultModel.IsSuccess = true;

            return encryptResultModel;

        }


        public override EncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedFile(string encryptedFileFullPath, Encoding encoding = null)
        {
            return GetEncryptDigestInfoModelFromEncryptedFile<EncryptDigestInfoModel>(encryptedFileFullPath, encoding);
        }


        public override TEncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedFile<TEncryptDigestInfoModel>(string encryptedFileFullPath, Encoding encoding = null)
        {

            TEncryptDigestInfoModel encryptDigestInfoModel;

            using (var encryptedStream = File.OpenRead(encryptedFileFullPath))
            {
                //encryptDigestInfoModel = GetEncryptDigestInfoModelFromEncryptedStream(encryptedStream, secretKey, encoding);
                encryptDigestInfoModel = GetEncryptDigestInfoModelFromEncryptedStream<TEncryptDigestInfoModel>(encryptedStream, encoding);

            }

            return encryptDigestInfoModel;
        }


        public override EncryptDigestInfoModel DencryptStreamFromStream(Stream encryptedStream, Stream sourceStream, string secretKey = null, Encoding encoding = null)
        {
            return DencryptStreamFromStream<EncryptDigestInfoModel>(encryptedStream, sourceStream, secretKey, encoding);
        }


        public override TEncryptDigestInfoModel DencryptStreamFromStream<TEncryptDigestInfoModel>(Stream encryptedStream, Stream sourceStream, string secretKey = null, Encoding encoding = null)
        {

            if (secretKey.IfIsNullOrEmpty()) secretKey = DEFAULT_CRYPTO_KEY;
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

            var encryptResultModel = GetEncryptDigestInfoModelFromEncryptedStream<TEncryptDigestInfoModel>(encryptedStream, encoding);

            if (!encryptResultModel.IsSuccess)
            {
                return encryptResultModel;
            }

            //密钥必须16位
            byte[] secretKeyBytes = GetSecurityKey16Bytes(secretKey, encoding);

            using (var decompressionStream = new GZipStream(encryptedStream, CompressionMode.Decompress, true))
            using (ICryptoTransform transform = new TripleDESCryptoServiceProvider().CreateDecryptor(secretKeyBytes, secretKeyBytes))
            using (CryptoStream cryptoStream = new CryptoStream(decompressionStream, transform, CryptoStreamMode.Read))
            {

                //cryptoStream.CopyToAsync(sourceStream).Wait();
                cryptoStream.CopyTo(sourceStream);

            }

            return encryptResultModel;

        }



        public override EncryptBytesDigestInfoModel EncryptBytesToBytes(byte[] bytesToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {
            return EncryptBytesToBytes<EncryptBytesDigestInfoModel>(bytesToEncrypt, secretKey, ifRandom, encoding);
        }



        /// <summary>
        /// 二进制数组加密 返回加密后的二进制数组
        /// </summary>
        /// <param name="bytesToEncrypt">要加密的二进制数组</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public override TEncryptDigestInfoModel EncryptBytesToBytes<TEncryptDigestInfoModel>(byte[] bytesToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {

            if (bytesToEncrypt.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(bytesToEncrypt));

            TEncryptDigestInfoModel encryptBytesDigestInfoModel;

            using (var sourceStream = new MemoryStream(bytesToEncrypt))
            using (var encryptStream = new MemoryStream())
            {
                encryptBytesDigestInfoModel = EncryptStreamToStream<TEncryptDigestInfoModel>(sourceStream, encryptStream, secretKey, ifRandom, encoding);
                //encryptBytesDigestInfoModel = encryptDigestInfoModel.AsTypeByDeepClone<EncryptDigestInfoModel, EncryptBytesDigestInfoModel>();
                if (encryptBytesDigestInfoModel.IsSuccess)
                {
                    encryptBytesDigestInfoModel.SourceBytes = bytesToEncrypt;
                    encryptBytesDigestInfoModel.EncryptedBytes = encryptStream.ToArray();
                }
            }


            return encryptBytesDigestInfoModel;

        }


        public override EncryptBytesDigestInfoModel DecryptBytesFromBytes(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null)
        {
            return DecryptBytesFromBytes<EncryptBytesDigestInfoModel>(bytesToDecrypt, secretKey, encoding);
        }


        /// <summary>
        /// 解密二进制数组 返回 解密后的二进制数组
        /// </summary>
        /// <param name="bytesToDecrypt">要解密的二进制数组</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="encoding">解密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public override TEncryptDigestInfoModel DecryptBytesFromBytes<TEncryptDigestInfoModel>(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null)
        {

            if (bytesToDecrypt.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(bytesToDecrypt));

            TEncryptDigestInfoModel encryptBytesResultModel;

            using (var sourceStream = new MemoryStream())
            using (var encryptedStream = new MemoryStream(bytesToDecrypt))
            {

                encryptBytesResultModel = DencryptStreamFromStream<TEncryptDigestInfoModel>(encryptedStream, sourceStream, secretKey, encoding);

                if (encryptBytesResultModel.IsSuccess)
                {

                    encryptBytesResultModel.SourceBytes = sourceStream.ToArray();

                }

            }


            return encryptBytesResultModel;

        }







        public override EncryptStringDigestInfoModel EncryptStringToBytes(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {

            return EncryptStringToBytes<EncryptStringDigestInfoModel>(strToEncrypt, secretKey, ifRandom, encoding);

        }




        /// <summary>
        /// 字符串加密 返回 加密后的二进制数组
        /// </summary>
        /// <param name="strToEncrypt">要加密的字符串</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public override TEncryptDigestInfoModel EncryptStringToBytes<TEncryptDigestInfoModel>(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {

            if (strToEncrypt.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(strToEncrypt));
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

            var bytesToEncrypt = encoding.GetBytes(strToEncrypt);

            var encryptStringDigestInfoModel = EncryptBytesToBytes<TEncryptDigestInfoModel>(bytesToEncrypt, secretKey, ifRandom, encoding);

            if (encryptStringDigestInfoModel.IsSuccess)
            {
                //encryptStringDigestInfoModel.SourceBytes = null;
                encryptStringDigestInfoModel.SourceString = strToEncrypt;
            }

            return encryptStringDigestInfoModel;

        }


        public override EncryptStringDigestInfoModel DecryptStringFromBytes(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null)
        {
            return DecryptStringFromBytes<EncryptStringDigestInfoModel>(bytesToDecrypt, secretKey, encoding);
        }




        /// <summary>
        /// 解密二进制数组 返回 解密后的字符串
        /// </summary>
        /// <param name="bytesToDecrypt">要解密的二进制数组</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="encoding">解密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public override TEncryptDigestInfoModel DecryptStringFromBytes<TEncryptDigestInfoModel>(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null)
        {

            if (bytesToDecrypt.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(bytesToDecrypt));
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

            var encryptDigestInfoModel = DecryptBytesFromBytes<TEncryptDigestInfoModel>(bytesToDecrypt, secretKey, encoding);

            if (encryptDigestInfoModel.IsSuccess)
            {

                encryptDigestInfoModel.SourceString = encoding.GetString(encryptDigestInfoModel.SourceBytes);
                //encryptDigestInfoModel.SourceBytes = null;

            }

            return encryptDigestInfoModel;

        }






        public override EncryptBase64StringDigestInfoModel EncryptStringToBase64String(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {

            return EncryptStringToBase64String<EncryptBase64StringDigestInfoModel>(strToEncrypt, secretKey, ifRandom, encoding);

        }





        /// <summary>
        /// 字符串加密 返回 加密后的 Base64 字符串
        /// </summary>
        /// <param name="strToEncrypt">要加密的字符串</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public override TEncryptDigestInfoModel EncryptStringToBase64String<TEncryptDigestInfoModel>(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {


            var encryptStringDigestInfoModel = EncryptStringToBytes<TEncryptDigestInfoModel>(strToEncrypt, secretKey, ifRandom, encoding);

            if (encryptStringDigestInfoModel.IsSuccess)
            {

                encryptStringDigestInfoModel.EncryptedBase64String = Convert.ToBase64String(encryptStringDigestInfoModel.EncryptedBytes);

            }

            return encryptStringDigestInfoModel;


        }



        public override EncryptBase64StringDigestInfoModel DecryptStringFromBase64String(string base64StringToDecrypt, string secretKey = null, Encoding encoding = null)
        {
            return DecryptStringFromBase64String<EncryptBase64StringDigestInfoModel>(base64StringToDecrypt, secretKey, encoding);
        }



        /// <summary>
        /// 解密字符串 返回 解密后的字符串
        /// </summary>
        /// <param name="base64StringToDecrypt">要解密的Base64字符串</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="encoding">解密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public override TEncryptDigestInfoModel DecryptStringFromBase64String<TEncryptDigestInfoModel>(string base64StringToDecrypt, string secretKey = null, Encoding encoding = null)
        {

            if (base64StringToDecrypt.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(base64StringToDecrypt));

            var encryptDigestInfoModel = DecryptStringFromBytes<TEncryptDigestInfoModel>(Convert.FromBase64String(base64StringToDecrypt), secretKey, encoding);

            if (encryptDigestInfoModel.IsSuccess)
            {

                encryptDigestInfoModel.EncryptedBase64String = base64StringToDecrypt;

            }

            return encryptDigestInfoModel;

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
        public override EncryptModelDigestInfoModel<T> EncryptModelToBytes<T>(T t, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where T : class
        {

            //if (encoding.IfIsNullOrEmpty()) encoding = GlobalSettings.DEFAULT_ENCODING;

            var jsonString = JsonSerializeHelper.SerializeToJson(t);
            var encryptModelDigestInfoModel = EncryptStringToBase64String<EncryptModelDigestInfoModel<T>>(jsonString, secretKey, ifRandom, encoding);

            if (encryptModelDigestInfoModel.IsSuccess)
            {

                encryptModelDigestInfoModel.SourceModel = t;

                var modelType = typeof(T);
                encryptModelDigestInfoModel.ModelTypeName = modelType.Name;
                encryptModelDigestInfoModel.ModelTypeFullName = modelType.FullName;


                //encryptModelDigestInfoModel.SourceString = string.Empty;
                //encryptModelDigestInfoModel.EncryptedBase64String = string.Empty;

            }

            return encryptModelDigestInfoModel;


        }


        /// <summary>
        /// 解密并反序列化字节数组 返回 Model
        /// </summary>
        /// <typeparam name="T">要解密并反序列化的实体类型</typeparam>
        /// <param name="ecryptedBytes">要解密并反序列化的实体实例</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="encoding">编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public override EncryptModelDigestInfoModel<T> DecryptModelFromBytes<T>(byte[] ecryptedBytes, string secretKey = null, Encoding encoding = null) where T : class
        {

            //if (encoding.IfIsNullOrEmpty()) encoding = GlobalSettings.DEFAULT_ENCODING;


            var encryptModelDigestInfoModel = DecryptStringFromBytes<EncryptModelDigestInfoModel<T>>(ecryptedBytes, secretKey, encoding);

            if (encryptModelDigestInfoModel.IsSuccess)
            {

                encryptModelDigestInfoModel.SourceModel = JsonSerializeHelper.DeserializeFromJson<T>(encryptModelDigestInfoModel.SourceString);
                //encryptModelDigestInfoModel.SourceString = string.Empty;

            }

            return encryptModelDigestInfoModel;

        }


        /// <summary>
        /// 加密并序列化Model成Base64字符串
        /// </summary>
        /// <typeparam name="T">要序列化的实体类型</typeparam>
        /// <param name="t">要序列化的实体 实例</param>
        /// <param name="secretKey">密钥 Null 使用默认密钥</param>
        /// <returns></returns>
        public override EncryptModelDigestInfoModel<T> EncryptModelToBase64String<T>(T t, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where T : class
        {

            return EncryptModelToBytes(t, secretKey, ifRandom, encoding);

        }


        /// <summary>
        /// 解密并反序列化Base64字符串成实体实例
        /// </summary>
        /// <typeparam name="T">要反序列化成的实体类型</typeparam>
        /// <param name="encryptBase64String">要解密的Base64字符串</param>
        /// <param name="secretKey">密钥 Null 使用默认密钥</param>
        /// <returns></returns>
        public override EncryptModelDigestInfoModel<T> DecryptModelFromBase64String<T>(string encryptBase64String, string secretKey = null, Encoding encoding = null) where T : class
        {
            if (encryptBase64String.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(encryptBase64String));
            return DecryptModelFromBytes<T>(Convert.FromBase64String(encryptBase64String), secretKey, encoding);
        }













        public override EncryptStringFileDigestInfoModel EncryptBytesToFile(byte[] sourceBytes, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {
            return EncryptBytesToFile<EncryptStringFileDigestInfoModel>(sourceBytes, encryptFileFullPath, secretKey, ifRandom, encoding);
        }



        public override TEncryptDigestInfoModel EncryptBytesToFile<TEncryptDigestInfoModel>(byte[] sourceBytes, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {


            if (sourceBytes.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(sourceBytes));

            TEncryptDigestInfoModel encryptBytesDigestInfoModel;
            using (var sourceStream = new MemoryStream(sourceBytes))
            using (var encryptStream = File.Create(encryptFileFullPath, BufferSizeKeys.BUFFER_SIZE_4K))
            {

                encryptBytesDigestInfoModel = EncryptStreamToStream<TEncryptDigestInfoModel>(sourceStream, encryptStream, secretKey, ifRandom, encoding);
                if (encryptBytesDigestInfoModel.IsSuccess)
                {
                    encryptBytesDigestInfoModel.EncryptedFileFullPath = encryptFileFullPath;
                }

            }

            return encryptBytesDigestInfoModel;

        }




        public override EncryptStringFileDigestInfoModel DecryptBytesFromFile(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null)
        {
            return DecryptBytesFromFile<EncryptStringFileDigestInfoModel>(encryptedFileFullPath, secretKey, encoding);
        }


        public override TEncryptDigestInfoModel DecryptBytesFromFile<TEncryptDigestInfoModel>(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null)
        {

            TEncryptDigestInfoModel encryptStringFileDigestInfoModel;

            using (var sourceStream = new MemoryStream())
            using (var encryptedStream = File.OpenRead(encryptedFileFullPath))
            {
                encryptStringFileDigestInfoModel = DencryptStreamFromStream<TEncryptDigestInfoModel>(encryptedStream, sourceStream, secretKey, encoding);

                if (encryptStringFileDigestInfoModel.IsSuccess)
                {
                    encryptStringFileDigestInfoModel.EncryptedFileFullPath = encryptedFileFullPath;
                }

            }

            return encryptStringFileDigestInfoModel;

        }




        public override EncryptStringFileDigestInfoModel EncryptStringToFile(string sourceString, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {
            return EncryptStringToFile<EncryptStringFileDigestInfoModel>(sourceString, encryptFileFullPath, secretKey, ifRandom, encoding);
        }



        public override TEncryptDigestInfoModel EncryptStringToFile<TEncryptDigestInfoModel>(string sourceString, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {

            if (sourceString.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(sourceString));
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;


            var encryptStringFileDigestInfoModel = EncryptBytesToFile<TEncryptDigestInfoModel>(encoding.GetBytes(sourceString), encryptFileFullPath, secretKey, ifRandom, encoding);
            if (encryptStringFileDigestInfoModel.IsSuccess)
            {
                encryptStringFileDigestInfoModel.EncryptedFileFullPath = encryptFileFullPath;
            }

            return encryptStringFileDigestInfoModel;

        }


        public override EncryptStringFileDigestInfoModel DecryptStringFromFile(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null)
        {
            return DecryptStringFromFile<EncryptStringFileDigestInfoModel>(encryptedFileFullPath, secretKey, encoding);
        }

        public override TEncryptDigestInfoModel DecryptStringFromFile<TEncryptDigestInfoModel>(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null)
        {

            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

            var encryptStringDigestInfoModel = DecryptBytesFromFile<TEncryptDigestInfoModel>(encryptedFileFullPath, secretKey, encoding);

            if (encryptStringDigestInfoModel.IsSuccess)
            {
                encryptStringDigestInfoModel.EncryptedFileFullPath = encryptedFileFullPath;
            }

            return encryptStringDigestInfoModel;

        }




        public override EncryptModelFileDigestInfoModel<T> EncryptModelToFile<T>(T t, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where T : class
        {

            var json = JsonSerializeHelper.SerializeToJson(t);
            var encryptModelFileDigestInfoModel = EncryptStringToFile<EncryptModelFileDigestInfoModel<T>>(json, encryptFileFullPath, secretKey, ifRandom, encoding);

            if (encryptModelFileDigestInfoModel.IsSuccess)
            {

                encryptModelFileDigestInfoModel.SourceModel = t;

                var modelType = typeof(T);
                encryptModelFileDigestInfoModel.ModelTypeName = modelType.Name;
                encryptModelFileDigestInfoModel.ModelTypeFullName = modelType.FullName;

                encryptModelFileDigestInfoModel.EncryptedFileFullPath = encryptFileFullPath;

            }

            return encryptModelFileDigestInfoModel;
        }


        public override EncryptModelFileDigestInfoModel<T> DecryptModelFromFile<T>(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null) where T : class
        {

            var encryptModelFileDigestInfoModel = DecryptStringFromFile<EncryptModelFileDigestInfoModel<T>>(encryptedFileFullPath, secretKey, encoding);

            if (encryptModelFileDigestInfoModel.IsSuccess)
            {

                encryptModelFileDigestInfoModel.SourceModel = JsonSerializeHelper.DeserializeFromJson<T>(encryptModelFileDigestInfoModel.SourceString);
                encryptModelFileDigestInfoModel.EncryptedFileFullPath = encryptedFileFullPath;

            }

            return encryptModelFileDigestInfoModel;

        }






        public override EncryptStringFileDigestInfoModel EncryptFileToFile(string sourceFileFullPath, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        {

            EncryptStringFileDigestInfoModel encryptFileDigestInfoModel;

            using (var sourceStream = File.OpenRead(sourceFileFullPath))
            using (var encryptStream = File.Create(encryptFileFullPath, BufferSizeKeys.BUFFER_SIZE_4K))
            {

                encryptFileDigestInfoModel = EncryptStreamToStream<EncryptStringFileDigestInfoModel>(sourceStream, encryptStream, secretKey, ifRandom, encoding);

                if (encryptFileDigestInfoModel.IsSuccess)
                {
                    encryptFileDigestInfoModel.SourceFileFullPath = sourceFileFullPath;
                    encryptFileDigestInfoModel.EncryptedFileFullPath = encryptFileFullPath;

                }


            }

            return encryptFileDigestInfoModel;

        }



        public override EncryptStringFileDigestInfoModel DecryptFileFromFile(string encryptedFileFullPath, string sourceFileFullPath, string secretKey = null, Encoding encoding = null)
        {
            EncryptStringFileDigestInfoModel encryptStringFileDigestInfoModel;
            using (var encryptedStream = File.OpenRead(encryptedFileFullPath))
            using (var sourceStream = File.Create(sourceFileFullPath, BufferSizeKeys.BUFFER_SIZE_4K))
            {
                encryptStringFileDigestInfoModel = DencryptStreamFromStream<EncryptStringFileDigestInfoModel>(encryptedStream, sourceStream, secretKey, encoding);
            }
            return encryptStringFileDigestInfoModel;
        }






        private Bitmap GetEncryptedBitmap(string strToBitmap)
        {


            var message = new StringBuilder(strToBitmap + DEFAULT_CRYPTO_SPLIT_STRING);

            int messageLength = message.Length;
            int endStrLength = DEFAULT_CRYPTO_SPLIT_STRING.Length;

            //补齐二进制序列成正方形阵列
            int sqrt = (int)Math.Sqrt(messageLength) + 1;
            int sqrtLength = sqrt * sqrt;

            var overlength = sqrtLength - messageLength;

            for (int i = 0; i < overlength / endStrLength; i++)
            {
                message.Append(DEFAULT_CRYPTO_SPLIT_STRING);
            }

            //for (int i = 0; i < overlength % endStrLength; i++)
            //{
            //    message.Append(endStr[i]);
            //}

            message.Append(DEFAULT_CRYPTO_SPLIT_STRING.Substring(0, overlength % endStrLength));

            var messageChars = message.ToString().ToArray();

            var random = new Random((int)DateTime.Now.Ticks);
            var image = new Bitmap(sqrt, sqrt);

            for (int i = 0; i < sqrt; i++)
            {
                for (int j = 0; j < sqrt; j++)
                {

                    //byte[] bytes = Encoding.UTF32.GetBytes(message[i * sqrt + j].ToString());
                    byte[] bytes = Encoding.UTF32.GetBytes(messageChars, i * sqrt + j, 1);
                    EncryptBitmapBytes(random, bytes);
                    image.SetPixel(i, j, System.Drawing.Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]));

                }
            }

            return image;

        }


        private void EncryptBitmapBytes(Random random, byte[] bitmapBytes)
        {

            //var random = new Random((int)DateTime.Now.Ticks);
            //var random = new Random(Guid.NewGuid().GetHashCode());
            var randomBytes = BitConverter.GetBytes(random.Next(int.MinValue, int.MaxValue));
            //除了第一个元素本值反转,其他三个全部随机翻转
            for (int i = 0; i < bitmapBytes.Length; i++)
            {
                if (i > 0)
                {
                    //bitmapBytes[i] = (byte)random.Next(0, 256);
                    bitmapBytes[i] = randomBytes[i];
                }
                bitmapBytes[i] = (byte)(255 - bitmapBytes[i]);
            }

            //bitmapBytes = bitmapBytes.Reverse().ToArray();

            //for (int i = 0; i < bitmapBytes.Length; i++)
            //{
            //    bitmapBytes[i] = (byte)(255 - bitmapBytes[i]);
            //}

            //return bitmapBytes;

        }



        private string GetStringFromEncryptedBitmap(Bitmap encryptedBitmap)
        {

            if (encryptedBitmap.Width != encryptedBitmap.Height)
            {
                throw new ArgumentException("不是有效的加密位图数据源");
            }

            var sb = new StringBuilder();

            for (int i = 0; i < encryptedBitmap.Width; i++)
            {
                for (int j = 0; j < encryptedBitmap.Height; j++)
                {
                    var color = encryptedBitmap.GetPixel(i, j);
                    var bytes = new[] { color.A, color.R, color.G, color.B };
                    DecryptBitmapBytes(bytes);
                    sb.Append(Encoding.UTF32.GetString(bytes));

                }
            }

            return sb.ToString().LeftSubString(DEFAULT_CRYPTO_SPLIT_STRING);

        }


        private static void DecryptBitmapBytes(byte[] bitmapBytes)
        {

            bitmapBytes[0] = (byte)(255 - bitmapBytes[0]);
            bitmapBytes[1] = 0;
            bitmapBytes[2] = 0;
            bitmapBytes[3] = 0;

            //return new byte[] { (byte)(255 - bitmapBytes[0]), 0, 0, 0 };

            //for (int i = 0; i < bitmapBytes.Length; i++)
            //{
            //    bitmapBytes[i] = (byte)(255 - bitmapBytes[i]);
            //}

            //return bitmapBytes.Reverse().ToArray();
        }


        public override EncryptStringBitmapDigestInfoModel EncryptBytesToBitmap(byte[] bytesToEncrypt, string secretKey = null, Encoding encoding = null)
        {
            return EncryptBytesToBitmap<EncryptStringBitmapDigestInfoModel>(bytesToEncrypt, secretKey, encoding);
        }



        public override TEncryptDigestInfoModel EncryptBytesToBitmap<TEncryptDigestInfoModel>(byte[] bytesToEncrypt, string secretKey = null, Encoding encoding = null)
        {

            var encryptStringBitmapDigestInfoModel = EncryptBytesToBytes<TEncryptDigestInfoModel>(bytesToEncrypt, secretKey, true, encoding);

            if (encryptStringBitmapDigestInfoModel.IsSuccess)
            {

                encryptStringBitmapDigestInfoModel.EncryptedBitmap = GetEncryptedBitmap(Convert.ToBase64String(encryptStringBitmapDigestInfoModel.EncryptedBytes));

            }

            return encryptStringBitmapDigestInfoModel;

        }


        public override EncryptStringBitmapDigestInfoModel DecryptBytesFromBitmap(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null)
        {
            return DecryptBytesFromBitmap<EncryptStringBitmapDigestInfoModel>(encryptedBitmap, secretKey, encoding);
        }


        public override TEncryptDigestInfoModel DecryptBytesFromBitmap<TEncryptDigestInfoModel>(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null)
        {



            var bitmapBase64String = GetStringFromEncryptedBitmap(encryptedBitmap);
            var encryptedBytes = Convert.FromBase64String(bitmapBase64String);
            var encryptBitmapDigestInfoModel = DecryptBytesFromBytes<TEncryptDigestInfoModel>(encryptedBytes, secretKey, encoding);

            if (encryptBitmapDigestInfoModel.IsSuccess)
            {
                encryptBitmapDigestInfoModel.EncryptedBitmap = encryptedBitmap;
            }

            return encryptBitmapDigestInfoModel;

        }


        public override EncryptStringBitmapDigestInfoModel EncryptStringToBitmap(string strToEncrypt, string secretKey = null, Encoding encoding = null)
        {
            return EncryptStringToBitmap<EncryptStringBitmapDigestInfoModel>(strToEncrypt, secretKey, encoding);
        }


        public override TEncryptDigestInfoModel EncryptStringToBitmap<TEncryptDigestInfoModel>(string strToEncrypt, string secretKey = null, Encoding encoding = null)
        {

            if (strToEncrypt.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(strToEncrypt));
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;



            var encryptBitmapDigestInfoModel = EncryptBytesToBitmap<TEncryptDigestInfoModel>(encoding.GetBytes(strToEncrypt), secretKey, encoding);

            if (encryptBitmapDigestInfoModel.IsSuccess)
            {
                encryptBitmapDigestInfoModel.SourceString = strToEncrypt;
            }

            return encryptBitmapDigestInfoModel;

        }


        public override EncryptStringBitmapDigestInfoModel DecryptStringFromBitmap(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null)
        {
            return DecryptStringFromBitmap<EncryptStringBitmapDigestInfoModel>(encryptedBitmap, secretKey, encoding);
        }



        /// <summary>
        /// Decrypts the string from bitmap.
        /// </summary>
        /// <param name="decryptBitmap">The decrypt bitmap.</param>
        /// <param name="isDecryptStringFromBase64String">是否 从 Base64String 字符串中 解密 出 原始字符串</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentException">不是有效的加密位图数据源</exception>
        public override TEncryptDigestInfoModel DecryptStringFromBitmap<TEncryptDigestInfoModel>(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null)
        {

            var encryptBitmapDigestInfoModel = DecryptBytesFromBitmap<TEncryptDigestInfoModel>(encryptedBitmap, secretKey, encoding);

            if (encryptBitmapDigestInfoModel.IsSuccess)
            {
                encryptBitmapDigestInfoModel.SourceString = encoding.GetString(encryptBitmapDigestInfoModel.SourceBytes);
            }

            return encryptBitmapDigestInfoModel;

        }




        public override EncryptModelBitmapDigestInfoModel<T> EncryptModelToBitmap<T>(T t, string secretKey = null, Encoding encoding = null) where T : class
        {

            var json = JsonSerializeHelper.SerializeToJson(t);
            var encryptModelFileDigestInfoModel = EncryptStringToBitmap<EncryptModelBitmapDigestInfoModel<T>>(json, secretKey, encoding);

            if (encryptModelFileDigestInfoModel.IsSuccess)
            {

                encryptModelFileDigestInfoModel.SourceModel = t;

                var modelType = typeof(T);
                encryptModelFileDigestInfoModel.ModelTypeName = modelType.Name;
                encryptModelFileDigestInfoModel.ModelTypeFullName = modelType.FullName;

            }

            return encryptModelFileDigestInfoModel;


        }


        public override EncryptModelBitmapDigestInfoModel<T> DecryptModelFromBitmap<T>(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null) where T : class
        {

            var encryptModelFileDigestInfoModel = DecryptStringFromBitmap<EncryptModelBitmapDigestInfoModel<T>>(encryptedBitmap, secretKey, encoding);

            if (encryptModelFileDigestInfoModel.IsSuccess)
            {

                encryptModelFileDigestInfoModel.SourceModel = JsonSerializeHelper.DeserializeFromJson<T>(encryptModelFileDigestInfoModel.SourceString);

            }

            return encryptModelFileDigestInfoModel;

        }











        public override EncryptStringImageFileDigestInfoModel EncryptBytesToImageFile(byte[] bytesToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null)
        {
            return EncryptBytesToImageFile<EncryptStringImageFileDigestInfoModel>(bytesToEncrypt, imageFileFullPath, secretKey, encoding);
        }



        /// <summary>
        /// Encrypts the bytes to image file.
        /// </summary>
        /// <param name="bytesToEncrypt">The bytes to encrypt.</param>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override TEncryptDigestInfoModel EncryptBytesToImageFile<TEncryptDigestInfoModel>(byte[] bytesToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null)
        {
            var encryptStringImageFileDigestInfoModel = EncryptBytesToBitmap<TEncryptDigestInfoModel>(bytesToEncrypt, secretKey, encoding);
            if (encryptStringImageFileDigestInfoModel.IsSuccess)
            {
                ImageHelper.SaveBitmapToImageFile(encryptStringImageFileDigestInfoModel.EncryptedBitmap, imageFileFullPath);
                encryptStringImageFileDigestInfoModel.EncryptedFileFullPath = imageFileFullPath;
                encryptStringImageFileDigestInfoModel.EncryptedBitmap.Dispose();
                encryptStringImageFileDigestInfoModel.EncryptedBitmap = null;
            }
            return encryptStringImageFileDigestInfoModel;
        }




        public override EncryptStringImageFileDigestInfoModel DecryptBytesFromImageFile(string imageFileFullPath, string secretKey = null, Encoding encoding = null)
        {
            return DecryptBytesFromImageFile<EncryptStringImageFileDigestInfoModel>(imageFileFullPath, secretKey, encoding);
        }



        /// <summary>
        /// Decrypts the bytes from image file.
        /// </summary>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <returns>System.Byte[].</returns>
        public override TEncryptDigestInfoModel DecryptBytesFromImageFile<TEncryptDigestInfoModel>(string imageFileFullPath, string secretKey = null, Encoding encoding = null)
        {

            TEncryptDigestInfoModel encryptBitmapDigestInfoModel;

            using (var encryptedBitmap = ImageHelper.GetBitmapFromImageFile(imageFileFullPath))
            {
                encryptBitmapDigestInfoModel = DecryptBytesFromBitmap<TEncryptDigestInfoModel>(encryptedBitmap, secretKey, encoding);
                if (encryptBitmapDigestInfoModel.IsSuccess)
                {
                    encryptBitmapDigestInfoModel.EncryptedFileFullPath = imageFileFullPath;
                }

                if (!encryptBitmapDigestInfoModel.EncryptedBitmap.IfIsNullOrEmpty())
                {
                    encryptBitmapDigestInfoModel.EncryptedBitmap.Dispose();
                    encryptBitmapDigestInfoModel.EncryptedBitmap = null;
                }
            }

            return encryptBitmapDigestInfoModel;

        }




        public override EncryptStringImageFileDigestInfoModel EncryptStringToImageFile(string strToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null)
        {
            return EncryptStringToImageFile<EncryptStringImageFileDigestInfoModel>(strToEncrypt, imageFileFullPath, secretKey, encoding);
        }



        /// <summary>
        /// Encrypts the string to image file.
        /// </summary>
        /// <param name="strToEncrypt">The string to encrypt.</param>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override TEncryptDigestInfoModel EncryptStringToImageFile<TEncryptDigestInfoModel>(string strToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null)
        {

            if (strToEncrypt.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(strToEncrypt));
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;




            var result = EncryptBytesToImageFile<TEncryptDigestInfoModel>(encoding.GetBytes(strToEncrypt), imageFileFullPath, secretKey, encoding);
            if (result.IsSuccess)
            {
                result.SourceString = strToEncrypt;
            }

            return result;

        }



        public override EncryptStringImageFileDigestInfoModel DecryptStringFromImageFile(string imageFileFullPath, string secretKey = null, Encoding encoding = null)
        {
            return DecryptStringFromImageFile<EncryptStringImageFileDigestInfoModel>(imageFileFullPath, secretKey, encoding);
        }


        /// <summary>
        /// Decrypts the string from image file.
        /// </summary>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <returns>System.String.</returns>
        public override TEncryptDigestInfoModel DecryptStringFromImageFile<TEncryptDigestInfoModel>(string imageFileFullPath, string secretKey = null, Encoding encoding = null)
        {

            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

            var encryptBitmapDigestInfoModel = DecryptBytesFromImageFile<TEncryptDigestInfoModel>(imageFileFullPath, secretKey, encoding);
            if (encryptBitmapDigestInfoModel.IsSuccess)
            {
                encryptBitmapDigestInfoModel.SourceString = encoding.GetString(encryptBitmapDigestInfoModel.SourceBytes);
            }

            return encryptBitmapDigestInfoModel;

        }



        public override EncryptModelImageFileDigestInfoModel<T> EncryptModelToImageFile<T>(T t, string imageFileFullPath, string secretKey = null, Encoding encoding = null) where T : class
        {

            var json = JsonSerializeHelper.SerializeToJson(t);
            var encryptModelImageFileDigestInfoModel = EncryptStringToImageFile<EncryptModelImageFileDigestInfoModel<T>>(json, imageFileFullPath, secretKey, encoding);

            if (encryptModelImageFileDigestInfoModel.IsSuccess)
            {

                encryptModelImageFileDigestInfoModel.SourceModel = t;

                var modelType = typeof(T);
                encryptModelImageFileDigestInfoModel.ModelTypeName = modelType.Name;
                encryptModelImageFileDigestInfoModel.ModelTypeFullName = modelType.FullName;

            }

            return encryptModelImageFileDigestInfoModel;

        }


        public override EncryptModelImageFileDigestInfoModel<T> DecryptModelFromImageFile<T>(string imageFileFullPath, string secretKey = null, Encoding encoding = null) where T : class
        {

            var encryptModelBitmapDigestInfoModel = DecryptStringFromImageFile<EncryptModelImageFileDigestInfoModel<T>>(imageFileFullPath, secretKey, encoding);

            if (encryptModelBitmapDigestInfoModel.IsSuccess)
            {

                encryptModelBitmapDigestInfoModel.SourceModel = JsonSerializeHelper.DeserializeFromJson<T>(encryptModelBitmapDigestInfoModel.SourceString);

            }

            return encryptModelBitmapDigestInfoModel;

        }









    }


}
