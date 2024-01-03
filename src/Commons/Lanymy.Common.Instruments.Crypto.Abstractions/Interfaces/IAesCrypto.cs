using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Interfaces
{


    public interface IAesCrypto
    {


        //void EncryptStreamToStream(Stream sourceStream, Stream encryptStream, string secretKey = null, string iv = null, Encoding encoding = null);

        //void DencryptStreamFromStream(Stream encryptedStream, Stream sourceStream, string secretKey = null, Encoding encoding = null);



        byte[] EncryptBytesToBteys(byte[] sourceBytes, string key = null, string iv = null, Encoding encoding = null);

        byte[] DecryptBytesFromBteys(byte[] encryptBytes, string key = null, string iv = null, Encoding encoding = null);





        byte[] EncryptStringToBteys(string sourceString, string key = null, string iv = null, Encoding encoding = null);

        string DecryptStringFromBteys(byte[] encrypBytes, string key = null, string iv = null, Encoding encoding = null);





        string EncryptStringToString(string sourceString, string key = null, string iv = null, Encoding encoding = null);

        string DecryptStringFromString(string encryptString, string key = null, string iv = null, Encoding encoding = null);



    }


}
