using Lanymy.Common.Instruments.Interfaces;

namespace Lanymy.Common.Instruments.CryptoModels
{
    public class EncryptStringFileDigestInfoModel : EncryptStringDigestInfoModel, ICryptoFileProperty
    {

        /// <summary>
        /// 原文件全名称
        /// </summary>
        public string SourceFileFullPath { get; set; }

        /// <summary>
        /// 加密后文件全名称
        /// </summary>
        public string EncryptedFileFullPath { get; set; }


    }

}
