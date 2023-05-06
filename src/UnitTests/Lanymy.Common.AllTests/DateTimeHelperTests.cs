using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lanymy.Common.Abstractions.Models;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.CryptoModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{



    [TestClass()]
    public class DateTimeHelperTests
    {





        [TestMethod()]
        public void DateTimeHelperTest()
        {

            var dtStart = new DateTime(2000, 1, 1);
            var dtNow = DateTime.Now;

            var a1 = dtNow.Subtract(dtStart).TotalMilliseconds;
            //var a2 = a1.ConvertToType<ulong>();
            var a2 = (ulong)a1;

            var dtEnd = dtStart.AddMilliseconds(a2);

            var dtUintMaxMilliseconds = dtStart.AddMilliseconds(uint.MaxValue);
            var dtUintMaxSeconds = dtStart.AddSeconds(uint.MaxValue);
            var dtUintMax1 = (ulong)DateTime.MaxValue.Subtract(dtStart).TotalMilliseconds;

        }



    }



}