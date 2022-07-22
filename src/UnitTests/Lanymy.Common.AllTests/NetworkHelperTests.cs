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
    public class NetworkHelperTests
    {





        [TestMethod()]
        public void NetworkHelperTest()
        {

            var ipAllList = NetworkHelper.GetLocalIpList();

            var ip4List = NetworkHelper.GetLocalIpV4List();


        }



    }



}