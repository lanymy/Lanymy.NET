//// *******************************************************************
//// 创建时间：2016年02月13日, AM 10:46:00
//// 作者：lanyanmiyu@qq.com
//// 说明：Zip 操作类
//// 其它:
//// *******************************************************************


//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Lanymy.General.Extension.ExtensionFunctions;
//using Ionic.Zip;
//using Lanymy.General.Extension.Models;
//using SharpCompress.Common;
//using SharpCompress.Readers;


//namespace Lanymy.General.Extension
//{

//    /// <summary>
//    /// Zip 操作类
//    /// </summary>
//    public class ZipFunctions
//    {





//        private static void AddFileToZip(string sourceFileFullPath, string zipFileFullPath, ZipFile zip)
//        {
//            if (PathFunctions.GetPathType(zipFileFullPath) == PathTypeEnum.File)
//            {

//                var zipFileName = Path.GetFileName(zipFileFullPath);
//                var zipFileDirFullPath = zipFileFullPath.RightRemoveString(zipFileName.Length);

//                var zipEntry = zip.AddFile(sourceFileFullPath, zipFileDirFullPath);
//                zipEntry.FileName = zipEntry.FileName.RightRemoveString(Path.GetFileName(sourceFileFullPath).Length) + zipFileName;

//            }
//            else
//            {
//                zip.AddFile(sourceFileFullPath, zipFileFullPath);
//            }
//        }


//        /// <summary>
//        /// 压缩成zip文件
//        /// </summary>
//        /// <param name="zipCompressInfoList">要压缩的  文件 / 文件夹 全路径 支持多个同时合并压缩 (文件路径直接压缩 文件夹路径支持通配符过滤压缩)</param>
//        /// <param name="targetZipFileFullPath">压缩成zip的文件全路径</param>
//        /// <param name="dirSearchPatterns">只用做 过滤文件夹路径下的文件 文件过滤通配符  如 : *.txt 只压缩 txt文件 默认null 不过滤文件 压缩所有文件</param>
//        /// <returns></returns>
//        public static bool CompressToZipFile(List<ZipCompressInfoModel> zipCompressInfoList, string targetZipFileFullPath, params string[] dirSearchPatterns)
//        {

//            try
//            {

//                if (zipCompressInfoList.IfIsNullOrEmpty())
//                {
//                    throw new ArgumentNullException("zipCompressInfoList");
//                }
//                if (targetZipFileFullPath.IfIsNullOrEmpty())
//                {
//                    throw new ArgumentNullException("targetZipFileFullPath");
//                }
//                if (PathFunctions.GetPathType(targetZipFileFullPath) != PathTypeEnum.File)
//                {
//                    throw new ArgumentException("targetZipFileFullPath 不是有效的文件全路径信息");
//                }


//                using (ZipFile zip = new ZipFile(Encoding.Default))
//                {

//                    //string fileRootDirFullPath = string.Empty;


//                    foreach (var zipCompressInfo in zipCompressInfoList)
//                    {
//                        string zipFileFullPath = zipCompressInfo.TargetFullPath;
//                        if (zipFileFullPath.IfIsNullOrEmpty())
//                        {
//                            zipFileFullPath = "";
//                        }
//                        if (zipFileFullPath.StartsWith("/"))
//                        {
//                            zipFileFullPath = zipFileFullPath.Remove(0, 1);
//                        }
//                        var pathType = PathFunctions.GetPathType(zipCompressInfo.SourceFullPath);
//                        if (pathType == PathTypeEnum.File) //文件路径
//                        {
//                            AddFileToZip(zipCompressInfo.SourceFullPath, zipFileFullPath, zip);
//                        }
//                        else if (pathType == PathTypeEnum.Directory) //文件夹路径
//                        {

//                            if (!zipCompressInfo.SourceFullPath.EndsWith("\\"))
//                            {
//                                zipCompressInfo.SourceFullPath += "\\";
//                            }
//                            if (!zipFileFullPath.EndsWith("/"))
//                            {
//                                zipFileFullPath += "/";
//                            }

