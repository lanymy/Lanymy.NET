namespace Lanymy.Common.Instruments.CryptoModels
{
    //public class EncryptStringDigestInfoModel : EncryptDigestInfoModel
    public class EncryptStringDigestInfoModel : EncryptBytesDigestInfoModel
    {

        public string SourceString { get; set; }


    }
}
