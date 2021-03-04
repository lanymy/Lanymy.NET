using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{
    public interface ICryptoModel
    {

        /// <summary>
        /// 加密并序列化Model成字节数组
        /// </summary>
        /// <typeparam name="T">要加密序列化的实体类型</typeparam>
        /// <param name="t">要加密序列化的实体实例</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="ifRandom">是否随机不重复 True 随机; False 不随机; 默认值True</param>
        /// <param name="encoding">加密使用的编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        EncryptModelDigestInfoModel<T> EncryptModelToBytes<T>(T t, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where T : class;


        /// <summary>
        /// 解密并反序列化字节数组 返回 Model
        /// </summary>
        /// <typeparam name="T">要解密并反序列化的实体类型</typeparam>
        /// <param name="ecryptedBytes">要解密并反序列化的实体实例</param>
        /// <param name="secretKey">密钥 , Null 表示 使用默认密钥</param>
        /// <param name="encoding">编码 , Null 表示 使用默认编码</param>
        /// <returns></returns>
        EncryptModelDigestInfoModel<T> DecryptModelFromBytes<T>(byte[] ecryptedBytes, string secretKey = null, Encoding encoding = null) where T : class;


        /// <summary>
        /// 加密并序列化Model成Base64字符串
        /// </summary>
        /// <typeparam name="T">要序列化的实体类型</typeparam>
        /// <param name="t">要序列化的实体 实例</param>
        /// <param name="secretKey">密钥 Null 使用默认密钥</param>
        /// <returns></returns>
        EncryptModelDigestInfoModel<T> EncryptModelToBase64String<T>(T t, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where T : class;


        /// <summary>
        /// 解密并反序列化Base64字符串成实体实例
        /// </summary>
        /// <typeparam name="T">要反序列化成的实体类型</typeparam>
        /// <param name="encryptBase64String">要解密的Base64字符串</param>
        /// <param name="secretKey">密钥 Null 使用默认密钥</param>
        /// <returns></returns>
        EncryptModelDigestInfoModel<T> DecryptModelFromBase64String<T>(string encryptBase64String, string secretKey = null, Encoding encoding = null) where T : class;



        EncryptModelFileDigestInfoModel<T> EncryptModelToFile<T>(T t, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null) where T : class;


        EncryptModelFileDigestInfoModel<T> DecryptModelFromFile<T>(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null) where T : class;






    }
}
