using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.Core.UnitTests
{

    [TestClass()]
    public class FileHelperTests
    {

        [TestMethod()]
        public void GetStreamHashCodeTest()
        {

            var sourceStr = "123";
            var bytess = Encoding.UTF8.GetBytes(sourceStr);
            var sourceHashCode = "A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3";
            var hashCode = string.Empty;

            using (var ms = new MemoryStream(bytess))
            {

                hashCode = FileHelper.GetStreamHashCode(ms);

                var hashCodeMD5 = FileHelper.GetStreamHashCode(ms, 0, HashAlgorithmTypeEnum.MD5);
                var hashCodeSHA1 = FileHelper.GetStreamHashCode(ms, 0, HashAlgorithmTypeEnum.SHA1);
                var hashCodeSHA256 = FileHelper.GetStreamHashCode(ms, 0, HashAlgorithmTypeEnum.SHA256);

            }




            Assert.AreEqual(hashCode, sourceHashCode);

        }

    }
}