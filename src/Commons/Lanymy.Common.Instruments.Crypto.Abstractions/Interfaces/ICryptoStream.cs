using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Enums;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{
    /// <summary>
    /// 数据流加密/解密
    /// </summary>
    public interface ICryptoStream
    {
        /// <summary>
        /// 把原始数据流加密写入目标流。
        /// </summary>
        EncryptDigestInfoModel EncryptStreamToStream(Stream sourceStream, Stream encryptStream, string secretKey = null, bool ifRandom = true, Encoding encoding = null);

        /// <summary>
        /// 从加密数据流中读取摘要信息，而不解密正文。
        /// </summary>
        EncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedStream(Stream encryptedStream, Encoding encoding = null);

        /// <summary>
        /// 把加密数据流解密写入目标流。
        /// </summary>
        EncryptDigestInfoModel DencryptStreamFromStream(Stream encryptedStream, Stream sourceStream, string secretKey = null, Encoding encoding = null);
    }
}
