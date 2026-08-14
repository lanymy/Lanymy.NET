using System;
using System.IO;
using System.Text;
using Lanymy.Common.Abstractions.Models;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class LanymyIsolatedStorageTests
    {
        private sealed class ThrowingSerializeModel
        {
            public string Name => throw new InvalidOperationException("serialize-failed");
        }

        private static string CreateTempDirectory()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), "LanymyIsolatedStorageTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        [TestMethod]
        public void SaveStringWithResult_ShouldCaptureFileContextAndRoundTrip_WhenUsingCustomStorage()
        {
            var tempDirectory = CreateTempDirectory();

            try
            {
                var isolatedStorage = new LanymyIsolatedStorage(tempDirectory);
                const string token = "lanymy-string";
                const string sourceString = "lanymy-isolated-storage";

                var saveResult = IsolatedStorageHelper.SaveStringWithResult(sourceString, token, isolatedStorageString: isolatedStorage);
                var getResult = IsolatedStorageHelper.GetStringWithResult(token, isolatedStorageString: isolatedStorage);

                Assert.IsTrue(saveResult.IsSuccess);
                Assert.IsTrue(saveResult.IfUsesCustomIsolatedStorageMode);
                Assert.IsTrue(saveResult.StorageFileExists);
                Assert.IsFalse(string.IsNullOrEmpty(saveResult.StorageFileName));
                Assert.IsFalse(string.IsNullOrEmpty(saveResult.FilePath));
                Assert.IsTrue(File.Exists(saveResult.FilePath));
                Assert.AreEqual(sourceString, getResult.SourceString);
                Assert.IsTrue(getResult.IsSuccess);
                Assert.AreEqual(saveResult.StorageFileName, getResult.StorageFileName);
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
        public void GetStringWithResult_ShouldReturnFileNotFoundFailure_WhenStorageFileMissing()
        {
            var tempDirectory = CreateTempDirectory();

            try
            {
                var isolatedStorage = new LanymyIsolatedStorage(tempDirectory);

                var result = IsolatedStorageHelper.GetStringWithResult("missing-token", isolatedStorageString: isolatedStorage);
                var legacyResult = IsolatedStorageHelper.GetString("missing-token", isolatedStorageString: isolatedStorage);

                Assert.IsFalse(result.IsSuccess);
                Assert.IsInstanceOfType(result.Exception, typeof(FileNotFoundException));
                Assert.AreEqual(result.Exception.Message, result.ErrorMessage);
                Assert.IsFalse(result.StorageFileExists);
                Assert.AreEqual(string.Empty, legacyResult);
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
        public void GetStringWithResult_ShouldKeepContentFailureInResult_WhenSecurityKeyIsWrong()
        {
            var tempDirectory = CreateTempDirectory();

            try
            {
                var isolatedStorage = new LanymyIsolatedStorage(tempDirectory);
                const string token = "wrong-key-token";
                const string sourceString = "lanymy-secret";

                IsolatedStorageHelper.SaveString(sourceString, token, "correct-key", Encoding.UTF8, isolatedStorage);

                var result = IsolatedStorageHelper.GetStringWithResult(token, "wrong-key", Encoding.UTF8, isolatedStorage);
                var legacyResult = IsolatedStorageHelper.GetString(token, "wrong-key", Encoding.UTF8, isolatedStorage);

                Assert.IsFalse(result.IsSuccess);
                Assert.IsNull(result.Exception);
                Assert.AreEqual("解密失败,密钥或加密内容无效", result.ErrorMessage);
                Assert.IsTrue(result.StorageFileExists);
                Assert.IsNull(legacyResult);
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
        public void SaveModelWithResult_ShouldUseDefaultTypeTokenAndRoundTrip_WhenTokenIsNull()
        {
            var tempDirectory = CreateTempDirectory();

            try
            {
                var isolatedStorage = new LanymyIsolatedStorage(tempDirectory);
                var model = new ScheduleFileInfoModel
                {
                    SourceFileFullPath = "source.txt",
                    TargetFileFullPath = "target.txt",
                };

                var saveResult = IsolatedStorageHelper.SaveModelWithResult(model, isolatedStorageModel: isolatedStorage);
                var getResult = IsolatedStorageHelper.GetModelWithResult<ScheduleFileInfoModel>(isolatedStorageModel: isolatedStorage);

                Assert.IsTrue(saveResult.IsSuccess);
                Assert.AreEqual(nameof(ScheduleFileInfoModel), saveResult.Token);
                Assert.AreEqual(nameof(ScheduleFileInfoModel), saveResult.ModelTypeName);
                Assert.IsTrue(saveResult.StorageFileExists);
                Assert.IsTrue(getResult.IsSuccess);
                Assert.IsNotNull(getResult.Model);
                Assert.AreEqual(model.SourceFileFullPath, getResult.Model.SourceFileFullPath);
                Assert.AreEqual(model.TargetFileFullPath, getResult.Model.TargetFileFullPath);
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
        public void GetModelWithResult_ShouldCaptureDeserializationException_WhenStoredJsonIsInvalid()
        {
            var tempDirectory = CreateTempDirectory();

            try
            {
                var isolatedStorage = new LanymyIsolatedStorage(tempDirectory);
                const string token = "invalid-json";
                IsolatedStorageHelper.SaveString("{invalid-json}", token, isolatedStorageString: isolatedStorage);

                var result = IsolatedStorageHelper.GetModelWithResult<ScheduleFileInfoModel>(token, isolatedStorageModel: isolatedStorage);

                Assert.IsFalse(result.IsSuccess);
                Assert.IsNotNull(result.Exception);
                Assert.AreEqual(result.Exception.Message, result.ErrorMessage);
                Assert.IsTrue(result.StorageFileExists);
                Assert.AreEqual("{invalid-json}", result.SerializedString);

                try
                {
                    IsolatedStorageHelper.GetModel<ScheduleFileInfoModel>(token, isolatedStorageModel: isolatedStorage);
                    Assert.Fail("Expected GetModel to rethrow deserialization exception.");
                }
                catch (Exception ex)
                {
                    Assert.AreSame(result.Exception.GetType(), ex.GetType());
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
        public void SaveModelWithResult_ShouldCaptureSerializationExceptionWithoutThrowing_WhenModelSerializationFails()
        {
            var tempDirectory = CreateTempDirectory();

            try
            {
                var isolatedStorage = new LanymyIsolatedStorage(tempDirectory);
                var model = new ThrowingSerializeModel();

                var result = IsolatedStorageHelper.SaveModelWithResult(model, isolatedStorageModel: isolatedStorage);

                Assert.IsFalse(result.IsSuccess);
                Assert.IsNotNull(result.Exception);
                Assert.AreEqual(result.Exception.Message, result.ErrorMessage);
                Assert.AreSame(model, result.Model);
                Assert.AreEqual(nameof(ThrowingSerializeModel), result.Token);
                Assert.AreEqual(nameof(ThrowingSerializeModel), result.ModelTypeName);
                Assert.AreEqual(typeof(ThrowingSerializeModel).FullName, result.ModelTypeFullName);
                Assert.IsFalse(result.StorageFileExists);
                Assert.IsNull(result.SerializedString);
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
