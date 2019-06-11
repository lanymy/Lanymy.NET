namespace Lanymy.Common.Models.SecurityModels
{
    public class EncryptFileDigestInfoModel : EncryptDigestInfoModel
    {

        /// <summary>
        /// 原文件全名称
        /// </summary>
        public string SourceFileFullName { get; set; }

        /// <summary>
        /// 加密后文件全名称
        /// </summary>
        public string EncryptedFileFullName { get; set; }


    }

}
