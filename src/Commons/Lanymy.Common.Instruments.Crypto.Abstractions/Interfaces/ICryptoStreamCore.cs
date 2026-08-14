using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{
    /// <summary>
    /// 定义返回具体摘要模型的流加解密泛型入口。
    /// </summary>
    public interface ICryptoStreamCore
    {
        /// <summary>
        /// 把原始流加密写入目标流，并返回指定摘要模型。
        /// </summary>
        TEncryptDigestInfoModel EncryptStreamToStream<TEncryptDigestInfoModel>(Stream sourceStream, Stream encryptStream, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptDigestInfoModel, new();

        /// <summary>
        /// 从加密流读取指定类型的摘要信息。
        /// </summary>
        TEncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedStream<TEncryptDigestInfoModel>(Stream encryptedStream, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptDigestInfoModel, new();

        /// <summary>
        /// 把加密流解密写入目标流，并返回指定摘要模型。
        /// </summary>
        TEncryptDigestInfoModel DencryptStreamFromStream<TEncryptDigestInfoModel>(Stream encryptedStream, Stream sourceStream, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptDigestInfoModel, new();
    }
}
