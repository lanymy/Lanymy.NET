using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lanymy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Models;

namespace Lanymy.Common.Tests
{


    [TestClass()]
    public class NetworkHelperTests
    {


        [TestMethod()]
        public void GetIpAddressByIpStringTest()
        {

            string ipStr = "192.168.1.1";
            var ipAddress1 = NetworkHelper.GetIpAddressByIpString(ipStr);

            var ipAddress2 = new IPAddress(new byte[] { 192, 168, 1, 1 });

            Assert.AreEqual(ipAddress1.ToString(), ipAddress2.ToString());

        }


        [TestMethod()]
        public void GetLanIpInfoListTest()
        {

            Action<IpInfoModel> action = model =>
            {

                var digest = model.Digest;
                string strEnd = string.Empty;

            };

            var gatewayIpAddress = NetworkHelper.GetIpAddressByIpString("192.168.2.1");

            var list = NetworkHelper.GetLanIpInfoList(gatewayIpAddress, action);

        }



    }
}