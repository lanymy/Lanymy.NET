using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.Enums;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Helpers.SecurityModels;

namespace Lanymy.Common.Helpers
{


    /// <summary>
    /// 安全加密类 辅助方法
    /// </summary>
    public class SecurityHelper
    {


        private const string SPLIT_STRING = "[$@lanymy@$]";


        /// <summary>
        /// 默认密钥
        /// </summary>
        private const string DEFAULT_SECURITY_KEY = DefaultSettingKeys.DEFAULT_SECURITY_KEY;

        /// <summary>
        /// 加密文件头摘要信息的长度 二进制数组的 长度
        /// </summary>
        private const int ENCRYPT_HEADER_INFO_BYTES_LENGTH = sizeof(int);

        /// <summary>
        /// 加密 后 正文二进制数组 长度
        /// </summary>
        private const int ENCRYPT_AFTER_CONTENT_BYTES_LENGTH = sizeof(long);

        /// <summary>
        /// 加密种子头部标识数据长度
        /// </summary>
        private const int ENCRYPT_RANDOM_HEADER_FLAG_DATA_LENGTH = 17;

        ///// <summary>
        ///// 二进制 数组 SHA256 哈希散列算法 哈希值 长度
        ///// </summary>
        //private const int BYTES_HASH_CODE_LENGTH = 64;

        /// <summary>
        /// 二进制 数组 SHA1 哈希散列算法 哈希值 长度
        /// </summary>
        private const int BYTES_HASH_CODE_LENGTH = 40;

        /// <summary>
        /// 当前 哈希散列算法 加密类型
        /// </summary>
        private static readonly HashAlgorithmTypeEnum _CurrentHashAlgorithmType = HashAlgorithmTypeEnum.SHA1;



        private static byte[] GetSecurityKey16Bytes(string securityKey, Encoding encoding)
        {
            const int keySize = 16;
            byte[] bytes = encoding.GetBytes(securityKey);

            return bytes.Length >= keySize ? bytes.Take(keySize).ToArray() : ArrayHelper.MergerArray(bytes, new byte[keySize - bytes.Length]);
        }


