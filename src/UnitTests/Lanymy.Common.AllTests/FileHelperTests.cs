using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class FileHelperTests
    {
        [TestMethod]
        public void FileHelper_GetBinaryFileBytes_WhenFileExists_ShouldRoundTripAllBytes()
        {
            var tempFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_filehelper_{Guid.NewGuid():N}.bin");
            var sourceBytes = new byte[64 * 1024 + 17];

            try
            {
                for (var i = 0; i < sourceBytes.Length; i++)
                {
                    sourceBytes[i] = (byte)(i % 251);
                }

                File.WriteAllBytes(tempFilePath, sourceBytes);

                var actualBytes = Lanymy.Common.Helpers.FileHelper.GetBinaryFileBytes(tempFilePath);

                CollectionAssert.AreEqual(sourceBytes, actualBytes);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        [TestMethod]
        public void FileHelper_CopyFolderToNewFolerWithResult_WhenSourceExists_ShouldCopyNestedContent()
        {
            var sourceFolderPath = Path.Combine(Path.GetTempPath(), $"lanymy_copy_source_{Guid.NewGuid():N}");
            var targetFolderPath = Path.Combine(Path.GetTempPath(), $"lanymy_copy_target_{Guid.NewGuid():N}");
            var nestedFolderPath = Path.Combine(sourceFolderPath, "nested");
            var sourceFilePath = Path.Combine(nestedFolderPath, "sample.txt");
            var expectedContent = "lanymy-file-copy";

            try
            {
                Directory.CreateDirectory(nestedFolderPath);
                File.WriteAllText(sourceFilePath, expectedContent);

                var result = Lanymy.Common.Helpers.FileHelper.CopyFolderToNewFolerWithResult(sourceFolderPath, targetFolderPath);
                var copiedFilePath = Path.Combine(targetFolderPath, "nested", "sample.txt");

                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.Exception);
                Assert.IsTrue(File.Exists(copiedFilePath));
                Assert.AreEqual(expectedContent, File.ReadAllText(copiedFilePath));
            }
            finally
            {
                if (Directory.Exists(sourceFolderPath))
                {
                    Directory.Delete(sourceFolderPath, true);
                }

                if (Directory.Exists(targetFolderPath))
                {
                    Directory.Delete(targetFolderPath, true);
                }
            }
        }

        [TestMethod]
        public void FileHelper_DeleteFolderWithResult_WhenFolderMissing_ShouldCaptureException()
        {
            var sourceFolderPath = Path.Combine(Path.GetTempPath(), $"lanymy_delete_missing_{Guid.NewGuid():N}");

            var result = Lanymy.Common.Helpers.FileHelper.DeleteFolderWithResult(sourceFolderPath, false);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Exception);
            Assert.IsFalse(Directory.Exists(sourceFolderPath));
        }
    }
}
