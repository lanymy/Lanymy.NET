namespace Lanymy.Common.Models.SecurityModels
{
    public class EncryptModelDigestInfoModel<T> : EncryptDigestInfoModel where T : class
    {

        public string ModelTypeName { get; set; }
        public string ModelTypeFullName { get; set; }
        public T SourceModel { get; set; }
        public byte[] EncryptedBytes { get; set; }
        public string EncryptedBase64String { get; set; }

    }
}
