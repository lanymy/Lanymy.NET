using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Lanymy.Common.Abstractions.Models;
using Lanymy.Common.Enums;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Helpers.ResultModels;

namespace Lanymy.Common.Helpers
{
    /// <summary>
    /// 文件扩展类
    /// </summary>
    public class FileHelper
    {



        /// <summary>
        /// 获取文件哈希值
        /// </summary>
        /// <param name="fileFullPath">文件全路径</param>
        /// <param name="offset">偏移量 默认值 0 不偏移</param>
        /// <returns>文件哈希值</returns>
        public static string GetFileHashCode(string fileFullPath, int offset = 0)
        {
            string hashString = string.Empty;

            using (var fileStream = File.OpenRead(fileFullPath))
            {
                hashString = GetStreamHashCode(fileStream, offset);
            }

            return hashString;
        }

        /// <summary>
        /// 获取数据流的哈希值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="offset">偏移量 默认值 0 不偏移</param>
        /// <param name="hashAlgorithmType">哈希散列算法类型 默认使用 SHA256</param>
        /// <returns></returns>
        public static string GetStreamHashCode(Stream inputStream, int offset = 0, HashAlgorithmTypeEnum hashAlgorithmType = HashAlgorithmTypeEnum.SHA256)
        {
            string hashString = string.Empty;
            using (var hash = CreateHashAlgorithm(hashAlgorithmType))
            {
                if (inputStream.CanSeek)
                {
                    var normalizedOffset = offset >= 0 && offset < inputStream.Length ? offset : 0;
                    inputStream.Position = normalizedOffset;
                }
                var bytes = hash.ComputeHash(inputStream);
                hashString = BitConverter.ToString(bytes).Replace("-", "");
            }

            return hashString;
        }

        private static HashAlgorithm CreateHashAlgorithm(HashAlgorithmTypeEnum hashAlgorithmType)
        {
            switch (hashAlgorithmType)
            {
                case HashAlgorithmTypeEnum.MD5:
                    return MD5.Create();
                case HashAlgorithmTypeEnum.SHA1:
                    return SHA1.Create();
                case HashAlgorithmTypeEnum.SHA256:
                    return SHA256.Create();
                case HashAlgorithmTypeEnum.SHA384:
                    return SHA384.Create();
                case HashAlgorithmTypeEnum.SHA512:
                    return SHA512.Create();
                default:
                    throw new NotSupportedException($"Unsupported hash algorithm type: {hashAlgorithmType}.");
            }
        }

        /// <summary>
        /// 获取 二进制数组的 哈希值
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset">偏移量 默认值 0 不偏移</param>
        /// <param name="hashAlgorithmType">哈希散列算法类型 默认使用 SHA256</param>
        /// <returns></returns>
        public static string GetBytesHashCode(byte[] bytes, int offset = 0, HashAlgorithmTypeEnum hashAlgorithmType = HashAlgorithmTypeEnum.SHA256)
        {
            string hashString = string.Empty;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                hashString = GetStreamHashCode(ms, offset, hashAlgorithmType);
            }
            return hashString;
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="sourceFileFullPath">源文件全路径</param>
        /// <param name="targetFileFullPath">目标文件全路径</param>
        /// <param name="ifOverWriteTargetFile">如果目标文件存在 是否 覆盖目标文件</param>
        public static void CopyFile(string sourceFileFullPath, string targetFileFullPath, bool ifOverWriteTargetFile = true)
        {
            var result = CopyFileWithResult(sourceFileFullPath, targetFileFullPath, ifOverWriteTargetFile);
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }
        }

