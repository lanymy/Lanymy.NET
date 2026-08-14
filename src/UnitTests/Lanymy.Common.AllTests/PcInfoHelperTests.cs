using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using Lanymy.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{



    [TestClass()]
    public class PcInfoHelperTests
    {





        [TestMethod()]
        [SupportedOSPlatform("windows")]
        public void PcInfoHelperTest()
        {
            var sourceFileFullPath = Environment.ProcessPath;
            Assert.IsFalse(string.IsNullOrWhiteSpace(sourceFileFullPath));

            var shortcutDirectoryPath = Path.Combine(Path.GetTempPath(), $"PcInfoHelperTests_{Guid.NewGuid():N}");
            var targetShortcutFileFullPath = Path.Combine(shortcutDirectoryPath, "ShortcutDemo");

            Directory.CreateDirectory(shortcutDirectoryPath);

            try
            {
                var result = PcInfoHelper.CreateShortcut(sourceFileFullPath, targetShortcutFileFullPath);

                Assert.IsTrue(result);
                Assert.IsTrue(File.Exists(targetShortcutFileFullPath + ".lnk"));
            }
            finally
            {
                if (Directory.Exists(shortcutDirectoryPath))
                {
                    Directory.Delete(shortcutDirectoryPath, true);
                }
            }
        }

        [TestMethod]
        public void PcInfoHelper_GetHostNameWithResult_ShouldMatchCompatibilityValue()
        {
            var result = PcInfoHelper.GetHostNameWithResult();

            Assert.IsNotNull(result);
            Assert.IsNull(result.Exception);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.HostName));
            Assert.AreEqual(PcInfoHelper.GetHostName(), result.HostName);
        }

        [TestMethod]
        public void PcInfoHelper_GetIPV4WithResult_ShouldMatchCompatibilityValue()
        {
            var result = PcInfoHelper.GetIPV4WithResult();
            var compatibilityValue = PcInfoHelper.GetIPV4();

            Assert.IsNotNull(result);
            Assert.AreEqual(compatibilityValue, result.AddressText ?? string.Empty);
            Assert.IsNotNull(result.CandidateAddresses);

            if (result.IsSuccess)
            {
                Assert.IsNotNull(result.Address);
                Assert.AreEqual(result.Address.ToString(), result.AddressText);
            }
            else
            {
                Assert.IsNull(result.Address);
                Assert.AreEqual(string.Empty, compatibilityValue);
            }
        }

        [TestMethod]
        public void PcInfoHelper_GetIPV6WithResult_ShouldMatchCompatibilityValue()
        {
            var result = PcInfoHelper.GetIPV6WithResult();
            var compatibilityValue = PcInfoHelper.GetIPV6();

            Assert.IsNotNull(result);
            Assert.AreEqual(compatibilityValue, result.AddressText ?? string.Empty);
            Assert.IsNotNull(result.CandidateAddresses);

            if (result.IsSuccess)
            {
                Assert.IsNotNull(result.Address);
                Assert.AreEqual(result.Address.ToString(), result.AddressText);
            }
            else
            {
                Assert.IsNull(result.Address);
                Assert.AreEqual(string.Empty, compatibilityValue);
            }
        }

        [TestMethod]
        public void PcInfoHelper_GetLocalIpAddressWithResult_ShouldMatchCompatibilityValue()
        {
            var result = PcInfoHelper.GetLocalIpAddressWithResult();
            var compatibilityValue = PcInfoHelper.GetLocalIpAddress();

            Assert.IsNotNull(result);
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.CandidateAddresses);
            Assert.AreEqual(compatibilityValue, result.AddressText ?? string.Empty);

            if (result.IsSuccess)
            {
                Assert.IsNotNull(result.Address);
                Assert.AreEqual(result.Address.ToString(), result.AddressText);
            }
            else
            {
                Assert.IsNull(result.Address);
                Assert.AreEqual(string.Empty, compatibilityValue);
            }
        }

        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void PcInfoHelper_CreateShortcutWithResult_WhenTargetDirectoryExists_ShouldReportSuccess()
        {
            var sourceFileFullPath = Environment.ProcessPath;
            Assert.IsFalse(string.IsNullOrWhiteSpace(sourceFileFullPath));

            var shortcutDirectoryPath = Path.Combine(Path.GetTempPath(), $"PcInfoHelperTests_{Guid.NewGuid():N}");
            var targetShortcutFileFullPath = Path.Combine(shortcutDirectoryPath, "ShortcutWithResultDemo");

            Directory.CreateDirectory(shortcutDirectoryPath);

            try
            {
                var result = PcInfoHelper.CreateShortcutWithResult(sourceFileFullPath, targetShortcutFileFullPath);

                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.Exception);
                Assert.AreEqual(targetShortcutFileFullPath + ".lnk", result.TargetPath);
                Assert.IsTrue(File.Exists(targetShortcutFileFullPath + ".lnk"));
            }
            finally
            {
                if (Directory.Exists(shortcutDirectoryPath))
                {
                    Directory.Delete(shortcutDirectoryPath, true);
                }
            }
        }

        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void PcInfoHelper_CreateShortcutWithResult_WhenTargetDirectoryMissing_ShouldCaptureException()
        {
            var sourceFileFullPath = Environment.ProcessPath;
            Assert.IsFalse(string.IsNullOrWhiteSpace(sourceFileFullPath));

            var shortcutDirectoryPath = Path.Combine(Path.GetTempPath(), $"PcInfoHelperTests_{Guid.NewGuid():N}");
            var targetShortcutFileFullPath = Path.Combine(shortcutDirectoryPath, "MissingDirectoryShortcutDemo");

            var result = PcInfoHelper.CreateShortcutWithResult(sourceFileFullPath, targetShortcutFileFullPath);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Exception);
            Assert.AreEqual(targetShortcutFileFullPath + ".lnk", result.TargetPath);
            Assert.IsFalse(File.Exists(targetShortcutFileFullPath + ".lnk"));
        }



    }



}
