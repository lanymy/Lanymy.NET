namespace Lanymy.Common.Helpers.SecurityModels
{
    public class EncryptBytesDigestInfoModel : EncryptDigestInfoModel
    {

        /// <summary>
        /// 源二进制数组
        /// </summary>
        public byte[] SourceBytes { get; set; }

        /// <summary>
        /// 加密后二进制数组
        /// </summary>
        public byte[] EncryptedBytes { get; set; }

        //public string EncryptFileFullName { get; set; }
        public string EncryptFileFullPath { get; set; }


    }

}