//                            if (dirSearchPatterns.IfIsNullOrEmpty()) //压缩全部文件
//                            {
//                                zip.AddDirectory(zipCompressInfo.SourceFullPath, zipFileFullPath == "/" ? "" : zipFileFullPath);
//                            }
//                            else
//                            {
//                                //List<string> files = new List<string>();
//                                //fileRootDirName = PathFunctions.GetPathRootFolderName(zipSourceFullPath);
//                                //fileRootDirFullPath = PathFunctions.GetFolderPath(zipSourceFullPath);
//                                //foreach (var dirSearchPattern in dirSearchPatterns)
//                                //{
//                                //    files.AddRange(Directory.GetFiles(zipCompressInfo.SourceFullPath, dirSearchPattern, SearchOption.AllDirectories));
//                                //}



//                                foreach (var file in PathFunctions.GetFilesFromFolder(zipCompressInfo.SourceFullPath, dirSearchPatterns))
//                                {
//                                    AddFileToZip(file, file.Replace(zipCompressInfo.SourceFullPath, zipFileFullPath).Replace("\\", "/"), zip);
//                                }

//                            }
//                        }
//                    }

//                    PathFunctions.InitDirectoryPath(targetZipFileFullPath);
//                    zip.Save(targetZipFileFullPath);
//                }

//            }
//            catch (Exception)
//            {
//                return false;
//            }

//            return true;
//        }



//        /// <summary>
//        /// 解压缩zip文件
//        /// </summary>
//        /// <param name="sourceZipFileFullPath">zip文件全路径</param>
//        /// <param name="targetDirFullPath">要解压缩到的目录全路径</param>
//        /// <param name="ifOverWrite">是否覆盖重名文件 True 覆盖; False 不覆盖 . 默认 True 覆盖</param>
//        /// <returns>如果解压缩成功 返回 新的路径 ; 失败 则返回空</returns>
//        public static string DecompressZipFile(string sourceZipFileFullPath, string targetDirFullPath, bool ifOverWrite = true)
//        {

//            try
//            {
//                if (!File.Exists(sourceZipFileFullPath))
//                    throw new FileNotFoundException("sourceZipFileFullPath");
//                targetDirFullPath = Path.Combine(targetDirFullPath, Path.GetFileNameWithoutExtension(sourceZipFileFullPath));
//                PathFunctions.InitDirectoryPath(targetDirFullPath, true);


//                if (Path.GetExtension(sourceZipFileFullPath).ToLower() == ".zip")
//                {
//                    //调用zip解压缩api
//                    DecompressZip(sourceZipFileFullPath, targetDirFullPath, ifOverWrite);
//                }
//                else
//                {
//                    //其它压缩包格式解压缩 如 rar  7z 等
//                    DecompressOther(sourceZipFileFullPath, targetDirFullPath, ifOverWrite);
//                }

//                return targetDirFullPath;
//            }
//            catch (Exception)
//            {
//                return null;
//            }

//        }


//        private static void DecompressZip(string sourceZipFileFullPath, string targetDirFullPath, bool ifOverWrite)
//        {
//            ExtractExistingFileAction extractExistingFileAction = ifOverWrite ? ExtractExistingFileAction.OverwriteSilently : ExtractExistingFileAction.DoNotOverwrite;

//            using (ZipFile zip = ZipFile.Read(sourceZipFileFullPath, new ReadOptions { Encoding = Encoding.Default }))
//            {
//                foreach (ZipEntry e in zip)
//                {
//                    e.Extract(targetDirFullPath, extractExistingFileAction);  // overwrite == true
//                }
//            }
//        }

//        private static void DecompressOther(string sourceZipFileFullPath, string targetDirFullPath, bool ifOverWrite)
//        {

//            using (Stream stream = File.OpenRead(sourceZipFileFullPath))
//            {

