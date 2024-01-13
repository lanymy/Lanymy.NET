using System;
using System.Collections.Generic;
using System.Text;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.Interfaces;


namespace Lanymy.Common.Helpers
{


    public class SecurityAesHelper
    {


        public static readonly IAesCrypto DefaultLanymyCrypto = new LanymyAesCrypto();



        public static byte[] EncryptBytesToBteys(byte[] sourceBytes, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null)
        {
            return GenericityHelper.GetInterface(crypto, DefaultLanymyCrypto).EncryptBytesToBteys(sourceBytes, key, iv, encoding);
        }

        public static byte[] DecryptBytesFromBteys(byte[] encryptBytes, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null)
        {
            return GenericityHelper.GetInterface(crypto, DefaultLanymyCrypto).DecryptBytesFromBteys(encryptBytes, key, iv, encoding);
        }




        public static byte[] EncryptStringToBteys(string sourceString, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null)
        {
            return GenericityHelper.GetInterface(crypto, DefaultLanymyCrypto).EncryptStringToBteys(sourceString, key, iv, encoding);
        }
        public static string DecryptStringFromBteys(byte[] encrypBytes, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null)
        {
            return GenericityHelper.GetInterface(crypto, DefaultLanymyCrypto).DecryptStringFromBteys(encrypBytes, key, iv, encoding);
        }




        public static string EncryptStringToString(string sourceString, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null)
        {
            return GenericityHelper.GetInterface(crypto, DefaultLanymyCrypto).EncryptStringToString(sourceString, key, iv, encoding);
        }
        public static string DecryptStringFromString(string encryptString, string key = null, string iv = null, Encoding encoding = null, IAesCrypto crypto = null)
        {
            return GenericityHelper.GetInterface(crypto, DefaultLanymyCrypto).DecryptStringFromString(encryptString, key, iv, encoding);
        }






    }

}