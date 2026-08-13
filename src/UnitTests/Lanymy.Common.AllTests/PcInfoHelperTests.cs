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



    }



}