        /// <summary>
        /// 复制文件，并返回详细结果。
        /// </summary>
        /// <param name="sourceFileFullPath">源文件全路径</param>
        /// <param name="targetFileFullPath">目标文件全路径</param>
        /// <param name="ifOverWriteTargetFile">如果目标文件存在 是否覆盖目标文件</param>
        /// <returns>文件复制结果</returns>
        public static FileOperationResultModel CopyFileWithResult(string sourceFileFullPath, string targetFileFullPath, bool ifOverWriteTargetFile = true)
        {
            var result = new FileOperationResultModel
            {
                SourcePath = sourceFileFullPath,
                TargetPath = targetFileFullPath,
            };

            try
            {
                if (!File.Exists(sourceFileFullPath))
                {
                    throw new FileNotFoundException("Source file does not exist.", sourceFileFullPath);
                }

                PathHelper.InitDirectoryPath(targetFileFullPath);
                File.Copy(sourceFileFullPath, targetFileFullPath, ifOverWriteTargetFile);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="scheduleFileInfo">文件调度信息实体类</param>
        /// <param name="ifOverWriteTargetFile">如果目标文件存在 是否 覆盖目标文件</param>
        public static void CopyFile(ScheduleFileInfoModel scheduleFileInfo, bool ifOverWriteTargetFile = true)
        {
            if (scheduleFileInfo.IfIsNullOrEmpty())
                throw new ArgumentNullException(nameof(scheduleFileInfo));
            CopyFile(scheduleFileInfo.SourceFileFullPath, scheduleFileInfo.TargetFileFullPath, ifOverWriteTargetFile);
        }

        /// <summary>
        /// 复制文件，并返回详细结果。
        /// </summary>
        /// <param name="scheduleFileInfo">文件调度信息实体类</param>
        /// <param name="ifOverWriteTargetFile">如果目标文件存在 是否覆盖目标文件</param>
        /// <returns>文件复制结果</returns>
        public static FileOperationResultModel CopyFileWithResult(ScheduleFileInfoModel scheduleFileInfo, bool ifOverWriteTargetFile = true)
        {
            if (scheduleFileInfo.IfIsNullOrEmpty())
            {
                return new FileOperationResultModel
                {
                    Exception = new ArgumentNullException(nameof(scheduleFileInfo)),
                };
            }

            return CopyFileWithResult(scheduleFileInfo.SourceFileFullPath, scheduleFileInfo.TargetFileFullPath, ifOverWriteTargetFile);
        }

        /// <summary>
        /// 批量复制文件
        /// </summary>
        /// <param name="scheduleFileInfoList">文件调度信息实体类集合</param>
        /// <param name="ifOverWriteTargetFile">如果目标文件存在 是否 覆盖目标文件</param>
        public static void CopyFiles(IEnumerable<ScheduleFileInfoModel> scheduleFileInfoList, bool ifOverWriteTargetFile = true)
        {
            if (scheduleFileInfoList.IfIsNullOrEmpty())
                throw new ArgumentNullException(nameof(scheduleFileInfoList));

            foreach (var scheduleFileInfoModel in scheduleFileInfoList)
            {
                CopyFile(scheduleFileInfoModel, ifOverWriteTargetFile);
            }
        }

        /// <summary>
        /// 批量复制文件，并返回明细结果。
        /// </summary>
        /// <param name="scheduleFileInfoList">文件调度信息实体类集合</param>
        /// <param name="ifOverWriteTargetFile">如果目标文件存在 是否覆盖目标文件</param>
        /// <returns>批量文件复制结果</returns>
        public static BatchFileOperationResultModel CopyFilesWithResult(IEnumerable<ScheduleFileInfoModel> scheduleFileInfoList, bool ifOverWriteTargetFile = true)
        {
            var scheduleFileInfos = scheduleFileInfoList?.ToList();

            if (scheduleFileInfos.IfIsNullOrEmpty())
            {
                return new BatchFileOperationResultModel
                {
                    RequestedItemCount = scheduleFileInfos?.Count ?? 0,
                    Exception = new ArgumentNullException(nameof(scheduleFileInfoList)),
                };
            }

            var results = new List<FileOperationResultModel>();

            foreach (var scheduleFileInfoModel in scheduleFileInfos)
            {
                results.Add(CopyFileWithResult(scheduleFileInfoModel, ifOverWriteTargetFile));
            }

            return new BatchFileOperationResultModel
            {
                RequestedItemCount = scheduleFileInfos.Count,
                Results = results,
            };
        }

        #region 移动文件

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="sourceFileFullPath">源文件物理全路径</param>
        /// <param name="targetFileFullPath">目标文件物理全路径</param>
        public static void MoveFile(string sourceFileFullPath, string targetFileFullPath)
        {
            if (!File.Exists(sourceFileFullPath))
            {
                return;
            }

            var result = MoveFileWithResult(sourceFileFullPath, targetFileFullPath);
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }
        }

        /// <summary>
        /// 移动文件，并返回详细结果。
        /// </summary>
        /// <param name="sourceFileFullPath">源文件物理全路径</param>
        /// <param name="targetFileFullPath">目标文件物理全路径</param>
        /// <returns>文件移动结果</returns>
        public static FileOperationResultModel MoveFileWithResult(string sourceFileFullPath, string targetFileFullPath)
        {
            var result = new FileOperationResultModel
            {
                SourcePath = sourceFileFullPath,
                TargetPath = targetFileFullPath,
            };

            try
            {
                if (!File.Exists(sourceFileFullPath))
                {
                    throw new FileNotFoundException("Source file does not exist.", sourceFileFullPath);
                }

                PathHelper.InitDirectoryPath(targetFileFullPath);
                File.Move(sourceFileFullPath, targetFileFullPath);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        #endregion

        #region 将一个文件夹中的内容复制到另一文件夹（源文件夹中文件和子目录也将一起复制到目标文件夹）

        /// <summary>
        /// 将一个文件夹中的内容复制到另一文件夹（源文件夹中文件和子目录也将一起复制到目标文件夹）
        /// </summary>
        /// <param name="sourceFolderPath">源文件夹物理路径</param>
        /// <param name="targetFolderPath">目标文件夹物理路径</param>
        public static void CopyFolderToNewFoler(string sourceFolderPath, string targetFolderPath)
        {
            CopyFolderToNewFolerWithResult(sourceFolderPath, targetFolderPath);
        }

        /// <summary>
        /// 将一个文件夹中的内容复制到另一文件夹，并返回详细结果。
        /// </summary>
        /// <param name="sourceFolderPath">源文件夹物理路径</param>
        /// <param name="targetFolderPath">目标文件夹物理路径</param>
        /// <returns>文件夹复制结果</returns>
        public static FileOperationResultModel CopyFolderToNewFolerWithResult(string sourceFolderPath, string targetFolderPath)
        {
            var result = new FileOperationResultModel
            {
                SourcePath = sourceFolderPath,
                TargetPath = targetFolderPath,
            };

            try
            {
                CopyFolderCore(sourceFolderPath, targetFolderPath);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        private static void CopyFolderCore(string sourceFolderPath, string targetFolderPath)
        {
            if (!Directory.Exists(sourceFolderPath))
            {
                throw new DirectoryNotFoundException($"Source directory does not exist: {sourceFolderPath}");
            }

            if (!Directory.Exists(targetFolderPath))
            {
                Directory.CreateDirectory(targetFolderPath);
            }

            var directInfo = new DirectoryInfo(sourceFolderPath);

            foreach (var fileinfo in directInfo.GetFiles())
            {
                var fileName = fileinfo.Name;
                File.Copy(fileinfo.FullName, Path.Combine(targetFolderPath, fileName), true);
            }

            foreach (var directoryPath in directInfo.GetDirectories())
            {
                var toDirPath = Path.Combine(targetFolderPath, directoryPath.Name);
                CopyFolderCore(directoryPath.FullName, toDirPath);
            }
        }

        #endregion


        #region 删除文件夹及文件夹中的所有内容


        /// <summary>
        /// 删除文件夹及文件夹中的所有内容
        /// </summary>
        /// <param name="sourceFolderPath">文件夹路径</param>
        /// <param name="ifClearSourceFolder">True 清空文件夹  False 删除文件夹 </param>
        /// <returns></returns>
        public static bool DeleteFolder(string sourceFolderPath, bool ifClearSourceFolder)
        {
            return DeleteFolderWithResult(sourceFolderPath, ifClearSourceFolder).IsSuccess;
        }

        /// <summary>
        /// 删除文件夹及文件夹中的所有内容，并返回详细结果。
        /// </summary>
        /// <param name="sourceFolderPath">文件夹路径</param>
        /// <param name="ifClearSourceFolder">True 清空文件夹  False 删除文件夹</param>
        /// <returns>文件夹删除结果</returns>
        public static FileOperationResultModel DeleteFolderWithResult(string sourceFolderPath, bool ifClearSourceFolder)
        {
            var result = new FileOperationResultModel
            {
                SourcePath = sourceFolderPath,
                TargetPath = sourceFolderPath,
            };

            try
            {
                sourceFolderPath = PathHelper.GetFolderPath(sourceFolderPath);

                Directory.Delete(sourceFolderPath, true);

                if (ifClearSourceFolder)
                {
                    PathHelper.InitDirectoryPath(sourceFolderPath);
                }

                result.SourcePath = sourceFolderPath;
                result.TargetPath = sourceFolderPath;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        #endregion


        #region 二进制 文件操作


        /// <summary>
        /// 创建二进制文件
        /// </summary>
        /// <param name="binaryFileFullPath">二进制文件全路径</param>
        /// <param name="bytes">二进制数据</param>
        public static void CreateBinaryFile(string binaryFileFullPath, byte[] bytes)
        {
            if (bytes.IfIsNullOrEmpty()) return;
            var result = CreateBinaryFileWithResult(binaryFileFullPath, bytes);
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }
        }

        /// <summary>
        /// 创建二进制文件，并返回详细结果。
        /// </summary>
        /// <param name="binaryFileFullPath">二进制文件全路径</param>
        /// <param name="bytes">二进制数据</param>
        /// <returns>文件创建结果</returns>
        public static FileOperationResultModel CreateBinaryFileWithResult(string binaryFileFullPath, byte[] bytes)
        {
            var result = new FileOperationResultModel
            {
                SourcePath = binaryFileFullPath,
                TargetPath = binaryFileFullPath,
            };

            try
            {
                if (bytes == null)
                {
                    throw new ArgumentNullException(nameof(bytes));
                }

                PathHelper.InitDirectoryPath(binaryFileFullPath);
                using (FileStream fs = new FileStream(binaryFileFullPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    if (bytes.Length > 0)
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// 一次性 读取 出来 二进制文件 内的 全部二进制数据 
        /// </summary>
        /// <param name="binaryFileFullPath">二进制文件全路径</param>
        /// <returns></returns>
        public static byte[] GetBinaryFileBytes(string binaryFileFullPath)
        {
            var result = GetBinaryFileBytesWithResult(binaryFileFullPath);
            if (!result.IsSuccess)
            {
                if (result.Exception is FileNotFoundException)
                {
                    return null;
                }

                if (result.Exception != null)
                {
                    throw result.Exception;
                }
            }

            return result.Bytes;
        }

        /// <summary>
        /// 一次性读取二进制文件全部内容，并返回详细结果。
        /// </summary>
        /// <param name="binaryFileFullPath">二进制文件全路径</param>
        /// <returns>二进制文件读取结果</returns>
        public static BinaryFileReadResultModel GetBinaryFileBytesWithResult(string binaryFileFullPath)
        {
            var result = new BinaryFileReadResultModel
            {
                FilePath = binaryFileFullPath,
            };

            try
            {
                if (!File.Exists(binaryFileFullPath))
                {
                    throw new FileNotFoundException("Binary file does not exist.", binaryFileFullPath);
                }

                using (FileStream fs = new FileStream(binaryFileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fs.Length > int.MaxValue)
                    {
                        throw new IOException("Binary file is too large to read into a single byte array.");
                    }

                    result.Bytes = new byte[fs.Length];
                    ReadExactly(fs, result.Bytes, 0, result.Bytes.Length);
                }

                result.BytesLength = result.Bytes.Length;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// 循环读取流，直到缓冲区读满或遇到 EOF。
        /// </summary>
        /// <param name="inputStream">输入流</param>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="offset">缓冲区偏移量</param>
        /// <param name="count">需要读取的字节数</param>
        /// <returns>实际读取的字节数</returns>
        public static int ReadToBuffer(Stream inputStream, byte[] buffer, int offset, int count)
        {
            if (inputStream.IfIsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(inputStream));
            }

            if (buffer.IfIsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0 || count < 0 || buffer.Length - offset < count)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            var totalReadCount = 0;

            while (totalReadCount < count)
            {
                var currentReadCount = inputStream.Read(buffer, offset + totalReadCount, count - totalReadCount);
                if (currentReadCount <= 0)
                {
                    break;
                }

                totalReadCount += currentReadCount;
            }

            return totalReadCount;
        }

        /// <summary>
        /// 循环读取流，要求缓冲区必须被完整填满。
        /// </summary>
        /// <param name="inputStream">输入流</param>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="offset">缓冲区偏移量</param>
        /// <param name="count">需要读取的字节数</param>
        public static void ReadExactly(Stream inputStream, byte[] buffer, int offset, int count)
        {
            var totalReadCount = ReadToBuffer(inputStream, buffer, offset, count);
            if (totalReadCount != count)
            {
                throw new EndOfStreamException($"Expected to read {count} bytes, but only read {totalReadCount} bytes.");
            }
        }

        #endregion


    }
}
