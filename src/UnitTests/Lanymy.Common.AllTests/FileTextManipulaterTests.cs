using System;
using System.IO;
using Lanymy.Common.Instruments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class FileTextManipulaterTests
    {
        private static string CreateTempDirectory()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), "LanymyFileTextManipulaterTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        [TestMethod]
        public void FileTextWriter_ShouldCreateParentDirectoryAndAppendByDefault()
        {
            var tempDirectory = CreateTempDirectory();
            var textFileFullPath = Path.Combine(tempDirectory, "nested", "demo.txt");

            try
            {
                using (var writer = new FileTextWriter(textFileFullPath))
                {
                    writer.Write("A");
                }

                using (var writer = new FileTextWriter(textFileFullPath))
                {
                    writer.Write("B");
                }

                using (var reader = new FileTextReader(textFileFullPath))
                {
                    Assert.AreEqual("AB", reader.ReadAll());
                }

                Assert.IsTrue(Directory.Exists(Path.GetDirectoryName(textFileFullPath)));
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
        public void FileTextWriter_ShouldOverwriteExistingFile_WhenIfOverWriteFileIsTrue()
        {
            var tempDirectory = CreateTempDirectory();
            var textFileFullPath = Path.Combine(tempDirectory, "overwrite.txt");

            try
            {
                using (var writer = new FileTextWriter(textFileFullPath))
                {
                    writer.Write("legacy");
                }

                using (var writer = new FileTextWriter(textFileFullPath, true))
                {
                    writer.Write("fresh");
                }

                using (var reader = new FileTextReader(textFileFullPath))
                {
                    Assert.AreEqual("fresh", reader.ReadAll());
                }
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
        public void FileTextReader_ShouldCreateEmptyFile_WhenTargetFileDoesNotExist()
        {
            var tempDirectory = CreateTempDirectory();
            var textFileFullPath = Path.Combine(tempDirectory, "missing", "empty.txt");

            try
            {
                Assert.IsFalse(File.Exists(textFileFullPath));

                using (var reader = new FileTextReader(textFileFullPath))
                {
                    Assert.AreEqual(string.Empty, reader.ReadAll());
                }

                Assert.IsTrue(File.Exists(textFileFullPath));
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
