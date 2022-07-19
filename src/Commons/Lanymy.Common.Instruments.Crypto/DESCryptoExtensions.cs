using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Lanymy.Common.Instruments
{


    internal static class DESCryptoExtensions
    {

        public static ICryptoTransform CreateWeakEncryptor(this TripleDESCryptoServiceProvider cryptoProvider, byte[] key, byte[] iv)
        {

            var aa = cryptoProvider.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);

            // reflective way of doing what CreateEncryptor() does, bypassing the check for weak keys
            MethodInfo mi = cryptoProvider.GetType().GetMethod("_NewEncryptor", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] Par = { key, cryptoProvider.Mode, iv, cryptoProvider.FeedbackSize, 0 };
            ICryptoTransform trans = mi.Invoke(cryptoProvider, Par) as ICryptoTransform;
            return trans;
        }


        public static ICryptoTransform CreateWeakEncryptor(this TripleDESCryptoServiceProvider cryptoProvider)
        {
            return CreateWeakEncryptor(cryptoProvider, cryptoProvider.Key, cryptoProvider.IV);
        }

        public static ICryptoTransform CreateWeakDecryptor(this TripleDESCryptoServiceProvider cryptoProvider, byte[] key, byte[] iv)
        {
            // reflective way of doing what CreateDecryptor() does, bypassing the check for weak keys
            MethodInfo mi = cryptoProvider.GetType().GetMethod("_NewEncryptor", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] Par = { key, cryptoProvider.Mode, iv, cryptoProvider.FeedbackSize, 1 };
            ICryptoTransform trans = mi.Invoke(cryptoProvider, Par) as ICryptoTransform;
            return trans;
        }

        public static ICryptoTransform CreateWeakDecryptor(this TripleDESCryptoServiceProvider cryptoProvider)
        {
            return CreateWeakDecryptor(cryptoProvider, cryptoProvider.Key, cryptoProvider.IV);
        }
    }

}
