using Lanymy.Common.Instruments.Interfaces;

namespace Lanymy.Common.Instruments.CryptoModels
{
    public class EncryptModelBitmapDigestInfoModel<T> : EncryptStringBitmapDigestInfoModel, ICryptoModelProperty<T> where T : class
    {

        public string ModelTypeName { get; set; }
        public string ModelTypeFullName { get; set; }
        public T SourceModel { get; set; }

    }
}
