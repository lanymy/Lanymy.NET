using System;
using System.IO;
using System.Threading.Tasks;
using Lanymy.Common.Abstractions.Models;
using Lanymy.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class JsonSerializeHelperTests
    {
        [TestMethod]
        public void JsonSerializeHelper_SerializeAndDeserializeJsonFileWithResult_ShouldRoundTripModel()
        {
            var targetFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_jsonserialize_{Guid.NewGuid():N}", "payload.json");
            var sourceModel = new ScheduleFileInfoModel
            {
                SourceFileFullPath = "source.txt",
                TargetFileFullPath = "target.txt",
            };

            try
            {
                var serializeResult = JsonSerializeHelper.SerializeToJsonFileWithResult(sourceModel, targetFilePath);
                var deserializeResult = JsonSerializeHelper.DeserializeFromJsonFileWithResult<ScheduleFileInfoModel>(targetFilePath);

                Assert.IsTrue(serializeResult.IsSuccess);
                Assert.IsNull(serializeResult.Exception);
                Assert.AreEqual(targetFilePath, serializeResult.FilePath);
                Assert.IsFalse(string.IsNullOrWhiteSpace(serializeResult.JsonContent));
                Assert.IsTrue(File.Exists(targetFilePath));

                Assert.IsTrue(deserializeResult.IsSuccess);
                Assert.IsNull(deserializeResult.Exception);
                Assert.IsNotNull(deserializeResult.Model);
                Assert.AreEqual(sourceModel.SourceFileFullPath, deserializeResult.Model.SourceFileFullPath);
                Assert.AreEqual(sourceModel.TargetFileFullPath, deserializeResult.Model.TargetFileFullPath);
                Assert.IsFalse(string.IsNullOrWhiteSpace(deserializeResult.JsonContent));
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
        public async Task JsonSerializeHelper_AsyncJsonFileWithResult_ShouldRoundTripModel()
        {
            var targetFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_jsonserialize_async_{Guid.NewGuid():N}", "payload.json");
            var sourceModel = new ScheduleFileInfoModel
            {
                SourceFileFullPath = "async-source.txt",
                TargetFileFullPath = "async-target.txt",
            };

            try
            {
                var serializeResult = await JsonSerializeHelper.SerializeToJsonFileWithResultAsync(sourceModel, targetFilePath);
                var deserializeResult = await JsonSerializeHelper.DeserializeFromJsonFileWithResultAsync<ScheduleFileInfoModel>(targetFilePath);

                Assert.IsTrue(serializeResult.IsSuccess);
                Assert.IsNull(serializeResult.Exception);
                Assert.IsTrue(File.Exists(targetFilePath));

                Assert.IsTrue(deserializeResult.IsSuccess);
                Assert.IsNull(deserializeResult.Exception);
                Assert.IsNotNull(deserializeResult.Model);
                Assert.AreEqual(sourceModel.SourceFileFullPath, deserializeResult.Model.SourceFileFullPath);
                Assert.AreEqual(sourceModel.TargetFileFullPath, deserializeResult.Model.TargetFileFullPath);
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
        public void JsonSerializeHelper_DeserializeFromJsonFileWithResult_WhenFileMissing_ShouldCaptureException()
        {
            var targetFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_jsonserialize_missing_{Guid.NewGuid():N}", "payload.json");

            var result = JsonSerializeHelper.DeserializeFromJsonFileWithResult<ScheduleFileInfoModel>(targetFilePath);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Exception);
            Assert.IsInstanceOfType(result.Exception, typeof(FileNotFoundException));
            Assert.IsNull(result.Model);
        }

        [TestMethod]
        public void JsonSerializeHelper_DeserializeFromJsonFile_WhenFileMissing_ShouldKeepCompatibilityReturnDefaultSemantics()
        {
            var targetFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_jsonserialize_missing_{Guid.NewGuid():N}", "payload.json");

            try
            {
                var result = JsonSerializeHelper.DeserializeFromJsonFile<ScheduleFileInfoModel>(targetFilePath);

                Assert.IsNull(result);
                Assert.IsTrue(File.Exists(targetFilePath));
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
    }
}
