using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{


    public interface ICryptoStringCore
    {



        /// <summary>
        /// 字符串加密 返回 加密后的二进制数组
        /// </summary>
        /// <param name="strToEncrypt">要加密的字符串</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        TEncryptDigestInfoModel EncryptStringToBytes<TEncryptDigestInfoModel>(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringDigestInfoModel, new();


        /// <summary>
        /// 解密二进制数组 返回 解密后的字符串
        /// </summary>
        /// <param name="bytesToDecrypt">要解密的二进制数组</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="encoding">解密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        TEncryptDigestInfoModel DecryptStringFromBytes<TEncryptDigestInfoModel>(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringDigestInfoModel, new();


        /// <summary>
        /// 字符串加密 返回 加密后的 Base64 字符串
        /// </summary>
        /// <param name="strToEncrypt">要加密的字符串</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        TEncryptDigestInfoModel EncryptStringToBase64String<TEncryptDigestInfoModel>(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptBase64StringDigestInfoModel, new();


        /// <summary>
        /// 解密字符串 返回 解密后的字符串
        /// </summary>
        /// <param name="base64StringToDecrypt">要解密的Base64字符串</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="encoding">解密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        TEncryptDigestInfoModel DecryptStringFromBase64String<TEncryptDigestInfoModel>(string base64StringToDecrypt, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptBase64StringDigestInfoModel, new();


        TEncryptDigestInfoModel EncryptStringToFile<TEncryptDigestInfoModel>(string sourceString, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringFileDigestInfoModel, new();


        TEncryptDigestInfoModel DecryptStringFromFile<TEncryptDigestInfoModel>(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringFileDigestInfoModel, new();





    }
}
