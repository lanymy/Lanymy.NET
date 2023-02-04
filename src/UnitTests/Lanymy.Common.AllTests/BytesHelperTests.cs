using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lanymy.Common.Abstractions.Models;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.CryptoModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{



    [TestClass()]
    public class BytesHelperTests
    {





        [TestMethod()]
        public void BytesHelperTest()
        {

            var bytes = new byte[]
            {
                255,
                255,
                255,
            };

            var str = BytesHelper.HexStringFromBytes(bytes);

        }



    }



}