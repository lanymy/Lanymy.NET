namespace Xb.Common.Helpers.SecurityModels
{
    public class EncryptStringDigestInfoModel : EncryptDigestInfoModel
    {

        public string SourceString { get; set; }

        public byte[] EncryptedBytes { get; set; }
        public string EncryptedBase64String { get; set; }

    }
}
