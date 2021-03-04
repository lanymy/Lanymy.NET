using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{
    public interface ICryptoFileCore
    {

        TEncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedFile<TEncryptDigestInfoModel>(string encryptedFileFullPath, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptDigestInfoModel, new();




    }
}
