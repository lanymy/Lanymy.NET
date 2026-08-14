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

        [TestMethod]
        public void GetFolderPath_ShouldReturnEmpty_WhenInputIsRelativeFileNameWithExtension()
        {
            var folderPath = PathHelper.GetFolderPath("README.txt");

            Assert.AreEqual(string.Empty, folderPath);
        }

        [TestMethod]
        public void GetFolderPath_ShouldReturnExistingFileParent_WhenFileHasNoExtension()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var fileFullPath = Path.Combine(tempDirectory, "README");

            Directory.CreateDirectory(tempDirectory);
            File.WriteAllText(fileFullPath, "test");

            try
            {
                var folderPath = PathHelper.GetFolderPath(fileFullPath);

                Assert.AreEqual(tempDirectory + Path.DirectorySeparatorChar, folderPath);
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
        public void GetFolderPath_ShouldNormalizeAltDirectorySeparatorSuffix()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var altDirectoryPath = tempDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.AltDirectorySeparatorChar;

            var folderPath = PathHelper.GetFolderPath(altDirectoryPath);

            Assert.AreEqual(tempDirectory + Path.DirectorySeparatorChar, folderPath);
        }

        [TestMethod]
        public void InitDirectoryPath_ShouldCreateDirectory_WhenInputUsesAltDirectorySeparator()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var nestedDirectory = Path.Combine(tempDirectory, "nested");
            var altDirectoryPath = nestedDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.AltDirectorySeparatorChar;

            try
            {
                PathHelper.InitDirectoryPath(altDirectoryPath);

                Assert.IsTrue(Directory.Exists(nestedDirectory));
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
        public void InitDirectoryPath_ShouldKeepNoOp_WhenInputIsRelativeFileNameWithExtension()
        {
            var relativeFileName = $"README_{Guid.NewGuid():N}.txt";
            var expectedFilePath = Path.Combine(Directory.GetCurrentDirectory(), relativeFileName);

            try
            {
                PathHelper.InitDirectoryPath(relativeFileName);

                Assert.IsFalse(File.Exists(expectedFilePath));
                Assert.IsFalse(Directory.Exists(expectedFilePath));
            }
            finally
            {
                if (File.Exists(expectedFilePath))
                {
                    File.Delete(expectedFilePath);
                }

                if (Directory.Exists(expectedFilePath))
                {
                    Directory.Delete(expectedFilePath, true);
                }
            }
        }

        [TestMethod]
        public void GetFileName_ShouldReturnExistingFileName_WhenFileHasNoExtension()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var fileFullPath = Path.Combine(tempDirectory, "README");

            Directory.CreateDirectory(tempDirectory);
            File.WriteAllText(fileFullPath, "test");

            try
            {
                var fileName = PathHelper.GetFileName(fileFullPath);

                Assert.AreEqual("README", fileName);
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
        public void GetPathType_ShouldTreatDirectorySyntaxAsDirectory_WhenDirectoryDoesNotExistYet()
        {
            var futureDirectoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")) + Path.DirectorySeparatorChar;

            var pathType = PathHelper.GetPathType(futureDirectoryPath);

            Assert.AreEqual(PathTypeEnum.Directory, pathType);
        }
    }
}
