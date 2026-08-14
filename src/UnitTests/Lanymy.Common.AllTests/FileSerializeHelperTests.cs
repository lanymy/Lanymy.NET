using System;
using System.IO;
using Lanymy.Common.Abstractions.Models;
using Lanymy.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class FileSerializeHelperTests
    {
        [TestMethod]
        public void FileSerializeHelper_SerializeAndDeserializeWithResult_ShouldRoundTripModel()
        {
            var targetFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_fileserialize_{Guid.NewGuid():N}", "payload.bin");
            var sourceModel = new ScheduleFileInfoModel
            {
                SourceFileFullPath = "source.txt",
                TargetFileFullPath = "target.txt",
            };

            try
            {
                var serializeResult = FileSerializeHelper.SerializeToBytesFileWithResult(sourceModel, targetFilePath);
                var deserializeResult = FileSerializeHelper.DeserializeFromBytesFileWithResult<ScheduleFileInfoModel>(targetFilePath);

                Assert.IsTrue(serializeResult.IsSuccess);
                Assert.IsNull(serializeResult.Exception);
                Assert.AreEqual(targetFilePath, serializeResult.FilePath);
                Assert.IsTrue(serializeResult.IsCompressed);
                Assert.IsTrue(serializeResult.BytesLength > 0);
                Assert.IsTrue(File.Exists(targetFilePath));

                Assert.IsTrue(deserializeResult.IsSuccess);
                Assert.IsNull(deserializeResult.Exception);
                Assert.IsNotNull(deserializeResult.Model);
                Assert.AreEqual(sourceModel.SourceFileFullPath, deserializeResult.Model.SourceFileFullPath);
                Assert.AreEqual(sourceModel.TargetFileFullPath, deserializeResult.Model.TargetFileFullPath);
                Assert.IsTrue(deserializeResult.BytesLength > 0);
            }
            finally
            {
                var targetDirectoryPath = Path.GetDirectoryName(targetFilePath);
                if (!string.IsNullOrEmpty(targetDirectoryPath) && Directory.Exists(targetDirectoryPath))
                {
                    Directory.Delete(targetDirectoryPath, true);
                }
            }
        }

        [TestMethod]
        public void FileSerializeHelper_DeserializeFromBytesFileWithResult_WhenFileMissing_ShouldCaptureException()
        {
            var targetFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_fileserialize_missing_{Guid.NewGuid():N}", "payload.bin");

            var result = FileSerializeHelper.DeserializeFromBytesFileWithResult<ScheduleFileInfoModel>(targetFilePath);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Exception);
            Assert.IsInstanceOfType(result.Exception, typeof(FileNotFoundException));
            Assert.IsNull(result.Model);
        }

        [TestMethod]
        public void FileSerializeHelper_DeserializeFromBytesFile_WhenFileMissing_ShouldKeepCompatibilityThrowSemantics()
        {
            var targetFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_fileserialize_missing_{Guid.NewGuid():N}", "payload.bin");

            try
            {
                FileSerializeHelper.DeserializeFromBytesFile<ScheduleFileInfoModel>(targetFilePath);
                Assert.Fail("Expected FileNotFoundException.");
            }
            catch (FileNotFoundException)
            {
            }
        }
    }
}
