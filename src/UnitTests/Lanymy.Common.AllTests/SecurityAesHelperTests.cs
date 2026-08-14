using System;
using System.IO;
using System.Threading.Tasks;
using Lanymy.Common.Abstractions.Models;
using Lanymy.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{



    [TestClass()]
    public class SecurityAesHelperTests
    {



        [TestMethod()]
        public async Task SecurityAesHelper_StringAndFileApis_ShouldKeepCompatibilityAndSupportWithResult()
        {
            var securityKey = "securityKey Hello World";
            var iv = "0123456789";
            var sourceString = "good-morning";
            var rootPath = Path.Combine(Path.GetTempPath(), $"lanymy_aes_{Guid.NewGuid():N}");
            var targetFilePath = Path.Combine(rootPath, "model.aes");

            var encryptString = SecurityAesHelper.EncryptStringToString(sourceString, securityKey);
            var decryptString = SecurityAesHelper.DecryptStringFromString(encryptString, securityKey);
            Assert.AreEqual(sourceString, decryptString);


            encryptString = SecurityAesHelper.EncryptStringToString(sourceString, securityKey, iv);
            decryptString = SecurityAesHelper.DecryptStringFromString(encryptString, securityKey, iv);
            Assert.AreEqual(sourceString, decryptString);



            var scheduleFileInfoModel = new ScheduleFileInfoModel
            {
                SourceFileFullPath = DateTime.Now.ToString("O"),
                TargetFileFullPath = Guid.NewGuid().ToString("N"),
            };
            Directory.CreateDirectory(rootPath);

            try
            {
                var encryptFileResult = SecurityAesHelper.EncryptModelToFileWithResult(scheduleFileInfoModel, targetFilePath, securityKey, iv);
                var decryptFileResult = SecurityAesHelper.DecryptModelFromFileWithResult<ScheduleFileInfoModel>(targetFilePath, securityKey, iv);

                Assert.IsTrue(encryptFileResult.IsSuccess);
                Assert.IsNull(encryptFileResult.Exception);
                Assert.IsTrue(string.IsNullOrEmpty(encryptFileResult.ErrorMessage));
                Assert.IsTrue(encryptFileResult.IsEncryptOperation);
                Assert.IsTrue(encryptFileResult.TargetFileExists);
                Assert.AreEqual(targetFilePath, encryptFileResult.FilePath);
                Assert.AreSame(scheduleFileInfoModel, encryptFileResult.Model);
                Assert.AreEqual(nameof(ScheduleFileInfoModel), encryptFileResult.ModelTypeName);
                Assert.AreEqual(typeof(ScheduleFileInfoModel).FullName, encryptFileResult.ModelTypeFullName);

                Assert.IsTrue(decryptFileResult.IsSuccess);
                Assert.IsNull(decryptFileResult.Exception);
                Assert.IsTrue(string.IsNullOrEmpty(decryptFileResult.ErrorMessage));
                Assert.IsFalse(decryptFileResult.IsEncryptOperation);
                Assert.IsTrue(decryptFileResult.TargetFileExists);
                Assert.AreEqual(targetFilePath, decryptFileResult.FilePath);
                Assert.IsNotNull(decryptFileResult.Model);
                Assert.AreEqual(nameof(ScheduleFileInfoModel), decryptFileResult.ModelTypeName);
                Assert.AreEqual(typeof(ScheduleFileInfoModel).FullName, decryptFileResult.ModelTypeFullName);
                Assert.AreEqual(scheduleFileInfoModel.SourceFileFullPath, decryptFileResult.Model.SourceFileFullPath);
                Assert.AreEqual(scheduleFileInfoModel.TargetFileFullPath, decryptFileResult.Model.TargetFileFullPath);

                SecurityAesHelper.EncryptModelToFile(scheduleFileInfoModel, targetFilePath, securityKey, iv);
                var compatibilityModel = SecurityAesHelper.DecryptModelFromFile<ScheduleFileInfoModel>(targetFilePath, securityKey, iv);
                Assert.AreEqual(scheduleFileInfoModel.SourceFileFullPath, compatibilityModel.SourceFileFullPath);
                Assert.AreEqual(scheduleFileInfoModel.TargetFileFullPath, compatibilityModel.TargetFileFullPath);
            }
            finally
            {
                if (Directory.Exists(rootPath))
                {
                    Directory.Delete(rootPath, true);
                }
            }

            await Task.CompletedTask;
        }

        [TestMethod]
        public void SecurityAesHelper_DecryptModelFromFileWithResult_WhenFileMissing_ShouldCaptureException()
        {
            var targetFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_aes_missing_{Guid.NewGuid():N}.aes");

            var result = SecurityAesHelper.DecryptModelFromFileWithResult<ScheduleFileInfoModel>(targetFilePath, "securityKey Hello World", "0123456789");

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Exception);
            Assert.AreEqual(result.Exception.Message, result.ErrorMessage);
            Assert.IsInstanceOfType(result.Exception, typeof(FileNotFoundException));
            Assert.IsFalse(result.IsEncryptOperation);
            Assert.IsFalse(result.TargetFileExists);
            Assert.AreEqual(targetFilePath, result.FilePath);
            Assert.AreEqual(nameof(ScheduleFileInfoModel), result.ModelTypeName);
            Assert.AreEqual(typeof(ScheduleFileInfoModel).FullName, result.ModelTypeFullName);
            Assert.IsNull(result.Model);
        }

        [TestMethod]
        public void SecurityAesHelper_DecryptModelFromFileWithResult_WhenKeyIsWrong_ShouldCaptureErrorMessageAndModelType()
        {
            var securityKey = "securityKey Hello World";
            var iv = "0123456789";
            var wrongSecurityKey = "wrong-security-key";
            var rootPath = Path.Combine(Path.GetTempPath(), $"lanymy_aes_wrong_key_{Guid.NewGuid():N}");
            var targetFilePath = Path.Combine(rootPath, "model.aes");
            var scheduleFileInfoModel = new ScheduleFileInfoModel
            {
                SourceFileFullPath = "source.txt",
                TargetFileFullPath = "target.txt",
            };

            Directory.CreateDirectory(rootPath);

            try
            {
                SecurityAesHelper.EncryptModelToFile(scheduleFileInfoModel, targetFilePath, securityKey, iv);

                var result = SecurityAesHelper.DecryptModelFromFileWithResult<ScheduleFileInfoModel>(targetFilePath, wrongSecurityKey, iv);

                Assert.IsFalse(result.IsSuccess);
                Assert.IsNotNull(result.Exception);
                Assert.AreEqual(result.Exception.Message, result.ErrorMessage);
                Assert.IsTrue(result.TargetFileExists);
                Assert.AreEqual(targetFilePath, result.FilePath);
                Assert.AreEqual(nameof(ScheduleFileInfoModel), result.ModelTypeName);
                Assert.AreEqual(typeof(ScheduleFileInfoModel).FullName, result.ModelTypeFullName);
                Assert.IsNull(result.Model);
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
        public void SecurityAesHelper_DecryptModelFromFile_WhenFileMissing_ShouldKeepCompatibilityThrowSemantics()
        {
            var targetFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_aes_missing_{Guid.NewGuid():N}.aes");

            try
            {
                SecurityAesHelper.DecryptModelFromFile<ScheduleFileInfoModel>(targetFilePath, "securityKey Hello World", "0123456789");
                Assert.Fail("Expected FileNotFoundException was not thrown.");
            }
            catch (FileNotFoundException ex)
            {
                Assert.AreEqual(targetFilePath, ex.FileName);
            }
        }



    }



}
