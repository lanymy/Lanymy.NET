using System;
using System.Collections.Generic;
using System.IO;
using Lanymy.Common.Abstractions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class FileHelperTests
    {
        private sealed class ChunkedReadStream : MemoryStream
        {
            private readonly int _maxChunkSize;

            public ChunkedReadStream(byte[] buffer, int maxChunkSize)
                : base(buffer)
            {
                _maxChunkSize = maxChunkSize;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return base.Read(buffer, offset, Math.Min(count, _maxChunkSize));
            }
        }

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
        public void FileHelper_GetBinaryFileBytesWithResult_WhenFileExists_ShouldCaptureFilePathAndLength()
        {
            var tempFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_filehelper_result_{Guid.NewGuid():N}.bin");
            var sourceBytes = new byte[] { 1, 2, 3, 4, 5, 6 };

            try
            {
                File.WriteAllBytes(tempFilePath, sourceBytes);

                var result = Lanymy.Common.Helpers.FileHelper.GetBinaryFileBytesWithResult(tempFilePath);

                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.Exception);
                Assert.AreEqual(tempFilePath, result.FilePath);
                Assert.AreEqual(sourceBytes.Length, result.BytesLength);
                CollectionAssert.AreEqual(sourceBytes, result.Bytes);
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
        public void FileHelper_GetBinaryFileBytesWithResult_WhenFileMissing_ShouldCaptureException()
        {
            var missingFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_filehelper_missing_{Guid.NewGuid():N}.bin");

            var result = Lanymy.Common.Helpers.FileHelper.GetBinaryFileBytesWithResult(missingFilePath);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(missingFilePath, result.FilePath);
            Assert.IsInstanceOfType(result.Exception, typeof(FileNotFoundException));
            Assert.IsNull(result.Bytes);
            Assert.IsNull(result.BytesLength);
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
        public void FileHelper_CopyFolderToNewFoler_WhenSourceMissing_ShouldKeepHistoricalSwallowBehavior()
        {
            var sourceFolderPath = Path.Combine(Path.GetTempPath(), $"lanymy_copy_missing_{Guid.NewGuid():N}");
            var targetFolderPath = Path.Combine(Path.GetTempPath(), $"lanymy_copy_target_{Guid.NewGuid():N}");

            try
            {
                Lanymy.Common.Helpers.FileHelper.CopyFolderToNewFoler(sourceFolderPath, targetFolderPath);

                Assert.IsFalse(Directory.Exists(targetFolderPath));
            }
            finally
            {
                if (Directory.Exists(targetFolderPath))
                {
                    Directory.Delete(targetFolderPath, true);
                }
            }
        }

        [TestMethod]
        public void FileHelper_CopyFolderToNewFolerWithResult_WhenSourceMissing_ShouldCapturePathsAndException()
        {
            var sourceFolderPath = Path.Combine(Path.GetTempPath(), $"lanymy_copy_missing_{Guid.NewGuid():N}");
            var targetFolderPath = Path.Combine(Path.GetTempPath(), $"lanymy_copy_target_{Guid.NewGuid():N}");

            var result = Lanymy.Common.Helpers.FileHelper.CopyFolderToNewFolerWithResult(sourceFolderPath, targetFolderPath);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsInstanceOfType(result.Exception, typeof(DirectoryNotFoundException));
            Assert.AreEqual(sourceFolderPath, result.SourcePath);
            Assert.AreEqual(targetFolderPath, result.TargetPath);
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

        [TestMethod]
        public void FileHelper_DeleteFolder_WhenFolderMissing_ShouldReturnFalse()
        {
            var sourceFolderPath = Path.Combine(Path.GetTempPath(), $"lanymy_delete_missing_{Guid.NewGuid():N}");

            var result = Lanymy.Common.Helpers.FileHelper.DeleteFolder(sourceFolderPath, false);

            Assert.IsFalse(result);
            Assert.IsFalse(Directory.Exists(sourceFolderPath));
        }

        [TestMethod]
        public void FileHelper_DeleteFolderWithResult_WhenClearSourceFolderTrue_ShouldRecreateDirectory()
        {
            var sourceFolderPath = Path.Combine(Path.GetTempPath(), $"lanymy_delete_recreate_{Guid.NewGuid():N}");
            var nestedFilePath = Path.Combine(sourceFolderPath, "nested.txt");

            try
            {
                Directory.CreateDirectory(sourceFolderPath);
                File.WriteAllText(nestedFilePath, "lanymy-delete-folder");

                var result = Lanymy.Common.Helpers.FileHelper.DeleteFolderWithResult(sourceFolderPath, true);

                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.Exception);
                Assert.IsTrue(Directory.Exists(sourceFolderPath));
                Assert.AreEqual(sourceFolderPath + Path.DirectorySeparatorChar, result.SourcePath);
                Assert.AreEqual(sourceFolderPath + Path.DirectorySeparatorChar, result.TargetPath);
                Assert.AreEqual(0, Directory.GetFileSystemEntries(sourceFolderPath).Length);
            }
            finally
            {
                if (Directory.Exists(sourceFolderPath))
                {
                    Directory.Delete(sourceFolderPath, true);
                }
            }
        }

        [TestMethod]
        public void FileHelper_CopyFileWithResult_WhenSourceExists_ShouldCopyFileContent()
        {
            var sourceFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_copy_source_{Guid.NewGuid():N}.txt");
            var targetFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_copy_target_{Guid.NewGuid():N}", "copied.txt");
            var expectedContent = "lanymy-copy-file";

            try
            {
                File.WriteAllText(sourceFilePath, expectedContent);

                var result = Lanymy.Common.Helpers.FileHelper.CopyFileWithResult(sourceFilePath, targetFilePath);

                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.Exception);
                Assert.IsTrue(File.Exists(targetFilePath));
                Assert.AreEqual(expectedContent, File.ReadAllText(targetFilePath));
            }
            finally
            {
                if (File.Exists(sourceFilePath))
                {
                    File.Delete(sourceFilePath);
                }

                var targetDirectoryPath = Path.GetDirectoryName(targetFilePath);
                if (!string.IsNullOrEmpty(targetDirectoryPath) && Directory.Exists(targetDirectoryPath))
                {
                    Directory.Delete(targetDirectoryPath, true);
                }
            }
        }

        [TestMethod]
        public void FileHelper_CopyFileWithResult_WhenScheduleMissing_ShouldReturnArgumentException()
        {
            var result = Lanymy.Common.Helpers.FileHelper.CopyFileWithResult((ScheduleFileInfoModel)null);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsInstanceOfType(result.Exception, typeof(ArgumentNullException));
            Assert.IsNull(result.SourcePath);
            Assert.IsNull(result.TargetPath);
        }

        [TestMethod]
        public void FileHelper_CopyFile_WhenScheduleMissing_ShouldThrowArgumentException()
        {
            try
            {
                Lanymy.Common.Helpers.FileHelper.CopyFile((ScheduleFileInfoModel)null);
                Assert.Fail("Expected ArgumentNullException was not thrown.");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("scheduleFileInfo", ex.ParamName);
            }
        }

        [TestMethod]
        public void FileHelper_MoveFileWithResult_WhenSourceMissing_ShouldCaptureException()
        {
            var sourceFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_move_missing_{Guid.NewGuid():N}.txt");
            var targetFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_move_target_{Guid.NewGuid():N}", "moved.txt");

            var result = Lanymy.Common.Helpers.FileHelper.MoveFileWithResult(sourceFilePath, targetFilePath);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Exception);
            Assert.IsFalse(File.Exists(targetFilePath));
        }

        [TestMethod]
        public void FileHelper_MoveFile_WhenSourceMissing_ShouldKeepHistoricalNoOpBehavior()
        {
            var sourceFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_move_missing_{Guid.NewGuid():N}.txt");
            var targetFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_move_target_{Guid.NewGuid():N}", "moved.txt");

            Lanymy.Common.Helpers.FileHelper.MoveFile(sourceFilePath, targetFilePath);

            Assert.IsFalse(File.Exists(targetFilePath));
        }

        [TestMethod]
        public void FileHelper_CreateBinaryFileWithResult_WhenBytesEmpty_ShouldCreateEmptyFile()
        {
            var targetFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_create_binary_{Guid.NewGuid():N}", "empty.bin");

            try
            {
                var result = Lanymy.Common.Helpers.FileHelper.CreateBinaryFileWithResult(targetFilePath, Array.Empty<byte>());

                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.Exception);
                Assert.IsTrue(File.Exists(targetFilePath));
                Assert.AreEqual(0, new FileInfo(targetFilePath).Length);
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
        public void FileHelper_CreateBinaryFileWithResult_WhenBytesNull_ShouldCaptureArgumentException()
        {
            var targetFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_create_binary_{Guid.NewGuid():N}", "null.bin");

            try
            {
                var result = Lanymy.Common.Helpers.FileHelper.CreateBinaryFileWithResult(targetFilePath, null);

                Assert.IsFalse(result.IsSuccess);
                Assert.IsInstanceOfType(result.Exception, typeof(ArgumentNullException));
                Assert.AreEqual(targetFilePath, result.SourcePath);
                Assert.AreEqual(targetFilePath, result.TargetPath);
                Assert.IsFalse(File.Exists(targetFilePath));
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
        public void FileHelper_CreateBinaryFile_WhenBytesEmpty_ShouldKeepHistoricalNoOpBehavior()
        {
            var targetFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_create_binary_{Guid.NewGuid():N}", "empty.bin");

            try
            {
                Lanymy.Common.Helpers.FileHelper.CreateBinaryFile(targetFilePath, Array.Empty<byte>());

                Assert.IsFalse(File.Exists(targetFilePath));
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
        public void FileHelper_GetBinaryFileBytes_WhenFileMissing_ShouldKeepHistoricalNullSemantics()
        {
            var missingFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_binary_missing_{Guid.NewGuid():N}.bin");

            var result = Lanymy.Common.Helpers.FileHelper.GetBinaryFileBytes(missingFilePath);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void FileHelper_CopyFilesWithResult_WhenOneItemFails_ShouldReturnPerItemResults()
        {
            var sourceRootPath = Path.Combine(Path.GetTempPath(), $"lanymy_copyfiles_source_{Guid.NewGuid():N}");
            var targetRootPath = Path.Combine(Path.GetTempPath(), $"lanymy_copyfiles_target_{Guid.NewGuid():N}");
            var sourceFilePath = Path.Combine(sourceRootPath, "source.txt");
            var copiedFilePath = Path.Combine(targetRootPath, "copied.txt");
            var missingSourceFilePath = Path.Combine(sourceRootPath, "missing.txt");

            try
            {
                Directory.CreateDirectory(sourceRootPath);
                File.WriteAllText(sourceFilePath, "lanymy-copy-files");

                var scheduleFileInfoList = new List<ScheduleFileInfoModel>
                {
                    new ScheduleFileInfoModel
                    {
                        SourceFileFullPath = sourceFilePath,
                        TargetFileFullPath = copiedFilePath,
                    },
                    new ScheduleFileInfoModel
                    {
                        SourceFileFullPath = missingSourceFilePath,
                        TargetFileFullPath = Path.Combine(targetRootPath, "missing.txt"),
                    }
                };

                var result = Lanymy.Common.Helpers.FileHelper.CopyFilesWithResult(scheduleFileInfoList);

                Assert.IsFalse(result.IsSuccess);
                Assert.IsNull(result.Exception);
                Assert.IsNotNull(result.FirstException);
                Assert.AreEqual(2, result.RequestedItemCount);
                Assert.AreEqual(1, result.SuccessCount);
                Assert.AreEqual(1, result.FailureCount);
                Assert.AreEqual(2, result.Results.Count);
                Assert.IsTrue(result.Results[0].IsSuccess);
                Assert.IsFalse(result.Results[1].IsSuccess);
                Assert.IsNotNull(result.Results[1].Exception);
                Assert.IsTrue(File.Exists(copiedFilePath));
            }
            finally
            {
                if (Directory.Exists(sourceRootPath))
                {
                    Directory.Delete(sourceRootPath, true);
                }

                if (Directory.Exists(targetRootPath))
                {
                    Directory.Delete(targetRootPath, true);
                }
            }
        }

        [TestMethod]
        public void FileHelper_CopyFiles_WhenScheduleListMissing_ShouldThrowArgumentException()
        {
            try
            {
                Lanymy.Common.Helpers.FileHelper.CopyFiles(null);
                Assert.Fail("Expected ArgumentNullException was not thrown.");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("scheduleFileInfoList", ex.ParamName);
            }
        }

        [TestMethod]
        public void FileHelper_CopyFiles_WhenFirstItemFails_ShouldKeepHistoricalThrowBehavior()
        {
            var sourceRootPath = Path.Combine(Path.GetTempPath(), $"lanymy_copyfiles_source_{Guid.NewGuid():N}");
            var targetRootPath = Path.Combine(Path.GetTempPath(), $"lanymy_copyfiles_target_{Guid.NewGuid():N}");
            var validSourceFilePath = Path.Combine(sourceRootPath, "source.txt");
            var missingSourceFilePath = Path.Combine(sourceRootPath, "missing.txt");
            var shouldNotBeCopiedPath = Path.Combine(targetRootPath, "copied.txt");

            try
            {
                Directory.CreateDirectory(sourceRootPath);
                File.WriteAllText(validSourceFilePath, "lanymy-copyfiles-throw");

                var scheduleFileInfoList = new List<ScheduleFileInfoModel>
                {
                    new ScheduleFileInfoModel
                    {
                        SourceFileFullPath = missingSourceFilePath,
                        TargetFileFullPath = Path.Combine(targetRootPath, "missing.txt"),
                    },
                    new ScheduleFileInfoModel
                    {
                        SourceFileFullPath = validSourceFilePath,
                        TargetFileFullPath = shouldNotBeCopiedPath,
                    }
                };

                try
                {
                    Lanymy.Common.Helpers.FileHelper.CopyFiles(scheduleFileInfoList);
                    Assert.Fail("Expected FileNotFoundException was not thrown.");
                }
                catch (FileNotFoundException ex)
                {
                    Assert.AreEqual(missingSourceFilePath, ex.FileName);
                }

                Assert.IsFalse(File.Exists(shouldNotBeCopiedPath));
            }
            finally
            {
                if (Directory.Exists(sourceRootPath))
                {
                    Directory.Delete(sourceRootPath, true);
                }

                if (Directory.Exists(targetRootPath))
                {
                    Directory.Delete(targetRootPath, true);
                }
            }
        }

        [TestMethod]
        public void FileHelper_CopyFilesWithResult_WhenScheduleListMissing_ShouldReturnBatchLevelException()
        {
            var result = Lanymy.Common.Helpers.FileHelper.CopyFilesWithResult(null);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsInstanceOfType(result.Exception, typeof(ArgumentNullException));
            Assert.AreSame(result.Exception, result.FirstException);
            Assert.AreEqual(0, result.RequestedItemCount);
            Assert.AreEqual(0, result.Results.Count);
            Assert.AreEqual(0, result.SuccessCount);
            Assert.AreEqual(0, result.FailureCount);
        }

        [TestMethod]
        public void FileHelper_CopyFilesWithResult_WhenOneScheduleItemMissing_ShouldPreserveRequestedCountAndPerItemFailure()
        {
            var sourceRootPath = Path.Combine(Path.GetTempPath(), $"lanymy_copyfiles_source_{Guid.NewGuid():N}");
            var targetRootPath = Path.Combine(Path.GetTempPath(), $"lanymy_copyfiles_target_{Guid.NewGuid():N}");
            var sourceFilePath = Path.Combine(sourceRootPath, "source.txt");

            try
            {
                Directory.CreateDirectory(sourceRootPath);
                File.WriteAllText(sourceFilePath, "lanymy-copy-files-null-item");

                var scheduleFileInfoList = new List<ScheduleFileInfoModel>
                {
                    new ScheduleFileInfoModel
                    {
                        SourceFileFullPath = sourceFilePath,
                        TargetFileFullPath = Path.Combine(targetRootPath, "copied.txt"),
                    },
                    null
                };

                var result = Lanymy.Common.Helpers.FileHelper.CopyFilesWithResult(scheduleFileInfoList);

                Assert.IsFalse(result.IsSuccess);
                Assert.IsNull(result.Exception);
                Assert.IsNotNull(result.FirstException);
                Assert.AreEqual(2, result.RequestedItemCount);
                Assert.AreEqual(2, result.Results.Count);
                Assert.AreEqual(1, result.SuccessCount);
                Assert.AreEqual(1, result.FailureCount);
                Assert.IsTrue(result.Results[0].IsSuccess);
                Assert.IsInstanceOfType(result.Results[1].Exception, typeof(ArgumentNullException));
            }
            finally
            {
                if (Directory.Exists(sourceRootPath))
                {
                    Directory.Delete(sourceRootPath, true);
                }

                if (Directory.Exists(targetRootPath))
                {
                    Directory.Delete(targetRootPath, true);
                }
            }
        }

        [TestMethod]
        public void FileHelper_GetStreamHashCode_WhenOffsetSpecified_ShouldHashTailBytes()
        {
            var sourceBytes = new byte[] { 10, 20, 30, 40, 50, 60 };
            using var stream = new MemoryStream(sourceBytes);

            var hashFromStream = Lanymy.Common.Helpers.FileHelper.GetStreamHashCode(stream, 2);
            var hashFromBytes = Lanymy.Common.Helpers.FileHelper.GetBytesHashCode(new byte[] { 30, 40, 50, 60 });

            Assert.AreEqual(hashFromBytes, hashFromStream);
        }

        [TestMethod]
        public void FileHelper_GetStreamHashCode_WhenOffsetOutOfRange_ShouldHashFromStreamStart()
        {
            var sourceBytes = new byte[] { 10, 20, 30, 40, 50, 60 };
            using var stream = new MemoryStream(sourceBytes);
            stream.Position = 4;

            var hashFromStream = Lanymy.Common.Helpers.FileHelper.GetStreamHashCode(stream, 99);
            var hashFromBytes = Lanymy.Common.Helpers.FileHelper.GetBytesHashCode(sourceBytes);

            Assert.AreEqual(hashFromBytes, hashFromStream);
        }

        [TestMethod]
        public void FileHelper_GetFileHashCode_WhenOffsetSpecified_ShouldHashTailBytes()
        {
            var sourceFilePath = Path.Combine(Path.GetTempPath(), $"lanymy_hash_source_{Guid.NewGuid():N}.bin");
            var sourceBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            try
            {
                File.WriteAllBytes(sourceFilePath, sourceBytes);

                var hashFromFile = Lanymy.Common.Helpers.FileHelper.GetFileHashCode(sourceFilePath, 3);
                var hashFromBytes = Lanymy.Common.Helpers.FileHelper.GetBytesHashCode(new byte[] { 4, 5, 6, 7, 8 });

                Assert.AreEqual(hashFromBytes, hashFromFile);
            }
            finally
            {
                if (File.Exists(sourceFilePath))
                {
                    File.Delete(sourceFilePath);
                }
            }
        }

        [TestMethod]
        public void FileHelper_ReadToBuffer_WhenStreamReturnsPartialChunks_ShouldFillRequestedCount()
        {
            var sourceBytes = new byte[] { 1, 2, 3, 4, 5, 6 };
            var buffer = new byte[6];

            using var stream = new ChunkedReadStream(sourceBytes, 2);

            var readCount = Lanymy.Common.Helpers.FileHelper.ReadToBuffer(stream, buffer, 0, buffer.Length);

            Assert.AreEqual(buffer.Length, readCount);
            CollectionAssert.AreEqual(sourceBytes, buffer);
        }

        [TestMethod]
        public void FileHelper_ReadExactly_WhenStreamEndsEarly_ShouldThrowEndOfStreamException()
        {
            var buffer = new byte[6];

            using var stream = new ChunkedReadStream(new byte[] { 1, 2, 3 }, 2);

            try
            {
                Lanymy.Common.Helpers.FileHelper.ReadExactly(stream, buffer, 0, buffer.Length);
                Assert.Fail("Expected EndOfStreamException was not thrown.");
            }
            catch (EndOfStreamException ex)
            {
                StringAssert.Contains(ex.Message, "Expected to read 6 bytes");
            }
        }
    }
}
