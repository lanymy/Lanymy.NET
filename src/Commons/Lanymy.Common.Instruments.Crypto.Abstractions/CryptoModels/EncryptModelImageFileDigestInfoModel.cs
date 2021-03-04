using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.Interfaces;

namespace Lanymy.Common.Instruments.CryptoModels
{
    public class EncryptModelImageFileDigestInfoModel<T> : EncryptStringImageFileDigestInfoModel, ICryptoModelProperty<T> where T : class
    {

        public string ModelTypeName { get; set; }
        public string ModelTypeFullName { get; set; }
        public T SourceModel { get; set; }

    }
}
