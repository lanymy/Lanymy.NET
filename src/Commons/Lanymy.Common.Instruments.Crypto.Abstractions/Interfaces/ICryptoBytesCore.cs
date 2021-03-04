using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{


    public interface ICryptoBytesCore
    {


        /// <summary>
        /// 二进制数组加密 返回加密后的二进制数组
        /// </summary>
        /// <param name="bytesToEncrypt">要加密的二进制数组</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        TEncryptDigestInfoModel EncryptBytesToBytes<TEncryptDigestInfoModel>(byte[] bytesToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptBytesDigestInfoModel, new();

        /// <summary>
        /// 解密二进制数组 返回 解密后的二进制数组
        /// </summary>
        /// <param name="bytesToDecrypt">要解密的二进制数组</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="encoding">解密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        TEncryptDigestInfoModel DecryptBytesFromBytes<TEncryptDigestInfoModel>(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptBytesDigestInfoModel, new();

        TEncryptDigestInfoModel EncryptBytesToFile<TEncryptDigestInfoModel>(byte[] sourceBytes, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringFileDigestInfoModel, new();

        TEncryptDigestInfoModel DecryptBytesFromFile<TEncryptDigestInfoModel>(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptStringFileDigestInfoModel, new();


    }
}
