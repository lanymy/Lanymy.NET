using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class CompressionHelperTests
    {
        [TestMethod]
        public void CompressionHelper_CompressAndDecompressFileWithResult_ShouldRoundTripContent()
        {
            var rootPath = Path.Combine(Path.GetTempPath(), $"lanymy_compress_{Guid.NewGuid():N}");
            var sourceFilePath = Path.Combine(rootPath, "source.txt");
            var compressFilePath = Path.Combine(rootPath, "source.gz");
            var decompressedFilePath = Path.Combine(rootPath, "source.out.txt");
            var sourceContent = "lanymy compression file result";

            Directory.CreateDirectory(rootPath);
            File.WriteAllText(sourceFilePath, sourceContent, Encoding.UTF8);

            try
            {
                var compressResult = CompressionHelper.CompressSourceFileToCompressFileWithResult(sourceFilePath, compressFilePath);
                var decompressResult = CompressionHelper.DecompressSourceFileFromCompressFileWithResult(decompressedFilePath, compressFilePath);

                Assert.IsTrue(compressResult.IsSuccess);
                Assert.IsNull(compressResult.Exception);
                Assert.IsTrue(compressResult.IsCompressOperation);
                Assert.IsTrue(compressResult.TargetFileExists);
                Assert.AreEqual(sourceFilePath, compressResult.SourcePath);
                Assert.AreEqual(compressFilePath, compressResult.TargetPath);

                Assert.IsTrue(decompressResult.IsSuccess);
                Assert.IsNull(decompressResult.Exception);
                Assert.IsFalse(decompressResult.IsCompressOperation);
                Assert.IsTrue(decompressResult.TargetFileExists);
                Assert.AreEqual(compressFilePath, decompressResult.SourcePath);
                Assert.AreEqual(decompressedFilePath, decompressResult.TargetPath);
                Assert.AreEqual(sourceContent, File.ReadAllText(decompressedFilePath, Encoding.UTF8));
            }
            finally
            {
                if (Directory.Exists(rootPath))
                {
                    Directory.Delete(rootPath, true);
                }
            }
        }

        [TestMethod]
        public async Task CompressionHelper_CompressFileWithResultAsync_WhenSourceMissing_ShouldCaptureException()
        {
            var rootPath = Path.Combine(Path.GetTempPath(), $"lanymy_compress_missing_{Guid.NewGuid():N}");
            var sourceFilePath = Path.Combine(rootPath, "missing.txt");
            var compressFilePath = Path.Combine(rootPath, "missing.gz");

            var result = await CompressionHelper.CompressSourceFileToCompressFileWithResultAsync(sourceFilePath, compressFilePath);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Exception);
            Assert.IsInstanceOfType(result.Exception, typeof(FileNotFoundException));
            Assert.IsFalse(result.TargetFileExists);
        }

        [TestMethod]
        public void CompressionHelper_CompressSourceFileToCompressFile_WhenSourceMissing_ShouldKeepCompatibilityNoOpSemantics()
        {
            var rootPath = Path.Combine(Path.GetTempPath(), $"lanymy_compress_missing_{Guid.NewGuid():N}");
            var sourceFilePath = Path.Combine(rootPath, "missing.txt");
            var compressFilePath = Path.Combine(rootPath, "missing.gz");

            CompressionHelper.CompressSourceFileToCompressFile(sourceFilePath, compressFilePath);

            Assert.IsFalse(File.Exists(compressFilePath));
        }

        [TestMethod]
        public async Task CompressionHelper_DecompressSourceFileFromCompressFileAsync_WhenSourceMissing_ShouldKeepCompatibilityNoOpSemantics()
        {
            var rootPath = Path.Combine(Path.GetTempPath(), $"lanymy_decompress_missing_{Guid.NewGuid():N}");
            var sourceFilePath = Path.Combine(rootPath, "output.txt");
            var compressFilePath = Path.Combine(rootPath, "missing.gz");

            await CompressionHelper.DecompressSourceFileFromCompressFileAsync(sourceFilePath, compressFilePath);

            Assert.IsFalse(File.Exists(sourceFilePath));
        }
    }
}
