using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{

    public interface ICryptoFile
    {


        EncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedFile(string encryptedFileFullPath, Encoding encoding = null);



        EncryptStringFileDigestInfoModel EncryptFileToFile(string sourceFileFullPath, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null);



        EncryptStringFileDigestInfoModel DecryptFileFromFile(string encryptedFileFullPath, string sourceFileFullPath, string secretKey = null, Encoding encoding = null);




    }


}