//                ArchiveEncoding.Default = Encoding.Default;
//                //ExtractOptions extractOptions = ifOverWrite ? ExtractOptions.Overwrite : ExtractOptions.None;
//                ExtractionOptions extractionOptions = new ExtractionOptions { ExtractFullPath = true, Overwrite = ifOverWrite };
//                var reader = ReaderFactory.Open(stream);
//                while (reader.MoveToNextEntry())
//                {
//                    if (!reader.Entry.IsDirectory)
//                    {
//                        //reader.WriteEntryToDirectory(targetDirFullPath, ExtractOptions.ExtractFullPath | extractOptions);
//                        reader.WriteEntryToDirectory(targetDirFullPath, extractionOptions);
//                    }
//                }

//                //using (var reader = ReaderFactory.Open(stream))
//                //{
//                //    reader.WriteEntryToDirectory(targetDirFullPath, extractOptions);
//                //}

//            }

//        }


//        //using (ZipFile zip = ZipFile.Read(args[0], options))
//        //{
//        //    // This call to ExtractAll() assumes:
//        //    //   - none of the entries are password-protected.
//        //    //   - want to extract all entries to current working directory
//        //    //   - none of the files in the zip already exist in the directory;
//        //    //     if they do, the method will throw.
//        //    zip.ExtractAll(args[1]);
//        //}

//        //using (ZipFile zip = ZipFile.Read("D:\\test\\2007.zip"))
//        //{
//        //    foreach (ZipEntry e in zip)
//        //    {
//        //        Console.WriteLine("file name:{0}", e.FileName);
//        //        Console.WriteLine(e.Comment);
//        //        e.Extract("D:\\test\\pwdata", true);  // overwrite == true
//        //    }
//        //}



//        /// <summary>
//        /// 检查文件是否是 有效的 zip 压缩包文件
//        /// </summary>
//        /// <param name="zipFileFullPath"></param>
//        /// <returns></returns>
//        public static bool IfIsZipFile(string zipFileFullPath)
//        {
//            return ZipFile.IsZipFile(zipFileFullPath);
//        }

//        /// <summary>
//        /// 读取Zip文件内 的 文件 结构 摘要信息列表
//        /// </summary>
//        /// <param name="zipFileFullPath">Zip文件全路径</param>
//        /// <returns></returns>
//        public static List<ZipFileDigestInfoModel> ReadZipFileDigestList(string zipFileFullPath)
//        {

//            List<ZipFileDigestInfoModel> result = new List<ZipFileDigestInfoModel>();

//            using (ZipFile zip = ZipFile.Read(zipFileFullPath, new ReadOptions { Encoding = Encoding.Default }))
//            {
//                foreach (ZipEntry zipEntry in zip)
//                {
//                    GetZipFileDigestInfoModel(zipEntry.FileName, result);
//                }
//            }

//            return result;
//        }

//        private static void GetZipFileDigestInfoModel(string fileFullPath, List<ZipFileDigestInfoModel> sourceList)
//        {
//            const string splitChar = "/";
//            if (fileFullPath.IfIsNullOrEmpty()) return;
//            string name = fileFullPath.LeftSubString(splitChar);
//            if (name.IfIsNullOrEmpty())
//            {
//                var zipFileDigestInfoModel = sourceList.Where(o => o.Header == fileFullPath).FirstOrDefault();
//                if (zipFileDigestInfoModel.IfIsNullOrEmpty())
//                {
//                    sourceList.Add(new ZipFileDigestInfoModel { Header = fileFullPath, IsDirectory = false });
//                }
//            }
//            else
//            {

//                var zipFileDigestInfoModel = sourceList.Where(o => o.Header == name).FirstOrDefault();
//                if (zipFileDigestInfoModel.IfIsNullOrEmpty())
//                {
//                    zipFileDigestInfoModel = new ZipFileDigestInfoModel { Header = name, IsDirectory = true };
//                    sourceList.Add(zipFileDigestInfoModel);
//                }

//                GetZipFileDigestInfoModel(fileFullPath.LeftRemoveString(splitChar), zipFileDigestInfoModel.Children);

//            }
//        }


//    }


//}
