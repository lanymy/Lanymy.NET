using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Models;

namespace Lanymy.Common
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
        /// <returns></returns>
        public static string GetStreamHashCode(Stream inputStream, int offset = 0)
        {
            string hashString = string.Empty;
            using (var hash = HashAlgorithm.Create())
            {
                if (offset >= 0 && offset < inputStream.Length)
                {
                    inputStream.Position = offset;
                }
                var bytes = hash.ComputeHash(inputStream);
                hashString = BitConverter.ToString(bytes).Replace("-", "");
            }

            return hashString;
        }

        /// <summary>
        /// 获取 二进制数组的 哈希值
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset">偏移量 默认值 0 不偏移</param>
        /// <returns></returns>
        public static string GetBytesHashCode(byte[] bytes, int offset = 0)
        {
            string hashString = string.Empty;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                hashString = GetStreamHashCode(ms, offset);
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

            if (!File.Exists(sourceFileFullPath))
                throw new FileNotFoundException(nameof(sourceFileFullPath));

            PathHelper.InitDirectoryPath(targetFileFullPath);
            File.Copy(sourceFileFullPath, targetFileFullPath, ifOverWriteTargetFile);

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

        #region 移动文件

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="sourceFileFullPath">源文件物理全路径</param>
        /// <param name="targetFileFullPath">目标文件物理全路径</param>
        public static void MoveFile(string sourceFileFullPath, string targetFileFullPath)
        {
            if (File.Exists(sourceFileFullPath))
            {
                PathHelper.InitDirectoryPath(targetFileFullPath);
                File.Move(sourceFileFullPath, targetFileFullPath);
            }
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

            try
            {

                if (!Directory.Exists(sourceFolderPath))
                {
                    throw new ApplicationException("Source directory does not exist");
                }
                if (!Directory.Exists(targetFolderPath))
                {
                    Directory.CreateDirectory(targetFolderPath);
                }
                DirectoryInfo directInfo = new DirectoryInfo(sourceFolderPath);
                //copy files
                FileInfo[] filesInfos = directInfo.GetFiles();
                foreach (FileInfo fileinfo in filesInfos)
                {
                    string fileName = fileinfo.Name;
                    //File.Copy(fileinfo.FullName, targetFolderPath + @"/" + fileName, true);
                    File.Copy(fileinfo.FullName, Path.Combine(targetFolderPath, fileName), true);
                }
                //copy directory
                foreach (DirectoryInfo directoryPath in directInfo.GetDirectories())
                {
                    //string toDirPath = toDir + @"/" + directoryPath.Name;
                    string toDirPath = Path.Combine(targetFolderPath, directoryPath.Name);
                    CopyFolderToNewFoler(directoryPath.FullName, toDirPath);
                }

                //targetFolderPath = PathHelper.GetFolderPath(targetFolderPath);

                ////// 判断目标目录是否存在如果不存在则新建之
                ////if (!Directory.Exists(desPath))
                ////{
                ////    Directory.CreateDirectory(desPath);
                ////}

                //PathHelper.InitDirectoryPath(targetFolderPath);

                //// 得到源目录的文件列表,该里面是包含文件名以及子目录名的一个数组
                //string[] fileList = Directory.GetFileSystemEntries(sourceFolderPath);
                ////若只需复制源目录中的文件，只使用下面的数组
                ////string[] fileList = Directory.GetFiles(sourcePath);

                //// 遍历所有的文件和子目录
                //foreach (string file in fileList)
                //{
                //    // 先将文件都当作目录处理，如果存在这个目录就递归,Copy该目录下面的所有文件
                //    if (Directory.Exists(file))
                //    {
                //        CopyFolderToNewFoler(file, Path.Combine(targetFolderPath, Path.GetFileName(file)));
                //    }
                //    // 否则直接Copy文件
                //    else
                //    {
                //        File.Copy(file, Path.Combine(targetFolderPath, Path.GetFileName(file)), true);
                //    }
                //}

            }
            catch
            {

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
            try
            {

                sourceFolderPath = PathHelper.GetFolderPath(sourceFolderPath);

                Directory.Delete(sourceFolderPath, true);


                if (ifClearSourceFolder)
                {
                    PathHelper.InitDirectoryPath(sourceFolderPath);
                }



                ////判断目标目录是否存在如果不存在则新建之
                //if (!Directory.Exists(desPath))
                //{
                //    Directory.CreateDirectory(desPath);
                //}

                ////得到源目录的文件列表,该里面是包含文件名以及子目录名的一个数组
                //string[] fileList = Directory.GetFileSystemEntries(sourceFolderPath);
                ////若只需复制源目录中的文件，只使用下面的数组
                ////string[] fileList = Directory.GetFiles(sourcePath);

                ////遍历所有的文件和子目录
                //foreach (string file in fileList)
                //{
                //    // 先将文件都当作目录处理，如果存在这个目录就递归,Delete该目录下面的所有文件
                //    if (Directory.Exists(file))
                //    {
                //        DeleteFolderAndFiles(desPath + Path.GetFileName(file));
                //    }
                //    //否则直接Delete文件
                //    else
                //    {
                //        File.Delete(desPath + Path.GetFileName(file));
                //    }
                //}
                ////最后删除文件夹
                //System.IO.Directory.Delete(desPath, true);


            }
            catch
            {
                return false;
            }

            return true;
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
            PathHelper.InitDirectoryPath(binaryFileFullPath);
            using (FileStream fs = new FileStream(binaryFileFullPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// 一次性 读取 出来 二进制文件 内的 全部二进制数据 
        /// </summary>
        /// <param name="binaryFileFullPath">二进制文件全路径</param>
        /// <returns></returns>
        public static byte[] GetBinaryFileBytes(string binaryFileFullPath)
        {

            if (!File.Exists(binaryFileFullPath)) return null;

            byte[] bytes;

            using (FileStream fs = new FileStream(binaryFileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
            }

            return bytes;
        }

        #endregion


    }
}
