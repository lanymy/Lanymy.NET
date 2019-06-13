using Lanymy.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace Lanymy.Common.Core.UnitTests
{

    [TestClass]
    public class VerificationCodeHelperTests
    {

        [TestMethod]
        public void CreateVerificationCodeTest()
        {

            for (int i = 0; i < 30; i++)
            {

                Debug.WriteLine(VerificationCodeHelper.CreateVerificationCode(isOnlyNumber: true));

            }

            string str = string.Empty;

        }

    }

}