        //public static TEncryptResultModel EncryptStreamToStream<TEncryptHeaderInfoModel, TEncryptResultModel>(Stream sourceStream, Stream encryptStream, string secretKey = null, bool ifRandom = true, TEncryptHeaderInfoModel encryptHeaderInfoModel = null, Encoding encoding = null)
        //    where TEncryptHeaderInfoModel : BaseEncryptHeaderInfoModel, new()
        //    where TEncryptResultModel : BaseEncryptResultModel<TEncryptHeaderInfoModel>, new()
        public static TEncryptDigestInfoModel EncryptStreamToStream<TEncryptDigestInfoModel>(Stream sourceStream, Stream encryptStream, SecurityEncryptDirectionTypeEnum securityEncryptDirectionType, string secretKey = null, bool ifRandom = true, TEncryptDigestInfoModel encryptDigestInfoModel = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptDigestInfoModel, new()
        {


            if (secretKey.IfIsNullOrEmpty()) secretKey = DEFAULT_SECURITY_KEY;
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

            //密钥必须16位
            byte[] secretKeyBytes = GetSecurityKey16Bytes(secretKey, encoding);

            long contentStartPosition = 0;

            using (var compressionStream = new GZipStream(encryptStream, CompressionMode.Compress, true))
            using (ICryptoTransform transform = new TripleDESCryptoServiceProvider().CreateEncryptor(secretKeyBytes, secretKeyBytes))
            using (CryptoStream cryptoStream = new CryptoStream(compressionStream, transform, CryptoStreamMode.Write))
            {

                //byte序列说明 byte1随机种子标识位 (----17位时间戳随机数----) utf8编码40头摘要哈希值 int4头摘要长度 byte*头摘要信息(不使用随机种子加密的时候,不能带时间日期属性,否则加密结果信息无法每次固定一致) long8正文大小 utf8编码40正文哈希值  

                if (encryptDigestInfoModel.IfIsNullOrEmpty())
                {
                    encryptDigestInfoModel = new TEncryptDigestInfoModel();
                }

                encryptDigestInfoModel.SecurityEncryptDirectionType = securityEncryptDirectionType;
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
                    encryptStream.Write(headerFlagDataStringBytes, 0, ENCRYPT_RANDOM_HEADER_FLAG_DATA_LENGTH);


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
                encryptStream.Write(encryptModelBytesLengthBytes, 0, ENCRYPT_HEADER_INFO_BYTES_LENGTH);

                //写入头摘要数据
                encryptStream.Write(encryptModelJsonBytes, 0, encryptModelJsonBytes.Length);

                //写入 正文 大小占位符
                encryptStream.Write(new byte[ENCRYPT_AFTER_CONTENT_BYTES_LENGTH], 0, ENCRYPT_AFTER_CONTENT_BYTES_LENGTH);

                //写入 正文 大小哈希值 占位符
                encryptStream.Write(new byte[BYTES_HASH_CODE_LENGTH], 0, BYTES_HASH_CODE_LENGTH);

                encryptStream.Flush();

                contentStartPosition = encryptStream.Position;

                //写入 正文 数据流
                sourceStream.CopyToAsync(cryptoStream).Wait();
                cryptoStream.FlushFinalBlock();

            }


            encryptStream.Flush();



            //加密后数据流总大小
            long encryptAfterBytesLength = encryptStream.Length;
            var encryptAfterBytesLengthBytes = BitConverter.GetBytes(encryptAfterBytesLength);

            //加密后 正文数据流 大小
            long encryptAfterContentBytesLength = encryptAfterBytesLength - contentStartPosition;
            var encryptAfterContentBytesLengthBytes = BitConverter.GetBytes(encryptAfterContentBytesLength);

            //加密后 正文数据流 哈希值
            var encryptAfterContentBytesHashCode = FileHelper.GetStreamHashCode(encryptStream, (int)contentStartPosition, _CurrentHashAlgorithmType);
            var encryptAfterContentBytesHashCodeBytes = encoding.GetBytes(encryptAfterContentBytesHashCode);

            encryptStream.Position = contentStartPosition - BYTES_HASH_CODE_LENGTH - ENCRYPT_AFTER_CONTENT_BYTES_LENGTH;

            //写入正文大小二进制数据
            encryptStream.Write(encryptAfterContentBytesLengthBytes, 0, ENCRYPT_AFTER_CONTENT_BYTES_LENGTH);

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


        //public static TEncryptResultModel GetHeaderInfoModelFromEncryptedStream<TEncryptHeaderInfoModel, TEncryptResultModel>(Stream encryptedStream, string secretKey = null, Encoding encoding = null)
        //    where TEncryptHeaderInfoModel : BaseEncryptHeaderInfoModel, new()
        //    where TEncryptResultModel : BaseEncryptResultModel<TEncryptHeaderInfoModel>, new()
        //public static EncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedStream(Stream encryptedStream, string secretKey = null, Encoding encoding = null)
        public static EncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedStream(Stream encryptedStream, Encoding encoding = null)
        {

            //if (secretKey.IfIsNullOrEmpty()) secretKey = DEFAULT_SECURITY_KEY;
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

            //var encryptResultModel = new EncryptResultModel<EncryptDigestInfoModel>
            //{
            //    IsSuccess = false,
            //};

            var encryptResultModel = new EncryptDigestInfoModel
            {
                IsSuccess = false,
            };

            ////密钥必须16位
            //byte[] secretKeyBytes = GetSecurityKey16Bytes(secretKey, encoding);


            encryptedStream.Position = 0;

            //提取随机种子标识位
            var ifRandomBytes = new byte[1];
            encryptedStream.Read(ifRandomBytes, 0, 1);

            bool ifRandom = ifRandomBytes[0] != 0;

            if (ifRandom)
            {

                //encryptedStream.Position += ENCRYPT_RANDOM_HEADER_FLAG_DATA_LENGTH;

                //提取随机种子标识位
                var headerFlagDataStringBytes = new byte[ENCRYPT_RANDOM_HEADER_FLAG_DATA_LENGTH];
                encryptedStream.Read(headerFlagDataStringBytes, 0, ENCRYPT_RANDOM_HEADER_FLAG_DATA_LENGTH);
                var headerFlagDataString = Encoding.UTF8.GetString(headerFlagDataStringBytes);

            }

            //提取头摘要哈希值
            var encryptModelJsonBytesHashCodeBytes = new byte[BYTES_HASH_CODE_LENGTH];
            encryptedStream.Read(encryptModelJsonBytesHashCodeBytes, 0, BYTES_HASH_CODE_LENGTH);
            var encryptModelJsonBytesHashCode = encoding.GetString(encryptModelJsonBytesHashCodeBytes);

            //提取头摘要长度
            var encryptModelBytesLengthBytes = new byte[ENCRYPT_HEADER_INFO_BYTES_LENGTH];
            encryptedStream.Read(encryptModelBytesLengthBytes, 0, ENCRYPT_HEADER_INFO_BYTES_LENGTH);
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

            var encryptModelJson = string.Empty;
            EncryptDigestInfoModel encryptHeaderModel;

            try
            {

                encryptModelJson = CompressionHelper.DecompressStringFromBytes(encryptModelJsonBytes);
                encryptHeaderModel = JsonSerializeHelper.DeserializeFromJson<EncryptDigestInfoModel>(encryptModelJson);
                encryptHeaderModel.DencryptHeaderInfoModelJsonString = encryptModelJson;

            }
            catch (Exception e)
            {
                encryptResultModel.ErrorMessage = "指纹信息解析失败,无法继续解析";
                return encryptResultModel;
            }


            //提取正文大小
            var encryptAfterContentBytesLengthBytes = new byte[ENCRYPT_AFTER_CONTENT_BYTES_LENGTH];
            encryptedStream.Read(encryptAfterContentBytesLengthBytes, 0, ENCRYPT_AFTER_CONTENT_BYTES_LENGTH);
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

            encryptHeaderModel.EncryptBytesSize = encryptedStream.Length;
            encryptHeaderModel.EncryptBytesHashCode = FileHelper.GetStreamHashCode(encryptedStream, hashAlgorithmType: _CurrentHashAlgorithmType);
            encryptHeaderModel.EncryptContentBytesSize = encryptAfterContentBytesLength;
            encryptHeaderModel.EncryptContentBytesHashCode = encryptAfterContentBytesHashCode;

            dynamic json = Newtonsoft.Json.Linq.JToken.Parse(encryptHeaderModel.DencryptHeaderInfoModelJsonString);
            json.EncryptBytesSize = encryptHeaderModel.EncryptBytesSize;
            json.EncryptBytesHashCode = encryptHeaderModel.EncryptBytesHashCode;
            json.EncryptContentBytesSize = encryptHeaderModel.EncryptContentBytesSize;
            json.EncryptContentBytesHashCode = encryptHeaderModel.EncryptContentBytesHashCode;
            var jsonNew = json.ToString();
            encryptHeaderModel.DencryptHeaderInfoModelJsonString = jsonNew;

            encryptedStream.Position = currentPosition;

            //encryptResultModel.IsSuccess = true;
            //encryptResultModel.HeaderInfoModel = encryptHeaderModel;

            //return encryptResultModel;

            encryptHeaderModel.IsSuccess = true;

            return encryptHeaderModel;

        }

        public static EncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedFile(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null)
        {
            EncryptDigestInfoModel encryptDigestInfoModel;

            using (var encryptedStream = File.OpenRead(encryptedFileFullPath))
            {
                //encryptDigestInfoModel = GetEncryptDigestInfoModelFromEncryptedStream(encryptedStream, secretKey, encoding);
                encryptDigestInfoModel = GetEncryptDigestInfoModelFromEncryptedStream(encryptedStream, encoding);

            }

            return encryptDigestInfoModel;
        }


        public static EncryptDigestInfoModel DencryptStreamFromStream(Stream encryptedStream, Stream sourceStream, string secretKey = null, Encoding encoding = null)
        {

            if (secretKey.IfIsNullOrEmpty()) secretKey = DEFAULT_SECURITY_KEY;
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

            //var encryptResultModel = GetEncryptDigestInfoModelFromEncryptedStream(encryptedStream, secretKey, encoding);
            var encryptResultModel = GetEncryptDigestInfoModelFromEncryptedStream(encryptedStream, encoding);

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

                cryptoStream.CopyToAsync(sourceStream).Wait();

            }

            ////效验头摘要信息和解密后数据流大小和哈希值
            ////var sourceStreamLength = sourceStream.Length;
            //var sourceStreamHashCode = FileHelper.GetStreamHashCode(sourceStream);

            //if (encryptResultModel.HeaderInfoModel.SourceBytesHashCode != sourceStreamHashCode)
            //{

            //}

            return encryptResultModel;

        }


        /// <summary>
        /// 二进制数组加密 返回加密后的二进制数组
        /// </summary>
        /// <param name="bytesToEncrypt">要加密的二进制数组</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static EncryptBytesDigestInfoModel EncryptBytesToBytes(byte[] bytesToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null, SecurityEncryptDirectionTypeEnum securityEncryptDirectionType = SecurityEncryptDirectionTypeEnum.BytesToBytes, EncryptDigestInfoModel encryptDigestInfoModel = null)
        {

            if (bytesToEncrypt.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(bytesToEncrypt));

            EncryptBytesDigestInfoModel encryptBytesDigestInfoModel;

            using (var sourceStream = new MemoryStream(bytesToEncrypt))
            using (var encryptStream = new MemoryStream())
            {
                encryptDigestInfoModel = EncryptStreamToStream(sourceStream, encryptStream, securityEncryptDirectionType, secretKey, ifRandom, encryptDigestInfoModel, encoding);
                encryptBytesDigestInfoModel = encryptDigestInfoModel.AsTypeByDeepClone<EncryptDigestInfoModel, EncryptBytesDigestInfoModel>();
                if (encryptBytesDigestInfoModel.IsSuccess)
                {
                    encryptBytesDigestInfoModel.EncryptedBytes = encryptStream.ToArray();
                }
            }


            return encryptBytesDigestInfoModel;

        }



        /// <summary>
        /// 解密二进制数组 返回 解密后的二进制数组
        /// </summary>
        /// <param name="bytesToDecrypt">要解密的二进制数组</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="encoding">解密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static EncryptBytesDigestInfoModel DecryptBytesFromBytes(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null)
        {

            if (bytesToDecrypt.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(bytesToDecrypt));

            EncryptBytesDigestInfoModel encryptBytesResultModel;

            using (var sourceStream = new MemoryStream())
            using (var encryptedStream = new MemoryStream(bytesToDecrypt))
            {

                var encryptDigestInfoModel = DencryptStreamFromStream(encryptedStream, sourceStream, secretKey, encoding);

                if (encryptDigestInfoModel.IsSuccess)
                {
                    var dencryptHeaderInfoModelJsonString = encryptDigestInfoModel.DencryptHeaderInfoModelJsonString;
                    encryptBytesResultModel = JsonSerializeHelper.DeserializeFromJson<EncryptBytesDigestInfoModel>(dencryptHeaderInfoModelJsonString);
                    encryptBytesResultModel.DencryptHeaderInfoModelJsonString = dencryptHeaderInfoModelJsonString;
                    encryptBytesResultModel.SourceBytes = sourceStream.ToArray();
                }
                else
                {
                    encryptBytesResultModel = new EncryptBytesDigestInfoModel();
                }

                encryptBytesResultModel.IsSuccess = encryptDigestInfoModel.IsSuccess;
                encryptBytesResultModel.ErrorMessage = encryptDigestInfoModel.ErrorMessage;

            }


            return encryptBytesResultModel;

        }




        /// <summary>
        /// 字符串加密 返回 加密后的二进制数组
        /// </summary>
        /// <param name="strToEncrypt">要加密的字符串</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static EncryptStringDigestInfoModel EncryptStringToBytes(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null, SecurityEncryptDirectionTypeEnum securityEncryptDirectionType = SecurityEncryptDirectionTypeEnum.StringToBytes, EncryptDigestInfoModel encryptDigestInfoModel = null)
        {

            if (strToEncrypt.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(strToEncrypt));
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

            EncryptStringDigestInfoModel encryptStringDigestInfoModel;
            var bytesToEncrypt = encoding.GetBytes(strToEncrypt);

            var encryptBytesDigestInfoModel = EncryptBytesToBytes(bytesToEncrypt, secretKey, ifRandom, encoding, securityEncryptDirectionType, encryptDigestInfoModel);

            if (encryptBytesDigestInfoModel.IsSuccess)
            {
                var encryptedBytes = encryptBytesDigestInfoModel.EncryptedBytes;
                encryptBytesDigestInfoModel.EncryptedBytes = null;
                encryptStringDigestInfoModel = encryptBytesDigestInfoModel.AsTypeByDeepClone<EncryptDigestInfoModel, EncryptStringDigestInfoModel>();
                encryptStringDigestInfoModel.EncryptedBytes = encryptedBytes;
            }
            else
            {
                encryptStringDigestInfoModel = new EncryptStringDigestInfoModel();
            }

            encryptStringDigestInfoModel.IsSuccess = encryptBytesDigestInfoModel.IsSuccess;
            encryptStringDigestInfoModel.ErrorMessage = encryptBytesDigestInfoModel.ErrorMessage;

            return encryptStringDigestInfoModel;

        }

        /// <summary>
        /// 解密二进制数组 返回 解密后的字符串
        /// </summary>
        /// <param name="bytesToDecrypt">要解密的二进制数组</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="encoding">解密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static EncryptStringDigestInfoModel DecryptStringFromBytes(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null)
        {

            if (bytesToDecrypt.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(bytesToDecrypt));
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

            EncryptStringDigestInfoModel encryptStringDigestInfoModel;

            var encryptBytesDigestInfoModel = DecryptBytesFromBytes(bytesToDecrypt, secretKey, encoding);

            if (encryptBytesDigestInfoModel.IsSuccess)
            {
                var dencryptHeaderInfoModelJsonString = encryptBytesDigestInfoModel.DencryptHeaderInfoModelJsonString;
                encryptStringDigestInfoModel = JsonSerializeHelper.DeserializeFromJson<EncryptStringDigestInfoModel>(dencryptHeaderInfoModelJsonString);
                encryptStringDigestInfoModel.DencryptHeaderInfoModelJsonString = dencryptHeaderInfoModelJsonString;
                encryptStringDigestInfoModel.SourceString = encoding.GetString(encryptBytesDigestInfoModel.SourceBytes);
            }
            else
            {
                encryptStringDigestInfoModel = new EncryptStringDigestInfoModel();
            }

            encryptStringDigestInfoModel.IsSuccess = encryptBytesDigestInfoModel.IsSuccess;
            encryptStringDigestInfoModel.ErrorMessage = encryptBytesDigestInfoModel.ErrorMessage;


            return encryptStringDigestInfoModel;

        }

        /// <summary>
        /// 字符串加密 返回 加密后的 Base64 字符串
        /// </summary>
        /// <param name="strToEncrypt">要加密的字符串</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static EncryptStringDigestInfoModel EncryptStringToBase64String(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null, SecurityEncryptDirectionTypeEnum securityEncryptDirectionType = SecurityEncryptDirectionTypeEnum.StringToBase64String, EncryptDigestInfoModel encryptDigestInfoModel = null)
        {


            var encryptStringDigestInfoModel = EncryptStringToBytes(strToEncrypt, secretKey, ifRandom, encoding, securityEncryptDirectionType, encryptDigestInfoModel);

            if (encryptStringDigestInfoModel.IsSuccess)
            {
                encryptStringDigestInfoModel.EncryptedBase64String = Convert.ToBase64String(encryptStringDigestInfoModel.EncryptedBytes);
                encryptStringDigestInfoModel.EncryptedBytes = null;
            }

            return encryptStringDigestInfoModel;


        }


        /// <summary>
        /// 解密字符串 返回 解密后的字符串
        /// </summary>
        /// <param name="base64StringToDecrypt">要解密的Base64字符串</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="encoding">解密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        public static EncryptStringDigestInfoModel DecryptStringFromBase64String(string base64StringToDecrypt, string secretKey = null, Encoding encoding = null)
        {

            if (base64StringToDecrypt.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(base64StringToDecrypt));
            return DecryptStringFromBytes(Convert.FromBase64String(base64StringToDecrypt), secretKey, encoding);

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
        public static EncryptModelDigestInfoModel<T> EncryptModelToBytes<T>(T t, string secretKey = null, bool ifRandom = true, Encoding encoding = null, SecurityEncryptDirectionTypeEnum securityEncryptDirectionType = SecurityEncryptDirectionTypeEnum.ModelToBytes, EncryptModelDigestInfoModel<T> encryptModelDigestInfoModel = null) where T : class
        {

            //if (encoding.IfIsNullOrEmpty()) encoding = GlobalSettings.DEFAULT_ENCODING;

            if (encryptModelDigestInfoModel.IfIsNullOrEmpty())
            {
                encryptModelDigestInfoModel = new EncryptModelDigestInfoModel<T>();
            }

            var modelType = typeof(T);
            encryptModelDigestInfoModel.ModelTypeName = modelType.Name;
            encryptModelDigestInfoModel.ModelTypeFullName = modelType.FullName;

            var jsonString = JsonSerializeHelper.SerializeToJson(t);

            var encryptStringDigestInfoModel = EncryptStringToBytes(jsonString, secretKey, ifRandom, encoding, securityEncryptDirectionType, encryptModelDigestInfoModel);

            if (encryptStringDigestInfoModel.IsSuccess)
            {
                encryptModelDigestInfoModel.EncryptedBytes = encryptStringDigestInfoModel.EncryptedBytes;
            }

            encryptModelDigestInfoModel.IsSuccess = encryptStringDigestInfoModel.IsSuccess;
            encryptModelDigestInfoModel.ErrorMessage = encryptStringDigestInfoModel.ErrorMessage;


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
        public static EncryptModelDigestInfoModel<T> DecryptModelFromBytes<T>(byte[] ecryptedBytes, string secretKey = null, Encoding encoding = null) where T : class
        {

            //if (encoding.IfIsNullOrEmpty()) encoding = GlobalSettings.DEFAULT_ENCODING;

            EncryptModelDigestInfoModel<T> encryptModelDigestInfoModel;

            var encryptStringDigestInfoModel = DecryptStringFromBytes(ecryptedBytes, secretKey, encoding);

            if (encryptStringDigestInfoModel.IsSuccess)
            {
                var dencryptHeaderInfoModelJsonString = encryptStringDigestInfoModel.DencryptHeaderInfoModelJsonString;
                encryptModelDigestInfoModel = JsonSerializeHelper.DeserializeFromJson<EncryptModelDigestInfoModel<T>>(dencryptHeaderInfoModelJsonString);
                encryptModelDigestInfoModel.DencryptHeaderInfoModelJsonString = dencryptHeaderInfoModelJsonString;
                var json = encryptStringDigestInfoModel.SourceString;
                encryptModelDigestInfoModel.SourceModel = JsonSerializeHelper.DeserializeFromJson<T>(json);
            }
            else
            {
                encryptModelDigestInfoModel = new EncryptModelDigestInfoModel<T>();
            }

            encryptModelDigestInfoModel.IsSuccess = encryptStringDigestInfoModel.IsSuccess;
            encryptModelDigestInfoModel.ErrorMessage = encryptStringDigestInfoModel.ErrorMessage;



            return encryptModelDigestInfoModel;

        }


        /// <summary>
        /// 加密并序列化Model成Base64字符串
        /// </summary>
        /// <typeparam name="T">要序列化的实体类型</typeparam>
        /// <param name="t">要序列化的实体 实例</param>
        /// <param name="secretKey">密钥 Null 使用默认密钥</param>
        /// <returns></returns>
        public static EncryptModelDigestInfoModel<T> EncryptModelToBase64String<T>(T t, string secretKey = null, bool ifRandom = true, Encoding encoding = null, SecurityEncryptDirectionTypeEnum securityEncryptDirectionType = SecurityEncryptDirectionTypeEnum.ModelToBase64String, EncryptModelDigestInfoModel<T> encryptModelDigestInfoModel = null) where T : class
        {

            //EncryptModelDigestInfoModel<T> resultModel;

            var encryptModelToBytes = EncryptModelToBytes(t, secretKey, ifRandom, encoding, securityEncryptDirectionType, encryptModelDigestInfoModel);

            if (encryptModelToBytes.IsSuccess)
            {
                var encryptedBytes = encryptModelToBytes.EncryptedBytes;
                encryptModelToBytes.EncryptedBytes = null;
                encryptModelToBytes.EncryptedBase64String = Convert.ToBase64String(encryptedBytes);
            }


            return encryptModelToBytes;

        }


        /// <summary>
        /// 解密并反序列化Base64字符串成实体实例
        /// </summary>
        /// <typeparam name="T">要反序列化成的实体类型</typeparam>
        /// <param name="encryptBase64String">要解密的Base64字符串</param>
        /// <param name="secretKey">密钥 Null 使用默认密钥</param>
        /// <returns></returns>
        public static EncryptModelDigestInfoModel<T> DecryptModelFromBase64String<T>(string encryptBase64String, string secretKey = null, Encoding encoding = null) where T : class
        {
            if (encryptBase64String.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(encryptBase64String));
            return DecryptModelFromBytes<T>(Convert.FromBase64String(encryptBase64String), secretKey, encoding);
        }



        public static EncryptBytesDigestInfoModel EncryptBytesToFile(byte[] sourceBytes, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null, SecurityEncryptDirectionTypeEnum securityEncryptDirectionType = SecurityEncryptDirectionTypeEnum.BytesToFile, EncryptDigestInfoModel encryptDigestInfoModel = null)
        {


            if (sourceBytes.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(sourceBytes));

            EncryptBytesDigestInfoModel encryptBytesDigestInfoModel;
            using (var sourceStream = new MemoryStream(sourceBytes))
            using (var encryptStream = File.Create(encryptFileFullPath, BufferSizeKeys.BUFFER_SIZE_4K))
            {

                encryptDigestInfoModel = EncryptStreamToStream(sourceStream, encryptStream, securityEncryptDirectionType, secretKey, ifRandom, encryptDigestInfoModel, encoding);
                encryptBytesDigestInfoModel = encryptDigestInfoModel.AsTypeByDeepClone<EncryptDigestInfoModel, EncryptBytesDigestInfoModel>();
            }

            return encryptBytesDigestInfoModel;

        }


        public static EncryptBytesDigestInfoModel DecryptBytesFromFile(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null)
        {

            EncryptBytesDigestInfoModel encryptBytesDigestInfoModel;

            using (var sourceStream = new MemoryStream())
            using (var encryptedStream = File.OpenRead(encryptedFileFullPath))
            {
                var encryptDigestInfoModel = DencryptStreamFromStream(encryptedStream, sourceStream, secretKey, encoding);

                if (encryptDigestInfoModel.IsSuccess)
                {
                    var dencryptHeaderInfoModelJsonString = encryptDigestInfoModel.DencryptHeaderInfoModelJsonString;
                    encryptBytesDigestInfoModel = JsonSerializeHelper.DeserializeFromJson<EncryptBytesDigestInfoModel>(dencryptHeaderInfoModelJsonString);
                    encryptBytesDigestInfoModel.DencryptHeaderInfoModelJsonString = dencryptHeaderInfoModelJsonString;
                    encryptBytesDigestInfoModel.SourceBytes = sourceStream.ToArray();
                }
                else
                {
                    encryptBytesDigestInfoModel = new EncryptBytesDigestInfoModel();
                }

                encryptBytesDigestInfoModel.IsSuccess = encryptDigestInfoModel.IsSuccess;
                encryptBytesDigestInfoModel.ErrorMessage = encryptDigestInfoModel.ErrorMessage;

            }

            return encryptBytesDigestInfoModel;

        }




        public static EncryptStringDigestInfoModel EncryptStringToFile(string sourceString, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null, SecurityEncryptDirectionTypeEnum securityEncryptDirectionType = SecurityEncryptDirectionTypeEnum.StringToFile, EncryptDigestInfoModel encryptDigestInfoModel = null)
        {

            if (sourceString.IfIsNullOrEmpty()) throw new ArgumentNullException(nameof(sourceString));
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

            EncryptStringDigestInfoModel encryptStringDigestInfoModel;

            var encryptBytesDigestInfoModel = EncryptBytesToFile(encoding.GetBytes(sourceString), encryptFileFullPath, secretKey, ifRandom, encoding, securityEncryptDirectionType, encryptDigestInfoModel);
            if (encryptBytesDigestInfoModel.IsSuccess)
            {
                encryptStringDigestInfoModel = encryptBytesDigestInfoModel.AsTypeByDeepClone<EncryptDigestInfoModel, EncryptStringDigestInfoModel>();
            }
            else
            {
                encryptStringDigestInfoModel = new EncryptStringDigestInfoModel();
            }

            encryptStringDigestInfoModel.IsSuccess = encryptBytesDigestInfoModel.IsSuccess;
            encryptStringDigestInfoModel.ErrorMessage = encryptBytesDigestInfoModel.ErrorMessage;

            return encryptStringDigestInfoModel;

        }

        public static EncryptStringDigestInfoModel DecryptStringFromFile(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null)
        {

            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;

            EncryptStringDigestInfoModel encryptStringDigestInfoModel;

            var encryptBytesDigestInfoModel = DecryptBytesFromFile(encryptedFileFullPath, secretKey, encoding);

            if (encryptBytesDigestInfoModel.IsSuccess)
            {
                var dencryptHeaderInfoModelJsonString = encryptBytesDigestInfoModel.DencryptHeaderInfoModelJsonString;
                encryptStringDigestInfoModel = JsonSerializeHelper.DeserializeFromJson<EncryptStringDigestInfoModel>(dencryptHeaderInfoModelJsonString);
                encryptStringDigestInfoModel.DencryptHeaderInfoModelJsonString = dencryptHeaderInfoModelJsonString;
                encryptStringDigestInfoModel.SourceString = encoding.GetString(encryptBytesDigestInfoModel.EncryptedBytes);
            }
            else
            {
                encryptStringDigestInfoModel = new EncryptStringDigestInfoModel();
            }

            encryptStringDigestInfoModel.IsSuccess = encryptBytesDigestInfoModel.IsSuccess;
            encryptStringDigestInfoModel.ErrorMessage = encryptBytesDigestInfoModel.ErrorMessage;


            return encryptStringDigestInfoModel;
        }




        public static EncryptModelDigestInfoModel<T> EncryptModelToFile<T>(T t, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null, SecurityEncryptDirectionTypeEnum securityEncryptDirectionType = SecurityEncryptDirectionTypeEnum.ModelToFile, EncryptModelDigestInfoModel<T> encryptModelDigestInfoModel = null) where T : class
        {

            EncryptModelToBytes(t, secretKey, ifRandom, encoding, securityEncryptDirectionType, encryptModelDigestInfoModel);

            if (encryptModelDigestInfoModel.IsSuccess)
            {
                File.WriteAllBytes(encryptFileFullPath, encryptModelDigestInfoModel.EncryptedBytes);
                encryptModelDigestInfoModel.EncryptedBytes = null;
            }

            return encryptModelDigestInfoModel;

        }


        public static EncryptModelDigestInfoModel<T> DecryptModelFromFile<T>(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null) where T : class
        {

            var ecryptedBytes = File.ReadAllBytes(encryptedFileFullPath);

            return DecryptModelFromBytes<T>(ecryptedBytes, secretKey, encoding);

        }






        public static EncryptFileDigestInfoModel EncryptFileToFile(string sourceFileFullPath, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null, SecurityEncryptDirectionTypeEnum securityEncryptDirectionType = SecurityEncryptDirectionTypeEnum.FileToFile, EncryptFileDigestInfoModel encryptFileDigestInfoModel = null)
        {

            if (encryptFileDigestInfoModel.IfIsNullOrEmpty())
            {
                encryptFileDigestInfoModel = new EncryptFileDigestInfoModel();
            }

            encryptFileDigestInfoModel.SourceFileFullName = Path.GetFileName(sourceFileFullPath);
            encryptFileDigestInfoModel.EncryptedFileFullName = Path.GetFileName(encryptFileFullPath);

            using (var sourceStream = File.OpenRead(sourceFileFullPath))
            using (var encryptStream = File.Create(encryptFileFullPath, BufferSizeKeys.BUFFER_SIZE_4K))
            {

                encryptFileDigestInfoModel = EncryptStreamToStream(sourceStream, encryptStream, securityEncryptDirectionType, secretKey, ifRandom, encryptFileDigestInfoModel, encoding);

            }

            return encryptFileDigestInfoModel;

        }


        public static EncryptFileDigestInfoModel DecryptFileFromFile(string encryptedFileFullPath, string sourceFileFullPath, bool ifOverWriteFile = false, string secretKey = null, Encoding encoding = null)
        {

            EncryptFileDigestInfoModel encryptFileDigestInfoModel;

            var encryptDigestInfoModel = GetEncryptDigestInfoModelFromEncryptedFile(encryptedFileFullPath, secretKey, encoding);

            if (encryptDigestInfoModel.IsSuccess)
            {

                var dencryptHeaderInfoModelJsonString = encryptDigestInfoModel.DencryptHeaderInfoModelJsonString;
                encryptFileDigestInfoModel = JsonSerializeHelper.DeserializeFromJson<EncryptFileDigestInfoModel>(dencryptHeaderInfoModelJsonString);
                encryptFileDigestInfoModel.DencryptHeaderInfoModelJsonString = dencryptHeaderInfoModelJsonString;

                if (!ifOverWriteFile && File.Exists(sourceFileFullPath))
                {

                    var sourceFileRootDirectoryFullPath = Path.GetDirectoryName(sourceFileFullPath);
                    var sourceFileFullName = Path.GetFileName(sourceFileFullPath);
                    sourceFileFullName = string.Format("{0}[{1:HHmmss}]{2}", Path.GetFileNameWithoutExtension(sourceFileFullName), DateTime.Now, Path.GetExtension(sourceFileFullName));
                    sourceFileFullPath = Path.Combine(sourceFileRootDirectoryFullPath, sourceFileFullName);

                }

                encryptFileDigestInfoModel.SourceFileFullName = Path.GetFileName(sourceFileFullPath);
                encryptFileDigestInfoModel.EncryptedFileFullName = Path.GetFileName(encryptedFileFullPath);

                using (var encryptedStream = File.OpenRead(encryptedFileFullPath))
                using (var sourceStream = File.Create(sourceFileFullPath, BufferSizeKeys.BUFFER_SIZE_4K))
                {
                    DencryptStreamFromStream(encryptedStream, sourceStream, secretKey, encoding);
                }

            }
            else
            {
                encryptFileDigestInfoModel = new EncryptFileDigestInfoModel();
            }

            encryptFileDigestInfoModel.IsSuccess = encryptDigestInfoModel.IsSuccess;
            encryptFileDigestInfoModel.ErrorMessage = encryptDigestInfoModel.ErrorMessage;

            return encryptFileDigestInfoModel;

        }



























        /// <summary>
        /// 通用MD5加密
        /// </summary>
        /// <param name="strToEncrypt"></param>
        /// <returns></returns>
        public static string EncryptToMD5(string strToEncrypt)
        {

            if (strToEncrypt.IfIsNullOrEmpty()) throw new ArgumentNullException("strToEncrypt");
            var md5 = new MD5CryptoServiceProvider();
            //byte[] bytes = Encoding.UTF8.GetBytes(strToEncrypt.ToLower());
            byte[] bytes = Encoding.UTF8.GetBytes(strToEncrypt);
            byte[] hashedBytes = md5.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            foreach (var b in hashedBytes)
            {
                //sb.Append(b.ToString("x2").ToLower());
                sb.Append(b.ToString("X2"));
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

            using (X509Store storeRoot = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
            {
                storeRoot.Open(OpenFlags.ReadWrite);
                if (!storeRoot.Certificates.Contains(certificate))
                {
                    storeRoot.Add(certificate);
                }
            }

            using (X509Store storeMy = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                storeMy.Open(OpenFlags.ReadWrite);
                if (!storeMy.Certificates.Contains(certificate))
                {
                    storeMy.Add(certificate);
                }
            }

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

            CreateRsaKeyBlobBytes(keySizeType, out var publicKeyBlobBytes, out var privateKeyBlobBytes);
            publicKeyBlobBase64String = Convert.ToBase64String(publicKeyBlobBytes);
            privateKeyBase64String = Convert.ToBase64String(privateKeyBlobBytes);

        }

        /// <summary>
        /// 创建 RSA 的 Blob 原始 二进制 数据 密钥
        /// </summary>
        /// <param name="keySizeType">密钥长度(加密级别) 1024/2048/3072/7680/15360</param>
        /// <param name="publicKeyBlobBytes">公钥 Blob 原始 二进制 数据</param>
        /// <param name="privateKeyBlobBytes">私钥 Blob 原始 二进制 数据</param>
        public static void CreateRsaKeyBlobBytes(RsaKeySizeTypeEnum keySizeType, out byte[] publicKeyBlobBytes, out byte[] privateKeyBlobBytes)
        {
            var rsa = new RSACryptoServiceProvider((int)keySizeType);
            publicKeyBlobBytes = rsa.ExportCspBlob(false);
            privateKeyBlobBytes = rsa.ExportCspBlob(true);
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

            byte[] bytes = null;

            try
            {
                var rsa = new RSACryptoServiceProvider((int)keySizeType);
                rsa.ImportCspBlob(publicKeyBlobBytes);
                bytes = rsa.Encrypt(bytesToEncrypt, false);
            }
            catch
            {

            }

            return bytes;

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

            byte[] bytes = null;

            try
            {

                var rsa = new RSACryptoServiceProvider((int)keySizeType);
                rsa.ImportCspBlob(privateKeyBlobBytes);
                bytes = rsa.Decrypt(bytesToDecrypt, false);

            }
            catch
            {

            }

            return bytes;

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

            string encryptBase64String = string.Empty;

            try
            {
                encryptBase64String = Convert.ToBase64String(RsaEncryptBytesToBytes(Convert.FromBase64String(publicKeyBlobBase64String), keySizeType, DefaultSettingKeys.DEFAULT_ENCODING.GetBytes(strToEncrypt)));
            }
            catch
            {

            }

            return encryptBase64String;

        }

        /// <summary>
        /// RSA 解密 Base64 字符串 返回 解密后的 原始 字符串  
        /// </summary>
        /// <param name="privateKeyBlobBase64String">私钥 Blob 二进制数组 的 Base64 字符串</param>
        /// <param name="keySize">密钥长度(加密级别) 1024/2048/3072/7680/15360</param>
        /// <param name="base64StringToDecrypt">要 解密 的 Base64 字符串</param>
        /// <returns></returns>
        public static string RsaDecryptStringFromBase64String(string privateKeyBlobBase64String, RsaKeySizeTypeEnum keySizeType, string base64StringToDecrypt)
        {

            string decryptString = string.Empty;

            try
            {

                decryptString = DefaultSettingKeys.DEFAULT_ENCODING.GetString(RsaDecryptBytesFromBytes(Convert.FromBase64String(privateKeyBlobBase64String), keySizeType, Convert.FromBase64String(base64StringToDecrypt)));

            }
            catch
            {

            }

            return decryptString;

        }



        private static void EncryptBitmapBytes(Random random, byte[] bitmapBytes)
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

        private static Bitmap GetEncryptedBitmap(string strToBitmap)
        {

            const string endStr = SPLIT_STRING;


            var message = new StringBuilder(strToBitmap + endStr);

            int messageLength = message.Length;
            int endStrLength = endStr.Length;

            //补齐二进制序列成正方形阵列
            int sqrt = (int)Math.Sqrt(messageLength) + 1;
            int sqrtLength = sqrt * sqrt;

            var overlength = sqrtLength - messageLength;

            for (int i = 0; i < overlength / endStrLength; i++)
            {
                message.Append(endStr);
            }

            //for (int i = 0; i < overlength % endStrLength; i++)
            //{
            //    message.Append(endStr[i]);
            //}

            message.Append(endStr.Substring(0, overlength % endStrLength));

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

        private static string GetStringFromEncryptedBitmap(Bitmap encryptedBitmap)
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

            return sb.ToString().LeftSubString(SPLIT_STRING);

        }


        public static EncryptBitmapDigestInfoModel EncryptBytesToBitmap(byte[] bytesToEncrypt, string secretKey = null, Encoding encoding = null, SecurityEncryptDirectionTypeEnum securityEncryptDirectionType = SecurityEncryptDirectionTypeEnum.BytesToBitmap, EncryptDigestInfoModel encryptDigestInfoModel = null)
        {

            //if (encoding.IfIsNullOrEmpty()) encoding = GlobalSettings.DEFAULT_ENCODING;

            EncryptBitmapDigestInfoModel encryptBitmapDigestInfoModel;

            var encryptBytesDigestInfoModel = EncryptBytesToBytes(bytesToEncrypt, secretKey, true, encoding, securityEncryptDirectionType, encryptDigestInfoModel);

            if (encryptBytesDigestInfoModel.IsSuccess)
            {

                encryptBitmapDigestInfoModel = encryptBytesDigestInfoModel.AsTypeByDeepClone<EncryptDigestInfoModel, EncryptBitmapDigestInfoModel>();
                var encryptedBytes = encryptBytesDigestInfoModel.EncryptedBytes;
                encryptBitmapDigestInfoModel.EncryptedBitmap = GetEncryptedBitmap(Convert.ToBase64String(encryptedBytes));

            }
            else
            {
                encryptBitmapDigestInfoModel = new EncryptBitmapDigestInfoModel();
            }

            encryptBitmapDigestInfoModel.IsSuccess = encryptBytesDigestInfoModel.IsSuccess;
            encryptBitmapDigestInfoModel.ErrorMessage = encryptBytesDigestInfoModel.ErrorMessage;

            return encryptBitmapDigestInfoModel;

        }


        public static EncryptBitmapDigestInfoModel DecryptBytesFromBitmap(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null)
        {

            EncryptBitmapDigestInfoModel encryptBitmapDigestInfoModel;

            var bitmapBase64String = GetStringFromEncryptedBitmap(encryptedBitmap);
            var encryptedBytes = Convert.FromBase64String(bitmapBase64String);
            var encryptBytesDigestInfoModel = DecryptBytesFromBytes(encryptedBytes, secretKey, encoding);

            if (encryptBytesDigestInfoModel.IsSuccess)
            {
                var dencryptHeaderInfoModelJsonString = encryptBytesDigestInfoModel.DencryptHeaderInfoModelJsonString;
                encryptBitmapDigestInfoModel = JsonSerializeHelper.DeserializeFromJson<EncryptBitmapDigestInfoModel>(dencryptHeaderInfoModelJsonString);
                encryptBitmapDigestInfoModel.DencryptHeaderInfoModelJsonString = dencryptHeaderInfoModelJsonString;
                encryptBitmapDigestInfoModel.SourceBytes = encryptBytesDigestInfoModel.SourceBytes;
            }
            else
            {
                encryptBitmapDigestInfoModel = new EncryptBitmapDigestInfoModel();
            }

            encryptBitmapDigestInfoModel.IsSuccess = encryptBytesDigestInfoModel.IsSuccess;
            encryptBitmapDigestInfoModel.ErrorMessage = encryptBytesDigestInfoModel.ErrorMessage;

            return encryptBitmapDigestInfoModel;

        }


        public static EncryptBitmapDigestInfoModel EncryptStringToBitmap(string strToEncrypt, string secretKey = null, Encoding encoding = null, SecurityEncryptDirectionTypeEnum securityEncryptDirectionType = SecurityEncryptDirectionTypeEnum.StringToBitmap, EncryptDigestInfoModel encryptDigestInfoModel = null)
        {

            EncryptBitmapDigestInfoModel encryptBitmapDigestInfoModel;

            var encryptStringDigestInfoModel = EncryptStringToBytes(strToEncrypt, secretKey, true, encoding, securityEncryptDirectionType, encryptDigestInfoModel);

            if (encryptStringDigestInfoModel.IsSuccess)
            {

                encryptBitmapDigestInfoModel = encryptStringDigestInfoModel.AsTypeByDeepClone<EncryptDigestInfoModel, EncryptBitmapDigestInfoModel>();
                var encryptedBytes = encryptStringDigestInfoModel.EncryptedBytes;
                encryptBitmapDigestInfoModel.EncryptedBitmap = GetEncryptedBitmap(Convert.ToBase64String(encryptedBytes));

            }
            else
            {
                encryptBitmapDigestInfoModel = new EncryptBitmapDigestInfoModel();
            }

            encryptBitmapDigestInfoModel.IsSuccess = encryptStringDigestInfoModel.IsSuccess;
            encryptBitmapDigestInfoModel.ErrorMessage = encryptStringDigestInfoModel.ErrorMessage;

            return encryptBitmapDigestInfoModel;

        }


        /// <summary>
        /// Decrypts the string from bitmap.
        /// </summary>
        /// <param name="decryptBitmap">The decrypt bitmap.</param>
        /// <param name="isDecryptStringFromBase64String">是否 从 Base64String 字符串中 解密 出 原始字符串</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentException">不是有效的加密位图数据源</exception>
        public static EncryptBitmapDigestInfoModel DecryptStringFromBitmap(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null)
        {

            EncryptBitmapDigestInfoModel encryptBitmapDigestInfoModel;

            var bitmapBase64String = GetStringFromEncryptedBitmap(encryptedBitmap);
            var encryptedBytes = Convert.FromBase64String(bitmapBase64String);
            var encryptStringDigestInfoModel = DecryptStringFromBytes(encryptedBytes, secretKey, encoding);

            if (encryptStringDigestInfoModel.IsSuccess)
            {
                var dencryptHeaderInfoModelJsonString = encryptStringDigestInfoModel.DencryptHeaderInfoModelJsonString;
                encryptBitmapDigestInfoModel = JsonSerializeHelper.DeserializeFromJson<EncryptBitmapDigestInfoModel>(dencryptHeaderInfoModelJsonString);
                encryptBitmapDigestInfoModel.DencryptHeaderInfoModelJsonString = dencryptHeaderInfoModelJsonString;
                encryptBitmapDigestInfoModel.SourceString = encryptStringDigestInfoModel.SourceString;
            }
            else
            {
                encryptBitmapDigestInfoModel = new EncryptBitmapDigestInfoModel();
            }

            encryptBitmapDigestInfoModel.IsSuccess = encryptStringDigestInfoModel.IsSuccess;
            encryptBitmapDigestInfoModel.ErrorMessage = encryptStringDigestInfoModel.ErrorMessage;

            return encryptBitmapDigestInfoModel;

        }




        public static EncryptModelBitmapDigestInfoModel<T> EncryptModelToBitmap<T>(T t, string secretKey = null, Encoding encoding = null, SecurityEncryptDirectionTypeEnum securityEncryptDirectionType = SecurityEncryptDirectionTypeEnum.ModelToBitmap, EncryptModelBitmapDigestInfoModel<T> encryptModelBitmapDigestInfoModel = null) where T : class
        {

            //EncryptModelBitmapDigestInfoModel<T> encryptModelBitmapDigestInfoModel;

            EncryptModelToBytes(t, secretKey, true, encoding, securityEncryptDirectionType, encryptModelBitmapDigestInfoModel);

            if (encryptModelBitmapDigestInfoModel.IsSuccess)
            {

                //encryptBitmapDigestInfoModel = encryptStringDigestInfoModel.AsTypeByDeepClone<EncryptDigestInfoModel, EncryptBitmapDigestInfoModel>();
                var encryptedBytes = encryptModelBitmapDigestInfoModel.EncryptedBytes;
                encryptModelBitmapDigestInfoModel.EncryptedBitmap = GetEncryptedBitmap(Convert.ToBase64String(encryptedBytes));
                encryptModelBitmapDigestInfoModel.EncryptedBytes = null;

            }


            return encryptModelBitmapDigestInfoModel;


        }


        public static EncryptModelBitmapDigestInfoModel<T> DecryptModelFromBitmap<T>(Bitmap encryptedBitmap, string secretKey = null, Encoding encoding = null) where T : class
        {

            //if (encoding.IfIsNullOrEmpty()) encoding = GlobalSettings.DEFAULT_ENCODING;

            EncryptModelBitmapDigestInfoModel<T> encryptModelBitmapDigestInfoModel;

            var bitmapBase64String = GetStringFromEncryptedBitmap(encryptedBitmap);
            var encryptedBytes = Convert.FromBase64String(bitmapBase64String);
            var encryptModelDigestInfoModel = DecryptModelFromBytes<T>(encryptedBytes, secretKey, encoding);

            if (encryptModelDigestInfoModel.IsSuccess)
            {
                var dencryptHeaderInfoModelJsonString = encryptModelDigestInfoModel.DencryptHeaderInfoModelJsonString;
                encryptModelBitmapDigestInfoModel = JsonSerializeHelper.DeserializeFromJson<EncryptModelBitmapDigestInfoModel<T>>(dencryptHeaderInfoModelJsonString);
                encryptModelBitmapDigestInfoModel.DencryptHeaderInfoModelJsonString = dencryptHeaderInfoModelJsonString;
                encryptModelBitmapDigestInfoModel.SourceModel = encryptModelDigestInfoModel.SourceModel;
            }
            else
            {
                encryptModelBitmapDigestInfoModel = new EncryptModelBitmapDigestInfoModel<T>();
            }

            encryptModelBitmapDigestInfoModel.IsSuccess = encryptModelDigestInfoModel.IsSuccess;
            encryptModelBitmapDigestInfoModel.ErrorMessage = encryptModelDigestInfoModel.ErrorMessage;



            return encryptModelBitmapDigestInfoModel;

        }



        /// <summary>
        /// Encrypts the bytes to image file.
        /// </summary>
        /// <param name="bytesToEncrypt">The bytes to encrypt.</param>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static EncryptBitmapDigestInfoModel EncryptBytesToImageFile(byte[] bytesToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null, SecurityEncryptDirectionTypeEnum securityEncryptDirectionType = SecurityEncryptDirectionTypeEnum.BytesToImageFile, EncryptBitmapDigestInfoModel encryptBitmapDigestInfoModel = null)
        {
            EncryptBytesToBitmap(bytesToEncrypt, secretKey, encoding, securityEncryptDirectionType, encryptBitmapDigestInfoModel);
            if (encryptBitmapDigestInfoModel.IsSuccess)
            {
                ImageHelper.SaveBitmapToImageFile(encryptBitmapDigestInfoModel.EncryptedBitmap, imageFileFullPath);
                encryptBitmapDigestInfoModel.EncryptedBitmap = null;
            }
            return encryptBitmapDigestInfoModel;
        }

        /// <summary>
        /// Decrypts the bytes from image file.
        /// </summary>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <returns>System.Byte[].</returns>
        public static EncryptBitmapDigestInfoModel DecryptBytesFromImageFile(string imageFileFullPath, string secretKey = null, Encoding encoding = null)
        {

            EncryptBitmapDigestInfoModel encryptBitmapDigestInfoModel;

            using (var encryptedBitmap = ImageHelper.GetBitmapFromImageFile(imageFileFullPath))
            {
                encryptBitmapDigestInfoModel = DecryptBytesFromBitmap(encryptedBitmap, secretKey, encoding);
            }


            return encryptBitmapDigestInfoModel;


        }



        /// <summary>
        /// Encrypts the string to image file.
        /// </summary>
        /// <param name="strToEncrypt">The string to encrypt.</param>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static EncryptBitmapDigestInfoModel EncryptStringToImageFile(string strToEncrypt, string imageFileFullPath, string secretKey = null, Encoding encoding = null, SecurityEncryptDirectionTypeEnum securityEncryptDirectionType = SecurityEncryptDirectionTypeEnum.StringToImageFile, EncryptBitmapDigestInfoModel encryptBitmapDigestInfoModel = null)
        {
            var result = EncryptStringToBitmap(strToEncrypt, secretKey, encoding, securityEncryptDirectionType, encryptBitmapDigestInfoModel);
            if (result.IsSuccess)
            {
                ImageHelper.SaveBitmapToImageFile(result.EncryptedBitmap, imageFileFullPath);
                result.EncryptedBitmap = null;
            }
            return result;
        }

        /// <summary>
        /// Decrypts the string from image file.
        /// </summary>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <returns>System.String.</returns>
        public static EncryptBitmapDigestInfoModel DecryptStringFromImageFile(string imageFileFullPath, string secretKey = null, Encoding encoding = null)
        {

            EncryptBitmapDigestInfoModel encryptBitmapDigestInfoModel;

            using (var encryptedBitmap = ImageHelper.GetBitmapFromImageFile(imageFileFullPath))
            {
                encryptBitmapDigestInfoModel = DecryptStringFromBitmap(encryptedBitmap, secretKey, encoding);
            }

            return encryptBitmapDigestInfoModel;

        }



        public static EncryptModelBitmapDigestInfoModel<T> EncryptModelToImageFile<T>(T t, string imageFileFullPath, string secretKey = null, Encoding encoding = null, SecurityEncryptDirectionTypeEnum securityEncryptDirectionType = SecurityEncryptDirectionTypeEnum.ModelToImageFile, EncryptModelBitmapDigestInfoModel<T> encryptModelBitmapDigestInfoModel = null) where T : class
        {
            var result = EncryptModelToBitmap(t, secretKey, encoding, securityEncryptDirectionType, encryptModelBitmapDigestInfoModel);
            if (result.IsSuccess)
            {
                ImageHelper.SaveBitmapToImageFile(result.EncryptedBitmap, imageFileFullPath);
                result.EncryptedBitmap = null;
            }
            return result;

        }


        public static EncryptModelBitmapDigestInfoModel<T> DecryptModelFromImageFile<T>(string imageFileFullPath, string secretKey = null, Encoding encoding = null) where T : class
        {

            EncryptModelBitmapDigestInfoModel<T> encryptModelBitmapDigestInfoModel;

            using (var encryptedBitmap = ImageHelper.GetBitmapFromImageFile(imageFileFullPath))
            {
                encryptModelBitmapDigestInfoModel = DecryptModelFromBitmap<T>(encryptedBitmap, secretKey, encoding);
            }

            return encryptModelBitmapDigestInfoModel;

        }




    }
}
