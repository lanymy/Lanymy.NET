using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{
    /// <summary>
    /// 定义返回具体摘要模型的加密文件元数据读取入口。
    /// </summary>
    public interface ICryptoFileCore
    {
        /// <summary>
        /// 从加密文件读取指定类型的摘要信息。
        /// </summary>
        TEncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedFile<TEncryptDigestInfoModel>(string encryptedFileFullPath, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptDigestInfoModel, new();
    }
}
