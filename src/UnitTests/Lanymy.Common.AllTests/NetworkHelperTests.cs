using System;
using System.Net;
using Lanymy.Common.Helpers;
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
            Assert.IsNotNull(ipAllList);
            Assert.IsNotNull(ip4List);
        }

        [TestMethod]
        public void NetworkHelper_GetIpAddressByIpStringWithResult_WhenIpStringIsInvalid_ShouldCaptureArgumentError()
        {
            var result = NetworkHelper.GetIpAddressByIpStringWithResult("not_an_ip");

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Exception);
            Assert.AreEqual("not_an_ip", result.Input);
            Assert.IsNull(result.Address);
        }

        [TestMethod]
        public void NetworkHelper_GetIpAddressByIpString_WhenIpStringIsInvalid_ShouldKeepCompatibilityThrowSemantics()
        {
            ArgumentException exception = null;

            try
            {
                NetworkHelper.GetIpAddressByIpString("not_an_ip");
            }
            catch (ArgumentException ex)
            {
                exception = ex;
            }

            Assert.IsNotNull(exception);
            Assert.AreEqual("ipString", exception.ParamName);
        }

        [TestMethod]
        public void NetworkHelper_GetLocalIpListWithResult_ShouldReturnQueryableAddressList()
        {
            var result = NetworkHelper.GetLocalIpListWithResult();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Addresses);
        }

        [TestMethod]
        public void NetworkHelper_GetLocalIPWithResult_ShouldMatchCompatibilityValue()
        {
            var result = NetworkHelper.GetLocalIPWithResult();
            var compatibilityValue = NetworkHelper.GetLocalIP();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Addresses);
            Assert.AreEqual(result.AddressText ?? string.Empty, compatibilityValue);

            if (result.IsSuccess)
            {
                Assert.IsNotNull(result.Address);
                Assert.AreEqual(result.Address.ToString(), result.AddressText);
            }
            else
            {
                Assert.IsNull(result.Address);
            }
        }

        [TestMethod]
        public void NetworkHelper_PingIPWithResult_WhenIpStringIsInvalid_ShouldCaptureArgumentError()
        {
            var result = NetworkHelper.PingIPWithResult("not_an_ip");

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Exception);
            Assert.AreEqual("not_an_ip", result.AddressText);
            Assert.IsNull(result.Address);
            Assert.IsNull(result.Status);
        }

        [TestMethod]
        public void NetworkHelper_PingIP_WhenIpStringIsInvalid_ShouldKeepCompatibilityFalseSemantics()
        {
            var result = NetworkHelper.PingIP("not_an_ip");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NetworkHelper_PingIPWithResult_WhenIpAddressIsNull_ShouldCaptureArgumentNullError()
        {
            var result = NetworkHelper.PingIPWithResult((IPAddress)null);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Exception);
            Assert.IsNull(result.Address);
            Assert.IsNull(result.Status);
        }

    }



}
