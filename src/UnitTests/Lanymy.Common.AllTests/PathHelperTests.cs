using System;
using System.IO;
using Lanymy.Common.Enums;
using Lanymy.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class PathHelperTests
    {
        [TestMethod]
        public void GetPathType_ShouldTreatExistingFileWithoutExtensionAsFile()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var fileFullPath = Path.Combine(tempDirectory, "README");

            Directory.CreateDirectory(tempDirectory);
            File.WriteAllText(fileFullPath, "test");

            try
            {
                var pathType = PathHelper.GetPathType(fileFullPath);

                Assert.AreEqual(PathTypeEnum.File, pathType);
            }
            finally
            {
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, true);
                }
            }
        }

        [TestMethod]
        public void GetFolderPath_ShouldKeepExistingDirectoryWithDotSuffix()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var dottedDirectory = Path.Combine(tempDirectory, "archive.v1");

            Directory.CreateDirectory(dottedDirectory);

            try
            {
                var folderPath = PathHelper.GetFolderPath(dottedDirectory);

                StringAssert.StartsWith(folderPath, dottedDirectory);
                Assert.AreEqual(Path.DirectorySeparatorChar, folderPath[folderPath.Length - 1]);
            }
            finally
            {
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, true);
                }
            }
        }
    }
}
