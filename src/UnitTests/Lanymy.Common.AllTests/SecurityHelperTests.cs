using System;
using System.Drawing;
using System.IO;
using System.Linq;
#if NET8_0_OR_GREATER
using System.Runtime.Versioning;
#endif
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;
using Lanymy.Common.Abstractions.Models;
using Lanymy.Common.Enums;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.CryptoModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class SecurityHelperTests
    {
        [TestMethod]
        public void SecurityHelper_LoadCertificateWithResult_FromPasswordProtectedFile_ShouldLoadIntoCurrentUserStores()
        {
            const string password = "Lanymy.Test.Password.123!";
            const StoreName trustedStoreName = StoreName.AddressBook;
            const StoreName personalStoreName = StoreName.My;
            var certificateFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.pfx");
            using var certificate = CreateSelfSignedCertificate();
            var thumbprint = certificate.Thumbprint;

            try
            {
                File.WriteAllBytes(certificateFilePath, certificate.Export(X509ContentType.Pfx, password));

                var result = SecurityHelper.LoadCertificateWithResult(certificateFilePath, password, StoreLocation.CurrentUser, trustedStoreName, personalStoreName);

                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.Exception);
                Assert.AreEqual(certificateFilePath, result.FilePath);
                Assert.AreEqual(StoreLocation.CurrentUser, result.StoreLocation);
                Assert.AreEqual(trustedStoreName, result.TrustedStoreName);
                Assert.AreEqual(personalStoreName, result.PersonalStoreName);
                Assert.AreEqual(thumbprint, result.CertificateThumbprint);
                Assert.IsFalse(string.IsNullOrEmpty(result.CertificateSubject));
                Assert.IsTrue(result.ExistsInTrustedStore);
                Assert.IsTrue(result.ExistsInPersonalStore);
                Assert.IsTrue(StoreContainsCertificate(trustedStoreName, thumbprint));
                Assert.IsTrue(StoreContainsCertificate(personalStoreName, thumbprint));
            }
            finally
            {
                RemoveCertificateFromCurrentUserStores(thumbprint, trustedStoreName, personalStoreName);

                if (File.Exists(certificateFilePath))
                {
                    File.Delete(certificateFilePath);
                }
            }
        }

        [TestMethod]
        public void SecurityHelper_LoadCertificateWithResult_WhenFileMissing_ShouldCaptureException()
        {
            var certificateFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.pfx");

            var result = SecurityHelper.LoadCertificateWithResult(certificateFilePath, StoreLocation.CurrentUser);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(certificateFilePath, result.FilePath);
            Assert.AreEqual(StoreLocation.CurrentUser, result.StoreLocation);
            Assert.IsInstanceOfType(result.Exception, typeof(FileNotFoundException));
        }

        [TestMethod]
        public void SecurityHelper_LoadCertificateCompatibilityApis_WhenFileMissing_ShouldKeepLegacyThrowSemantics()
        {
            var certificateFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.pfx");

            try
            {
                SecurityHelper.LoadCertificate(certificateFilePath);
                Assert.Fail("Expected FileNotFoundException was not thrown.");
            }
            catch (FileNotFoundException ex)
            {
                Assert.IsTrue(ex.Message.Contains(certificateFilePath));
            }
        }

        [TestMethod]
        public void SecurityHelper_LoadCertificateWithResult_WhenRawDataInvalid_ShouldCaptureException()
        {
            var result = SecurityHelper.LoadCertificateWithResult((byte[])null, StoreLocation.CurrentUser);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(StoreLocation.CurrentUser, result.StoreLocation);
            Assert.IsInstanceOfType(result.Exception, typeof(ArgumentNullException));
        }

        [TestMethod]
        public void SecurityHelper_EncryptStringToFile_ShouldRoundTripAndExposeDigestInfo()
        {
            const string sourceString = "lanymy-security-file-roundtrip";
            var encryptedFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.lanymy");

            try
            {
                var encryptResult = SecurityHelper.EncryptStringToFile(sourceString, encryptedFilePath);
                var decryptResult = SecurityHelper.DecryptStringFromFile(encryptedFilePath);

                Assert.IsTrue(encryptResult.IsSuccess);
                Assert.AreEqual(encryptedFilePath, encryptResult.EncryptedFileFullPath);
                Assert.AreEqual(sourceString, encryptResult.SourceString);
                Assert.IsFalse(string.IsNullOrEmpty(encryptResult.EncryptBytesHashCode));
                Assert.IsFalse(string.IsNullOrEmpty(encryptResult.EncryptContentBytesHashCode));

                Assert.IsTrue(decryptResult.IsSuccess);
                Assert.AreEqual(encryptedFilePath, decryptResult.EncryptedFileFullPath);
                Assert.AreEqual(sourceString, decryptResult.SourceString);
            }
            finally
            {
                if (File.Exists(encryptedFilePath))
                {
                    File.Delete(encryptedFilePath);
                }
            }
        }

        [TestMethod]
        public void SecurityHelper_EncryptStringToFile_WhenTargetDirectoryMissing_ShouldCreateParentDirectoryAndRoundTrip()
        {
            const string sourceString = "lanymy-security-create-parent-directory";
            var rootPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}");
            var encryptedFilePath = Path.Combine(rootPath, "nested", "payload.lanymy");

            try
            {
                var encryptResult = SecurityHelper.EncryptStringToFile(sourceString, encryptedFilePath);
                var decryptResult = SecurityHelper.DecryptStringFromFile(encryptedFilePath);

                Assert.IsTrue(encryptResult.IsSuccess);
                Assert.AreEqual(encryptedFilePath, encryptResult.EncryptedFileFullPath);
                Assert.AreEqual(sourceString, encryptResult.SourceString);
                Assert.IsTrue(File.Exists(encryptedFilePath));

                Assert.IsTrue(decryptResult.IsSuccess);
                Assert.AreEqual(sourceString, decryptResult.SourceString);
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
        public void SecurityHelper_EncryptStreamToStream_ShouldRoundTripAndExposeDigestInfo()
        {
            var sourceBytes = Enumerable.Range(1, 16).Select(i => (byte)i).ToArray();

            using var sourceStream = new MemoryStream(sourceBytes);
            using var encryptedStream = new MemoryStream();

            var encryptResult = SecurityHelper.EncryptStreamToStream(sourceStream, encryptedStream, "stream-secret");

            encryptedStream.Position = 0;
            using var decryptedStream = new MemoryStream();
            var decryptResult = SecurityHelper.DencryptStreamFromStream(encryptedStream, decryptedStream, "stream-secret");

            Assert.IsTrue(encryptResult.IsSuccess);
            Assert.AreEqual(sourceBytes.Length, encryptResult.SourceBytesSize);
            Assert.IsFalse(string.IsNullOrEmpty(encryptResult.SourceBytesHashCode));
            Assert.IsFalse(string.IsNullOrEmpty(encryptResult.EncryptBytesHashCode));

            Assert.IsTrue(decryptResult.IsSuccess);
            CollectionAssert.AreEqual(sourceBytes, decryptedStream.ToArray());
            Assert.AreEqual(encryptResult.SourceBytesSize, decryptResult.SourceBytesSize);
            Assert.AreEqual(encryptResult.SourceBytesHashCode, decryptResult.SourceBytesHashCode);
        }

        [TestMethod]
        public void SecurityHelper_EncryptStreamToStream_ShouldKeepDigestJsonConsistentWithRuntimeFields()
        {
            using var sourceStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("lanymy-stream-digest-json"));
            using var encryptedStream = new MemoryStream();

            var encryptResult = SecurityHelper.EncryptStreamToStream(sourceStream, encryptedStream);

            encryptedStream.Position = 0;
            var digestResult = SecurityHelper.GetEncryptDigestInfoModelFromEncryptedStream(encryptedStream);
            var digestJson = JObject.Parse(digestResult.DencryptHeaderInfoModelJsonString);

            Assert.IsTrue(encryptResult.IsSuccess);
            Assert.IsTrue(digestResult.IsSuccess);
            Assert.AreEqual(encryptResult.SourceBytesSize, digestResult.SourceBytesSize);
            Assert.AreEqual(encryptResult.SourceBytesHashCode, digestResult.SourceBytesHashCode);
            Assert.AreEqual(encryptResult.EncryptBytesSize, digestResult.EncryptBytesSize);
            Assert.AreEqual(encryptResult.EncryptBytesHashCode, digestResult.EncryptBytesHashCode);
            Assert.AreEqual(encryptResult.EncryptContentBytesSize, digestResult.EncryptContentBytesSize);
            Assert.AreEqual(encryptResult.EncryptContentBytesHashCode, digestResult.EncryptContentBytesHashCode);
            Assert.AreEqual(digestResult.EncryptBytesSize, digestJson.Value<long>("EncryptBytesSize"));
            Assert.AreEqual(digestResult.EncryptBytesHashCode, digestJson.Value<string>("EncryptBytesHashCode"));
            Assert.AreEqual(digestResult.EncryptContentBytesSize, digestJson.Value<long>("EncryptContentBytesSize"));
            Assert.AreEqual(digestResult.EncryptContentBytesHashCode, digestJson.Value<string>("EncryptContentBytesHashCode"));
        }

        [TestMethod]
        public void SecurityHelper_DencryptStreamFromStream_WhenSecretKeyInvalid_ShouldReturnFailedDigestAndClearSeekableStream()
        {
            using var sourceStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("lanymy-stream-wrong-key"));
            using var encryptedStream = new MemoryStream();

            var encryptResult = SecurityHelper.EncryptStreamToStream(sourceStream, encryptedStream, "correct-stream-key");

            encryptedStream.Position = 0;
            using var decryptedStream = new MemoryStream();
            var decryptResult = SecurityHelper.DencryptStreamFromStream(encryptedStream, decryptedStream, "wrong-stream-key");

            Assert.IsTrue(encryptResult.IsSuccess);
            Assert.IsFalse(decryptResult.IsSuccess);
            Assert.AreEqual("解密失败,密钥或加密内容无效", decryptResult.ErrorMessage);
            Assert.AreEqual(0, decryptedStream.Length);
        }

        [TestMethod]
        public void SecurityHelper_GetEncryptDigestInfoModelFromEncryptedStream_WhenStreamEmpty_ShouldReturnFailedDigest()
        {
            using var encryptedStream = new MemoryStream();

            var result = SecurityHelper.GetEncryptDigestInfoModelFromEncryptedStream(encryptedStream);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("加密信息结构无效,无法继续解析", result.ErrorMessage);
        }

        [TestMethod]
        public void SecurityHelper_DencryptStreamFromStream_WhenStreamEmpty_ShouldReturnFailedDigestAndKeepTargetEmpty()
        {
            using var encryptedStream = new MemoryStream();
            using var targetStream = new MemoryStream();

            var result = SecurityHelper.DencryptStreamFromStream(encryptedStream, targetStream);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("加密信息结构无效,无法继续解析", result.ErrorMessage);
            Assert.AreEqual(0, targetStream.Length);
        }

        [TestMethod]
        public void SecurityHelper_GetEncryptDigestInfoModelFromEncryptedFile_ShouldExposeEncryptedFilePath()
        {
            var encryptedFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.lanymy");

            try
            {
                var encryptResult = SecurityHelper.EncryptStringToFile("lanymy-file-digest-path", encryptedFilePath);
                var digestResult = new LanymyCrypto().GetEncryptDigestInfoModelFromEncryptedFile<EncryptStringFileDigestInfoModel>(encryptedFilePath);

                Assert.IsTrue(encryptResult.IsSuccess);
                Assert.IsTrue(digestResult.IsSuccess);
                Assert.AreEqual(encryptedFilePath, digestResult.EncryptedFileFullPath);
            }
            finally
            {
                if (File.Exists(encryptedFilePath))
                {
                    File.Delete(encryptedFilePath);
                }
            }
        }

        [TestMethod]
        public void SecurityHelper_GetEncryptDigestInfoModelFromEncryptedFile_DefaultDigestShouldNotExposeFileProperty()
        {
            var encryptedFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.lanymy");

            try
            {
                var encryptResult = SecurityHelper.EncryptStringToFile("lanymy-file-digest-boundary", encryptedFilePath);
                var digestResult = SecurityHelper.GetEncryptDigestInfoModelFromEncryptedFile(encryptedFilePath);

                Assert.IsTrue(encryptResult.IsSuccess);
                Assert.IsTrue(digestResult.IsSuccess);
                Assert.IsNull(digestResult.GetType().GetProperty("EncryptedFileFullPath"));
            }
            finally
            {
                if (File.Exists(encryptedFilePath))
                {
                    File.Delete(encryptedFilePath);
                }
            }
        }

        [TestMethod]
        public void SecurityHelper_EncryptBytesToFile_ShouldRoundTripBytesAndExposeSourceBytes()
        {
            var sourceBytes = new byte[] { 1, 2, 3, 4, 5, 6 };
            var encryptedFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.bin");

            try
            {
                var encryptResult = SecurityHelper.EncryptBytesToFile(sourceBytes, encryptedFilePath);
                var decryptResult = SecurityHelper.DecryptBytesFromFile(encryptedFilePath);

                Assert.IsTrue(encryptResult.IsSuccess);
                CollectionAssert.AreEqual(sourceBytes, encryptResult.SourceBytes);
                Assert.AreEqual(encryptedFilePath, encryptResult.EncryptedFileFullPath);

                Assert.IsTrue(decryptResult.IsSuccess);
                CollectionAssert.AreEqual(sourceBytes, decryptResult.SourceBytes);
                Assert.AreEqual(encryptedFilePath, decryptResult.EncryptedFileFullPath);
            }
            finally
            {
                if (File.Exists(encryptedFilePath))
                {
                    File.Delete(encryptedFilePath);
                }
            }
        }

        [TestMethod]
        public void SecurityHelper_EncryptFileToFile_ShouldRoundTripFileContentAndExposePaths()
        {
            const string sourceContent = "lanymy-file-to-file-roundtrip";
            var sourceFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.txt");
            var encryptedFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.enc");
            var decryptedFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.txt");

            try
            {
                File.WriteAllText(sourceFilePath, sourceContent);

                var encryptResult = SecurityHelper.EncryptFileToFile(sourceFilePath, encryptedFilePath);
                var decryptResult = SecurityHelper.DecryptFileFromFile(encryptedFilePath, decryptedFilePath);

                Assert.IsTrue(encryptResult.IsSuccess);
                Assert.AreEqual(sourceFilePath, encryptResult.SourceFileFullPath);
                Assert.AreEqual(encryptedFilePath, encryptResult.EncryptedFileFullPath);

                Assert.IsTrue(decryptResult.IsSuccess);
                Assert.AreEqual(decryptedFilePath, decryptResult.SourceFileFullPath);
                Assert.AreEqual(encryptedFilePath, decryptResult.EncryptedFileFullPath);
                Assert.AreEqual(sourceContent, File.ReadAllText(decryptedFilePath));
            }
            finally
            {
                if (File.Exists(sourceFilePath))
                {
                    File.Delete(sourceFilePath);
                }

                if (File.Exists(encryptedFilePath))
                {
                    File.Delete(encryptedFilePath);
                }

                if (File.Exists(decryptedFilePath))
                {
                    File.Delete(decryptedFilePath);
                }
            }
        }

        [TestMethod]
        public void SecurityHelper_EncryptAndDecryptFileToFile_WhenTargetDirectoriesMissing_ShouldCreateParentDirectories()
        {
            const string sourceContent = "lanymy-file-to-file-create-parent-directory";
            var rootPath = Path.Combine(Path.GetTempPath(), $"lanymy-security-file-create-dir_{Guid.NewGuid():N}");
            var sourceFilePath = Path.Combine(rootPath, "source.txt");
            var encryptedFilePath = Path.Combine(rootPath, "encrypt", "payload.enc");
            var decryptedFilePath = Path.Combine(rootPath, "decrypt", "source.txt");

            Directory.CreateDirectory(rootPath);

            try
            {
                File.WriteAllText(sourceFilePath, sourceContent);

                var encryptResult = SecurityHelper.EncryptFileToFile(sourceFilePath, encryptedFilePath);
                var decryptResult = SecurityHelper.DecryptFileFromFile(encryptedFilePath, decryptedFilePath);

                Assert.IsTrue(encryptResult.IsSuccess);
                Assert.AreEqual(sourceFilePath, encryptResult.SourceFileFullPath);
                Assert.AreEqual(encryptedFilePath, encryptResult.EncryptedFileFullPath);
                Assert.IsTrue(File.Exists(encryptedFilePath));

                Assert.IsTrue(decryptResult.IsSuccess);
                Assert.AreEqual(decryptedFilePath, decryptResult.SourceFileFullPath);
                Assert.AreEqual(encryptedFilePath, decryptResult.EncryptedFileFullPath);
                Assert.IsTrue(File.Exists(decryptedFilePath));
                Assert.AreEqual(sourceContent, File.ReadAllText(decryptedFilePath));
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
        public void SecurityHelper_EncryptModelToFile_ShouldRoundTripAndExposeMetadata()
        {
            var sourceModel = new ScheduleFileInfoModel
            {
                SourceFileFullPath = @"C:\temp\source-model-file.txt",
                TargetFileFullPath = @"C:\temp\target-model-file.txt",
            };
            var encryptedFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.model");

            try
            {
                var encryptResult = SecurityHelper.EncryptModelToFile(sourceModel, encryptedFilePath);
                var decryptResult = SecurityHelper.DecryptModelFromFile<ScheduleFileInfoModel>(encryptedFilePath);

                Assert.IsTrue(encryptResult.IsSuccess);
                Assert.AreEqual(typeof(ScheduleFileInfoModel).Name, encryptResult.ModelTypeName);
                Assert.AreEqual(typeof(ScheduleFileInfoModel).FullName, encryptResult.ModelTypeFullName);
                Assert.IsNotNull(encryptResult.SourceModel);
                Assert.AreEqual(encryptedFilePath, encryptResult.EncryptedFileFullPath);

                Assert.IsTrue(decryptResult.IsSuccess);
                Assert.AreEqual(typeof(ScheduleFileInfoModel).Name, decryptResult.ModelTypeName);
                Assert.AreEqual(typeof(ScheduleFileInfoModel).FullName, decryptResult.ModelTypeFullName);
                Assert.AreEqual(encryptedFilePath, decryptResult.EncryptedFileFullPath);
                Assert.IsNotNull(decryptResult.SourceModel);
                Assert.AreEqual(sourceModel.SourceFileFullPath, decryptResult.SourceModel.SourceFileFullPath);
                Assert.AreEqual(sourceModel.TargetFileFullPath, decryptResult.SourceModel.TargetFileFullPath);
            }
            finally
            {
                if (File.Exists(encryptedFilePath))
                {
                    File.Delete(encryptedFilePath);
                }
            }
        }

        [TestMethod]
        public void SecurityHelper_EncryptModelToBase64String_ShouldRoundTripAndExposeMetadata()
        {
            var sourceModel = new ScheduleFileInfoModel
            {
                SourceFileFullPath = @"C:\temp\source-model-base64.txt",
                TargetFileFullPath = @"C:\temp\target-model-base64.txt",
            };

            var encryptResult = SecurityHelper.EncryptModelToBase64String(sourceModel);
            var decryptResult = SecurityHelper.DecryptModelFromBase64String<ScheduleFileInfoModel>(encryptResult.EncryptedBase64String);

            Assert.IsTrue(encryptResult.IsSuccess);
            Assert.AreEqual(typeof(ScheduleFileInfoModel).Name, encryptResult.ModelTypeName);
            Assert.AreEqual(typeof(ScheduleFileInfoModel).FullName, encryptResult.ModelTypeFullName);
            Assert.IsNotNull(encryptResult.SourceModel);
            Assert.IsFalse(string.IsNullOrEmpty(encryptResult.EncryptedBase64String));

            Assert.IsTrue(decryptResult.IsSuccess);
            Assert.AreEqual(typeof(ScheduleFileInfoModel).Name, decryptResult.ModelTypeName);
            Assert.AreEqual(typeof(ScheduleFileInfoModel).FullName, decryptResult.ModelTypeFullName);
            Assert.AreEqual(encryptResult.EncryptedBase64String, decryptResult.EncryptedBase64String);
            Assert.IsNotNull(decryptResult.SourceModel);
            Assert.AreEqual(sourceModel.SourceFileFullPath, decryptResult.SourceModel.SourceFileFullPath);
            Assert.AreEqual(sourceModel.TargetFileFullPath, decryptResult.SourceModel.TargetFileFullPath);
        }

        [TestMethod]
        public void SecurityHelper_GetEncryptDigestInfoModelFromEncryptedStream_WhenContentTampered_ShouldReturnFailedDigest()
        {
            var encryptResult = SecurityHelper.EncryptStringToBytes("lanymy-tampered-content");
            var tamperedBytes = encryptResult.EncryptedBytes.ToArray();
            tamperedBytes[^1] ^= 0x01;

            using var encryptedStream = new MemoryStream(tamperedBytes);
            var result = SecurityHelper.GetEncryptDigestInfoModelFromEncryptedStream(encryptedStream);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("加密信息效验失败,无法继续解析", result.ErrorMessage);
        }

        [TestMethod]
        public void SecurityHelper_GetEncryptDigestInfoModelFromEncryptedStream_WhenStreamTruncated_ShouldReturnFailedDigest()
        {
            var encryptResult = SecurityHelper.EncryptStringToBytes("lanymy-truncated-content");
            var truncatedBytes = encryptResult.EncryptedBytes.Take(encryptResult.EncryptedBytes.Length - 3).ToArray();

            using var encryptedStream = new MemoryStream(truncatedBytes);
            var result = SecurityHelper.GetEncryptDigestInfoModelFromEncryptedStream(encryptedStream);

            Assert.IsFalse(result.IsSuccess);
            CollectionAssert.Contains(
                new[]
                {
                    "加密信息效验失败,无法继续解析",
                    "加密信息结构无效,无法继续解析",
                },
                result.ErrorMessage);
        }

        [TestMethod]
        public void SecurityHelper_GetEncryptDigestInfoModelFromEncryptedFile_WhenContentTampered_ShouldMatchStreamFailureSemantics()
        {
            var encryptedFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.lanymy");

            try
            {
                var encryptResult = SecurityHelper.EncryptStringToFile("lanymy-file-digest-check", encryptedFilePath);
                var tamperedBytes = File.ReadAllBytes(encryptedFilePath);
                tamperedBytes[^1] ^= 0x01;
                File.WriteAllBytes(encryptedFilePath, tamperedBytes);

                var result = SecurityHelper.GetEncryptDigestInfoModelFromEncryptedFile(encryptedFilePath);
                var typedResult = new LanymyCrypto().GetEncryptDigestInfoModelFromEncryptedFile<EncryptStringFileDigestInfoModel>(encryptedFilePath);

                Assert.IsTrue(encryptResult.IsSuccess);
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("加密信息效验失败,无法继续解析", result.ErrorMessage);
                Assert.IsFalse(typedResult.IsSuccess);
                Assert.AreEqual(encryptedFilePath, typedResult.EncryptedFileFullPath);
            }
            finally
            {
                if (File.Exists(encryptedFilePath))
                {
                    File.Delete(encryptedFilePath);
                }
            }
        }

        [TestMethod]
        public void SecurityHelper_DecryptStringFromBytes_WhenSecretKeyInvalid_ShouldReturnFailedDigest()
        {
            var encryptResult = SecurityHelper.EncryptStringToBytes("lanymy-wrong-key-check", "correct-key");
            var decryptResult = SecurityHelper.DecryptStringFromBytes(encryptResult.EncryptedBytes, "wrong-key");

            Assert.IsTrue(encryptResult.IsSuccess);
            Assert.IsFalse(decryptResult.IsSuccess);
            Assert.AreEqual("解密失败,密钥或加密内容无效", decryptResult.ErrorMessage);
            Assert.IsNull(decryptResult.SourceString);
        }

        [TestMethod]
        public void SecurityHelper_DecryptStringFromBase64String_WhenInputInvalid_ShouldReturnFailedDigest()
        {
            var decryptResult = SecurityHelper.DecryptStringFromBase64String("not-base64@@@");

            Assert.IsFalse(decryptResult.IsSuccess);
            Assert.AreEqual("加密内容不是有效的Base64字符串", decryptResult.ErrorMessage);
            Assert.AreEqual("not-base64@@@", decryptResult.EncryptedBase64String);
            Assert.IsNull(decryptResult.SourceString);
        }

        [TestMethod]
        public void SecurityHelper_DecryptModelFromBase64String_WhenJsonInvalid_ShouldReturnFailedDigest()
        {
            var encryptResult = SecurityHelper.EncryptStringToBase64String("not-json-model");
            var decryptResult = SecurityHelper.DecryptModelFromBase64String<ScheduleFileInfoModel>(encryptResult.EncryptedBase64String);

            Assert.IsTrue(encryptResult.IsSuccess);
            Assert.IsFalse(decryptResult.IsSuccess);
            Assert.AreEqual("模型反序列化失败,无法继续解析", decryptResult.ErrorMessage);
            Assert.AreEqual(encryptResult.EncryptedBase64String, decryptResult.EncryptedBase64String);
            Assert.IsNull(decryptResult.SourceModel);
        }

        [TestMethod]
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public void SecurityHelper_EncryptStringToImageFile_WhenTargetDirectoryMissing_ShouldReturnFailedDigest()
        {
            var imageFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "security-helper-test.png");

            var result = SecurityHelper.EncryptStringToImageFile("lanymy-image-failure", imageFilePath);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Failed to save encrypted bitmap to image file.", result.ErrorMessage);
            Assert.IsNull(result.EncryptedFileFullPath);
            Assert.IsFalse(File.Exists(imageFilePath));
        }

        [TestMethod]
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public void SecurityHelper_DecryptStringFromBitmap_WhenBitmapInvalid_ShouldReturnFailedDigest()
        {
            using var bitmap = new Bitmap(2, 3);

            var result = SecurityHelper.DecryptStringFromBitmap(bitmap);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("不是有效的加密位图数据源", result.ErrorMessage);
            Assert.IsNull(result.SourceString);
        }

        [TestMethod]
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public void SecurityHelper_DecryptStringFromImageFile_WhenImageContentInvalid_ShouldReturnFailedDigest()
        {
            var imageFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.png");

            try
            {
                File.WriteAllText(imageFilePath, "not-a-real-png");

                var result = SecurityHelper.DecryptStringFromImageFile(imageFilePath);

                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("图片文件内容无效,无法继续解析", result.ErrorMessage);
                Assert.AreEqual(imageFilePath, result.EncryptedFileFullPath);
                Assert.IsNull(result.SourceString);
            }
            finally
            {
                if (File.Exists(imageFilePath))
                {
                    File.Delete(imageFilePath);
                }
            }
        }

        [TestMethod]
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public void SecurityHelper_DecryptStringFromBitmap_ShouldUseDefaultEncodingWhenEncodingNull()
        {
            var encryptResult = SecurityHelper.EncryptStringToBitmap("lanymy-bitmap-roundtrip");

            try
            {
                Assert.IsTrue(encryptResult.IsSuccess);

                var decryptResult = SecurityHelper.DecryptStringFromBitmap(encryptResult.EncryptedBitmap);

                Assert.IsTrue(decryptResult.IsSuccess);
                Assert.AreEqual("lanymy-bitmap-roundtrip", decryptResult.SourceString);
                Assert.IsNotNull(decryptResult.SourceBytes);
            }
            finally
            {
                if (encryptResult.EncryptedBitmap != null)
                {
                    encryptResult.EncryptedBitmap.Dispose();
                }
            }
        }

        [TestMethod]
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public void SecurityHelper_EncryptModelToBitmap_ShouldRoundTripAndExposeMetadata()
        {
            var sourceModel = new ScheduleFileInfoModel
            {
                SourceFileFullPath = @"C:\temp\source-bitmap.txt",
                TargetFileFullPath = @"C:\temp\target-bitmap.txt",
            };
            var encryptResult = SecurityHelper.EncryptModelToBitmap(sourceModel);

            try
            {
                var decryptResult = SecurityHelper.DecryptModelFromBitmap<ScheduleFileInfoModel>(encryptResult.EncryptedBitmap);

                Assert.IsTrue(encryptResult.IsSuccess);
                Assert.AreEqual(typeof(ScheduleFileInfoModel).Name, encryptResult.ModelTypeName);
                Assert.AreEqual(typeof(ScheduleFileInfoModel).FullName, encryptResult.ModelTypeFullName);
                Assert.IsNotNull(encryptResult.SourceModel);

                Assert.IsTrue(decryptResult.IsSuccess);
                Assert.AreEqual(typeof(ScheduleFileInfoModel).Name, decryptResult.ModelTypeName);
                Assert.AreEqual(typeof(ScheduleFileInfoModel).FullName, decryptResult.ModelTypeFullName);
                Assert.IsNotNull(decryptResult.SourceModel);
                Assert.AreEqual(sourceModel.SourceFileFullPath, decryptResult.SourceModel.SourceFileFullPath);
                Assert.AreEqual(sourceModel.TargetFileFullPath, decryptResult.SourceModel.TargetFileFullPath);
            }
            finally
            {
                if (encryptResult.EncryptedBitmap != null)
                {
                    encryptResult.EncryptedBitmap.Dispose();
                }
            }
        }

        [TestMethod]
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public void SecurityHelper_EncryptModelToImageFile_ShouldRoundTripAndExposeDigestInfo()
        {
            var sourceModel = new ScheduleFileInfoModel
            {
                SourceFileFullPath = @"C:\temp\source.txt",
                TargetFileFullPath = @"C:\temp\target.txt",
            };
            var imageFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.png");

            try
            {
                var encryptResult = SecurityHelper.EncryptModelToImageFile(sourceModel, imageFilePath);
                var decryptResult = SecurityHelper.DecryptModelFromImageFile<ScheduleFileInfoModel>(imageFilePath);

                Assert.IsTrue(encryptResult.IsSuccess);
                Assert.AreEqual(imageFilePath, encryptResult.EncryptedFileFullPath);
                Assert.AreEqual(typeof(ScheduleFileInfoModel).Name, encryptResult.ModelTypeName);
                Assert.AreEqual(typeof(ScheduleFileInfoModel).FullName, encryptResult.ModelTypeFullName);
                Assert.IsNotNull(encryptResult.SourceModel);

                Assert.IsTrue(decryptResult.IsSuccess);
                Assert.AreEqual(imageFilePath, decryptResult.EncryptedFileFullPath);
                Assert.AreEqual(typeof(ScheduleFileInfoModel).Name, decryptResult.ModelTypeName);
                Assert.AreEqual(typeof(ScheduleFileInfoModel).FullName, decryptResult.ModelTypeFullName);
                Assert.IsNotNull(decryptResult.SourceModel);
                Assert.AreEqual(sourceModel.SourceFileFullPath, decryptResult.SourceModel.SourceFileFullPath);
                Assert.AreEqual(sourceModel.TargetFileFullPath, decryptResult.SourceModel.TargetFileFullPath);
            }
            finally
            {
                if (File.Exists(imageFilePath))
                {
                    File.Delete(imageFilePath);
                }
            }
        }

        [TestMethod]
        public void SecurityHelper_CreateRsaKeyBlobWithResult_ShouldReturnKeyPairs()
        {
            var bytesResult = SecurityHelper.CreateRsaKeyBlobBytesWithResult(RsaKeySizeTypeEnum.V1024);
            var stringResult = SecurityHelper.CreateRsaKeyBlobBase64StringWithResult(RsaKeySizeTypeEnum.V1024);

            Assert.IsTrue(bytesResult.IsSuccess);
            Assert.IsNull(bytesResult.Exception);
            Assert.AreEqual(RsaKeySizeTypeEnum.V1024, bytesResult.KeySizeType);
            Assert.IsNotNull(bytesResult.PublicKeyBlobBytes);
            Assert.IsNotNull(bytesResult.PrivateKeyBlobBytes);

            Assert.IsTrue(stringResult.IsSuccess);
            Assert.IsNull(stringResult.Exception);
            Assert.AreEqual(RsaKeySizeTypeEnum.V1024, stringResult.KeySizeType);
            Assert.IsFalse(string.IsNullOrEmpty(stringResult.PublicKeyBlobBase64String));
            Assert.IsFalse(string.IsNullOrEmpty(stringResult.PrivateKeyBlobBase64String));
        }

        [TestMethod]
        public void SecurityHelper_CreateRsaKeyBlobWithResult_WhenKeySizeInvalid_ShouldCaptureExceptionAndKeepLegacyThrowSemantics()
        {
            var invalidKeySizeType = (RsaKeySizeTypeEnum)123;
            var result = SecurityHelper.CreateRsaKeyBlobBytesWithResult(invalidKeySizeType);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(invalidKeySizeType, result.KeySizeType);
            Assert.IsNotNull(result.Exception);
            Assert.IsNull(result.PublicKeyBlobBytes);
            Assert.IsNull(result.PrivateKeyBlobBytes);

            try
            {
                SecurityHelper.CreateRsaKeyBlobBytes(invalidKeySizeType, out _, out _);
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
            }
        }

        [TestMethod]
        public void SecurityHelper_RsaStringWithResult_ShouldRoundTripString()
        {
            const string sourceString = "lanymy-rsa-roundtrip";

            var keyResult = SecurityHelper.CreateRsaKeyBlobBase64StringWithResult(RsaKeySizeTypeEnum.V1024);
            Assert.IsTrue(keyResult.IsSuccess);

            var encryptResult = SecurityHelper.RsaEncryptStringToBase64StringWithResult(keyResult.PublicKeyBlobBase64String, RsaKeySizeTypeEnum.V1024, sourceString);
            var decryptResult = SecurityHelper.RsaDecryptStringFromBase64StringWithResult(keyResult.PrivateKeyBlobBase64String, RsaKeySizeTypeEnum.V1024, encryptResult.ResultString);

            Assert.IsTrue(encryptResult.IsSuccess);
            Assert.IsNull(encryptResult.Exception);
            Assert.IsTrue(encryptResult.IsEncryptOperation);
            Assert.AreEqual(RsaKeySizeTypeEnum.V1024, encryptResult.KeySizeType);
            Assert.IsFalse(string.IsNullOrEmpty(encryptResult.ResultString));

            Assert.IsTrue(decryptResult.IsSuccess);
            Assert.IsNull(decryptResult.Exception);
            Assert.IsFalse(decryptResult.IsEncryptOperation);
            Assert.AreEqual(RsaKeySizeTypeEnum.V1024, decryptResult.KeySizeType);
            Assert.AreEqual(sourceString, decryptResult.ResultString);
        }

        [TestMethod]
        public void SecurityHelper_RsaBytesWithResult_WhenKeyDoesNotMatch_ShouldCaptureException()
        {
            var sourceBytes = new byte[] { 1, 2, 3, 4, 5 };

            var sourceKeyResult = SecurityHelper.CreateRsaKeyBlobBytesWithResult(RsaKeySizeTypeEnum.V1024);
            var anotherKeyResult = SecurityHelper.CreateRsaKeyBlobBytesWithResult(RsaKeySizeTypeEnum.V1024);
            Assert.IsTrue(sourceKeyResult.IsSuccess);
            Assert.IsTrue(anotherKeyResult.IsSuccess);

            var encryptResult = SecurityHelper.RsaEncryptBytesToBytesWithResult(sourceKeyResult.PublicKeyBlobBytes, RsaKeySizeTypeEnum.V1024, sourceBytes);
            var decryptResult = SecurityHelper.RsaDecryptBytesFromBytesWithResult(anotherKeyResult.PrivateKeyBlobBytes, RsaKeySizeTypeEnum.V1024, encryptResult.ResultBytes);

            Assert.IsTrue(encryptResult.IsSuccess);
            Assert.IsNull(encryptResult.Exception);
            Assert.IsNotNull(encryptResult.ResultBytes);

            Assert.IsFalse(decryptResult.IsSuccess);
            Assert.IsNotNull(decryptResult.Exception);
            Assert.IsNull(decryptResult.ResultBytes);
        }

        [TestMethod]
        public void SecurityHelper_RsaCompatibilityApis_WhenInputInvalid_ShouldKeepLegacyNullOrEmptySemantics()
        {
            var encryptBytesResult = SecurityHelper.RsaEncryptBytesToBytes(new byte[] { 1, 2, 3 }, RsaKeySizeTypeEnum.V1024, new byte[] { 4, 5, 6 });
            var decryptStringResult = SecurityHelper.RsaDecryptStringFromBase64String("invalid_base64", RsaKeySizeTypeEnum.V1024, "invalid_base64");

            Assert.IsNull(encryptBytesResult);
            Assert.AreEqual(string.Empty, decryptStringResult);
        }

        [TestMethod]
        public void SecurityHelper_RsaStringWithResult_WhenInputInvalid_ShouldCaptureException()
        {
            var result = SecurityHelper.RsaEncryptStringToBase64StringWithResult("invalid_base64", RsaKeySizeTypeEnum.V1024, "lanymy");

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Exception);
            Assert.IsTrue(result.IsEncryptOperation);
            Assert.AreEqual(string.Empty, SecurityHelper.RsaEncryptStringToBase64String("invalid_base64", RsaKeySizeTypeEnum.V1024, "lanymy"));
        }

        private static X509Certificate2 CreateSelfSignedCertificate()
        {
            using var rsa = RSA.Create(2048);
            var request = new CertificateRequest("CN=Lanymy.SecurityHelperTests", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(7));
        }

        private static bool StoreContainsCertificate(StoreName storeName, string thumbprint)
        {
            if (string.IsNullOrEmpty(thumbprint))
            {
                return false;
            }

            using var store = new X509Store(storeName, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            return store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false).Count > 0;
        }

        private static void RemoveCertificateFromCurrentUserStores(string thumbprint, StoreName trustedStoreName, StoreName personalStoreName)
        {
            if (string.IsNullOrEmpty(thumbprint))
            {
                return;
            }

            RemoveCertificateFromStore(trustedStoreName, thumbprint);
            if (personalStoreName != trustedStoreName)
            {
                RemoveCertificateFromStore(personalStoreName, thumbprint);
            }
        }

        private static void RemoveCertificateFromStore(StoreName storeName, string thumbprint)
        {
            using var store = new X509Store(storeName, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
            foreach (var certificate in certificates)
            {
                store.Remove(certificate);
            }
        }
    }
}
