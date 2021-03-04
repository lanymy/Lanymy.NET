namespace Lanymy.Common.Instruments.CryptoModels
{

    public class EncryptModelDigestInfoModel<T> : EncryptBase64StringDigestInfoModel where T : class
    {

        public string ModelTypeName { get; set; }
        public string ModelTypeFullName { get; set; }
        public T SourceModel { get; set; }


    }

}
