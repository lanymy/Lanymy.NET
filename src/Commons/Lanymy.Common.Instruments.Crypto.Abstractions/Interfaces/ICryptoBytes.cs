using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{


    public interface ICryptoBytes
    {



        EncryptBytesDigestInfoModel EncryptBytesToBytes(byte[] bytesToEncrypt, string secretKey = null, bool ifRandom = true, Encoding encoding = null);




        EncryptBytesDigestInfoModel DecryptBytesFromBytes(byte[] bytesToDecrypt, string secretKey = null, Encoding encoding = null);






        EncryptStringFileDigestInfoModel EncryptBytesToFile(byte[] sourceBytes, string encryptFileFullPath, string secretKey = null, bool ifRandom = true, Encoding encoding = null);







        EncryptStringFileDigestInfoModel DecryptBytesFromFile(string encryptedFileFullPath, string secretKey = null, Encoding encoding = null);





    }


}