using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{
    public interface ICryptoString
    {



        EncryptStringDigestInfoModel EncryptStringToBytes(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null);


        EncryptStringDigestInfoModel DecryptStringFromBytes(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null);



        EncryptBase64StringDigestInfoModel EncryptStringToBase64String(string strToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null);



        EncryptBase64StringDigestInfoModel DecryptStringFromBase64String(string base64StringToDecrypt, string secretKey = null, Encoding encoding = null);



        EncryptStringFileDigestInfoModel EncryptStringToFile(string sourceString, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null);




        EncryptStringFileDigestInfoModel DecryptStringFromFile(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null);





    }
}
