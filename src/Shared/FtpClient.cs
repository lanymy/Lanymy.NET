///********************************************************************

//时间: 2016年02月22日, PM 02:59:25

//作者: lanyanmiyu@qq.com

//描述: FTP操作类

//其它:     

//********************************************************************/



//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using SNFtp = System.Net.FtpClient;
//using Lanymy.General.Extension.ExtensionFunctions;
//using System.Net;
//using System.IO;
//using Lanymy.General.Extension.Models;


//namespace Lanymy.General.Extension
//{

//    /// <summary>
//    /// FTP操作类
//    /// </summary>
//    public class FtpClient : IDisposable
//    {

//        private SNFtp.FtpClient _FtpClient = new SNFtp.FtpClient();
//        private string _FtpServer;
//        //private string _FtpUserName;
//        //private string _FtpUserPassword;
//        //private int _FtpServerPort;
//        /// <summary>
//        /// 数据缓冲区大小
//        /// </summary>
//        private const int BUFFER_SIZE = 8 * 1024;

//        public void Dispose()
//        {
//            if (!_FtpClient.IfIsNullOrEmpty())
//            {
//                _FtpClient.Dispose();
//                _FtpClient = null;
//            }
//        }

//        /// <summary>
//        /// 初始化FTP操作类
//        /// </summary>
//        /// <param name="ftpServer">服务器地址</param>
//        /// <param name="ftpUserName">用户名</param>
//        /// <param name="ftpUserPassword">密码</param>
//        /// <param name="ftpServerPort">端口</param>
//        public FtpClient(string ftpServer, string ftpUserName, string ftpUserPassword, int ftpServerPort = 21)
//        {

//            if (ftpServer.IfIsNullOrEmpty())
//            {
//                throw new ArgumentNullException("ftpServer");
//            }

//            if (ftpUserName.IfIsNullOrEmpty())
//            {
//                throw new ArgumentNullException("ftpUserName");
//            }

//            if (ftpUserPassword.IfIsNullOrEmpty())
//            {
//                throw new ArgumentNullException("ftpUserPassword");
//            }

//            _FtpServer = ftpServer;
//            //_FtpUserName = ftpUserName;
//            //_FtpUserPassword = ftpUserPassword;
//            //_FtpServerPort = ftpServerPort;

//            _FtpClient = new SNFtp.FtpClient
//            {
//                Host = _FtpServer,
//                Credentials = new NetworkCredential(ftpUserName, ftpUserPassword),
//                Port = ftpServerPort
//            };

//            //_FtpClient.Connect();
//        }


//        private void OpenConnect()
//        {
//            if (!_FtpClient.IsConnected)
//            {

//                //_FtpClient.Host = _FtpServer;
//                //_FtpClient.Credentials = new NetworkCredential(_FtpUserName, _FtpUserPassword);
//                //_FtpClient.Port = _FtpServerPort;
//                _FtpClient.Connect();
//            }
//        }


//        /// <summary>
//        /// 根据FTP 服务器 目录全路径 获取此目录下当前层 的所有 文件 或 文件夹 名称集合 (不包含子目录) "/" 表示 FTP服务器根路径 默认值 "/" 
//        /// </summary>
//        /// <param name="ftpFolderFullPath"></param>
//        /// <returns></returns>
//        public List<string> GetFtpServerNamesList(string ftpFolderFullPath = "/")
//        {
//            OpenConnect();
//            List<string> list = new List<string>();
//            var ftpList = _FtpClient.GetNameListing(ftpFolderFullPath);
//            if (!ftpList.IfIsNullOrEmpty())
//            {
//                list.AddRange(ftpList);
//            }

//            return list;
//        }

//        public bool FtpDirectoryExists(string ftpFullPath)
//        {
//            OpenConnect();
//            return _FtpClient.DirectoryExists(ftpFullPath);
//        }


//        public bool FtpFileExists(string ftpFileFullPath)
//        {
//            OpenConnect();
//            return _FtpClient.FileExists(ftpFileFullPath);
//        }


//        /// <summary>
//        /// 单文件 或 目录 下载
//        /// </summary>
//        /// <param name="ftpLocalFileFullPath">本地保存文件全路径</param>
//        /// <param name="ftpSourceFullPath">FTP服务器 文件 或 文件夹 全路径 (FTP服务器路径格式 如 "/" 根目录  ; "/Dir1/" 为FTP服务器Dir1文件夹路径 ; "/Dir1/1.txt" 为FTP服务器 Dir1 文件夹下 1.txt" 文件全路径)</param>
//        /// <param name="searchPatterns">通配符</param>
//        public void DownloadFileFromFtp(string ftpLocalFileFullPath, string ftpSourceFullPath, params string[] searchPatterns)
//        {

//            if (ftpLocalFileFullPath.IfIsNullOrEmpty())
//            {
//                throw new ArgumentNullException("ftpLocalFileFullPath");
//            }

//            if (ftpSourceFullPath.IfIsNullOrEmpty())
//            {
//                throw new ArgumentNullException("ftpSourceFullPath");
//            }

//            if (!PathFunctions.IsAbsolutePath(ftpLocalFileFullPath))
//            {
//                throw new ArgumentException(string.Format("本地文件路径 [ {0} ] 不是有效的绝对路径", ftpLocalFileFullPath));
//            }


//            if (!PathFunctions.IsRelativePath(ftpSourceFullPath))
//            {
//                throw new ArgumentException(string.Format("FTP服务器路径 [ {0} ] 不是有效的相对路径", ftpSourceFullPath));
//            }

//            if (!ftpSourceFullPath.StartsWith("/"))
//            {
//                ftpSourceFullPath = "/" + ftpSourceFullPath;
//            }

//            if (PathFunctions.GetPathType(ftpSourceFullPath) == PathTypeEnum.Directory)
//            {
//                DownloadFilesFromFtp(new List<FtpPathInfoModel> { new FtpPathInfoModel { FtpFullPath = ftpSourceFullPath, LocalFullPath = ftpLocalFileFullPath } }, searchPatterns);
//                return;
//            }

//            if (PathFunctions.GetPathType(ftpSourceFullPath) != Models.PathTypeEnum.File)
//            {
//                throw new ArgumentException(string.Format("FTP服务器 [ {0} ] 文件路径 [ {1} ] 不是有效的文件路径", _FtpServer, ftpSourceFullPath));
//            }

//            if (!PathFunctions.CheckFileWithWildcard(ftpSourceFullPath, searchPatterns))
//            {
//                return;
//            }

//            OpenConnect();

//            //if (!_FtpClient.FileExists(ftpSourceFileFullPath))
//            //{
//            //    throw new FileNotFoundException(string.Format("FTP服务器 [ {0} ] 文件路径 [ {1} ] 文件不存在", _FtpServer, ftpSourceFileFullPath));
//            //}

//            if (PathFunctions.GetPathType(ftpLocalFileFullPath) == PathTypeEnum.Directory)
//            {
//                ftpLocalFileFullPath = Path.Combine(ftpLocalFileFullPath, Path.GetFileName(ftpSourceFullPath));
//            }

//            using (var ftpStream = _FtpClient.OpenRead(ftpSourceFullPath))
//            {
//                PathFunctions.InitDirectoryPath(ftpLocalFileFullPath);
//                using (var fileStream = File.Create(ftpLocalFileFullPath))
//                {
//                    //var buffer = new byte[8 * 1024];
//                    //int count;
//                    //while ((count = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
//                    //{
//                    //    fileStream.Write(buffer, 0, count);
//                    //}
//                    ftpStream.CopyTo(fileStream, BUFFER_SIZE);

//                }
//            }

//        }



//        /// <summary>
//        /// 下载多个文件  /  文件夹
//        /// </summary>
//        /// <param name="pathInfoList">下载路径信息列表</param>
//        /// <param name="searchPatterns">文件匹配通配符</param>
//        public void DownloadFilesFromFtp(List<FtpPathInfoModel> pathInfoList, params string[] searchPatterns)
//        {

//            if (pathInfoList.IfIsNullOrEmpty())
//            {
//                //throw new ArgumentNullException("pathInfoList");
//                return;
//            }



//            OpenConnect();


//            foreach (var ftpPathInfoModel in pathInfoList)
//            {

//                if (PathFunctions.GetPathType(ftpPathInfoModel.FtpFullPath) == PathTypeEnum.File)
//                {

//                    DownloadFileFromFtp(ftpPathInfoModel.LocalFullPath, ftpPathInfoModel.FtpFullPath, searchPatterns);


//                    //if (PathFunctions.GetPathType(ftpPathInfoModel.LocalFullPath) == PathTypeEnum.Directory)
//                    //{
//                    //    DownloadFileFromFtp(Path.Combine((new[] { ftpPathInfoModel.LocalFullPath }).Union(ftpPathInfoModel.FtpFullPath.Split('/')).ToArray()), ftpPathInfoModel.FtpFullPath);
//                    //}
//                    //else if (PathFunctions.GetPathType(ftpPathInfoModel.LocalFullPath) == PathTypeEnum.File)
//                    //{
//                    //    DownloadFileFromFtp(ftpPathInfoModel.LocalFullPath, ftpPathInfoModel.FtpFullPath);
//                    //}
//                }
//                else if (PathFunctions.GetPathType(ftpPathInfoModel.FtpFullPath) == PathTypeEnum.Directory)
//                {

//                    if (PathFunctions.GetPathType(ftpPathInfoModel.LocalFullPath) == PathTypeEnum.File)
//                    {
//                        ftpPathInfoModel.LocalFullPath = PathFunctions.GetFolderPath(ftpPathInfoModel.LocalFullPath);
//                    }

//                    if (ftpPathInfoModel.FtpFullPath.EndsWith("/"))
//                    {
//                        ftpPathInfoModel.FtpFullPath = ftpPathInfoModel.FtpFullPath.RightRemoveString(1);
//                    }

//                    ftpPathInfoModel.LocalFullPath = Path.Combine(ftpPathInfoModel.LocalFullPath, ftpPathInfoModel.FtpFullPath.RightSubString("/"));

//                    DownloadFilesFromFtp(GetFtpServerNamesList(ftpPathInfoModel.FtpFullPath).Select(ftpName => new FtpPathInfoModel { LocalFullPath = ftpPathInfoModel.LocalFullPath, FtpFullPath = ftpName }).ToList(), searchPatterns);
//                }



//            }

//        }


//        /// <summary>
//        /// 单文件 或 目录 上传 到FTP服务器
//        /// </summary>
//        /// <param name="ftpLocalFullPath">本地文件 或 目录 全路径</param>
//        /// <param name="ftpSourceFullPath">FTP服务器 文件 或 文件夹 全路径 (FTP服务器路径格式 如 "/" 根目录  ; "/Dir1/" 为FTP服务器Dir1文件夹路径 ; "/Dir1/1.txt" 为FTP服务器 Dir1 文件夹下 1.txt" 文件全路径)</param>
//        /// <param name="searchPatterns">通配符</param>
//        public void UploadFileToFtp(string ftpLocalFullPath, string ftpSourceFullPath, params string[] searchPatterns)
//        {

//            if (ftpLocalFullPath.IfIsNullOrEmpty())
//            {
//                throw new ArgumentNullException("ftpLocalFullPath");
//            }

//            if (ftpSourceFullPath.IfIsNullOrEmpty())
//            {
//                throw new ArgumentNullException("ftpSourceFullPath");
//            }

//            if (!PathFunctions.IsAbsolutePath(ftpLocalFullPath))
//            {
//                throw new ArgumentException(string.Format("本地路径 [ {0} ] 不是有效的绝对路径", ftpLocalFullPath));
//            }


//            if (!PathFunctions.IsRelativePath(ftpSourceFullPath))
//            {
//                throw new ArgumentException(string.Format("FTP服务器路径 [ {0} ] 不是有效的相对路径", ftpSourceFullPath));
//            }

//            if (!ftpSourceFullPath.StartsWith("/"))
//            {
//                ftpSourceFullPath = "/" + ftpSourceFullPath;
//            }

//            if (PathFunctions.GetPathType(ftpLocalFullPath) == PathTypeEnum.Directory)
//            {
//                UploadFilesToFtp(new List<FtpPathInfoModel> { new FtpPathInfoModel { FtpFullPath = ftpSourceFullPath, LocalFullPath = ftpLocalFullPath } }, searchPatterns);
//                return;
//            }

//            if (PathFunctions.GetPathType(ftpLocalFullPath) != Models.PathTypeEnum.File)
//            {
//                throw new ArgumentException(string.Format("本地路径 [ {0} ] 不是有效的文件路径", ftpLocalFullPath));
//            }

//            if (!PathFunctions.CheckFileWithWildcard(ftpLocalFullPath, searchPatterns))
//            {
//                return;
//            }


//            //if (!_FtpClient.FileExists(ftpSourceFileFullPath))
//            //{
//            //    throw new FileNotFoundException(string.Format("FTP服务器 [ {0} ] 文件路径 [ {1} ] 文件不存在", _FtpServer, ftpSourceFileFullPath));
//            //}

//            if (PathFunctions.GetPathType(ftpSourceFullPath) == PathTypeEnum.Directory)
//            {
//                ftpSourceFullPath = PathFunctions.CombineRelativePath(ftpSourceFullPath, Path.GetFileName(ftpLocalFullPath));
//            }


//            OpenConnect();

//            string strDirPath = ftpSourceFullPath.RightRemoveString("/", false);

//            if (!_FtpClient.DirectoryExists(strDirPath))
//            {
//                _FtpClient.CreateDirectory(strDirPath);
//            }

//            using (var ftpStream = _FtpClient.OpenWrite(ftpSourceFullPath))
//            {

//                using (var fileStream = File.OpenRead(ftpLocalFullPath))
//                {
//                    //var buffer = new byte[8 * 1024];
//                    //int count;
//                    //while ((count = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
//                    //{
//                    //    fileStream.Write(buffer, 0, count);
//                    //}
//                    fileStream.CopyTo(ftpStream, BUFFER_SIZE);

//                }

//            }






//        }



//        /// <summary>
//        /// 上传多个文件  /  文件夹 到FTP服务器
//        /// </summary>
//        /// <param name="pathInfoList">上传路径列表信息</param>
//        /// <param name="searchPatterns">文件匹配通配符</param>
//        public void UploadFilesToFtp(List<FtpPathInfoModel> pathInfoList, params string[] searchPatterns)
//        {

//            if (pathInfoList.IfIsNullOrEmpty())
//            {
//                //throw new ArgumentNullException("pathInfoList");
//                return;
//            }

//            OpenConnect();

//            foreach (var ftpPathInfoModel in pathInfoList)
//            {

//                if (PathFunctions.GetPathType(ftpPathInfoModel.LocalFullPath) == PathTypeEnum.File)
//                {

//                    UploadFileToFtp(ftpPathInfoModel.LocalFullPath, ftpPathInfoModel.FtpFullPath, searchPatterns);


//                    //if (PathFunctions.GetPathType(ftpPathInfoModel.LocalFullPath) == PathTypeEnum.Directory)
//                    //{
//                    //    DownloadFileFromFtp(Path.Combine((new[] { ftpPathInfoModel.LocalFullPath }).Union(ftpPathInfoModel.FtpFullPath.Split('/')).ToArray()), ftpPathInfoModel.FtpFullPath);
//                    //}
//                    //else if (PathFunctions.GetPathType(ftpPathInfoModel.LocalFullPath) == PathTypeEnum.File)
//                    //{
//                    //    DownloadFileFromFtp(ftpPathInfoModel.LocalFullPath, ftpPathInfoModel.FtpFullPath);
//                    //}
//                }
//                else if (PathFunctions.GetPathType(ftpPathInfoModel.LocalFullPath) == PathTypeEnum.Directory)
//                {



//                    if (PathFunctions.GetPathType(ftpPathInfoModel.FtpFullPath) == PathTypeEnum.File)
//                    {
//                        ftpPathInfoModel.FtpFullPath = Path.GetDirectoryName(ftpPathInfoModel.FtpFullPath).Replace("\\", "/");
//                    }

//                    if (!ftpPathInfoModel.FtpFullPath.EndsWith("/"))
//                    {
//                        ftpPathInfoModel.FtpFullPath += "/";
//                    }


//                    if (ftpPathInfoModel.LocalFullPath.EndsWith("\\"))
//                    {
//                        ftpPathInfoModel.LocalFullPath = ftpPathInfoModel.LocalFullPath.RightRemoveString(1);
//                    }

//                    ftpPathInfoModel.FtpFullPath = PathFunctions.CombineRelativePath(ftpPathInfoModel.FtpFullPath, ftpPathInfoModel.LocalFullPath.RightSubString("\\"));

//                    UploadFilesToFtp(Directory.GetDirectories(ftpPathInfoModel.LocalFullPath).Union(Directory.GetFiles(ftpPathInfoModel.LocalFullPath)).Select(localName => new FtpPathInfoModel { LocalFullPath = localName, FtpFullPath = ftpPathInfoModel.FtpFullPath }).ToList(), searchPatterns);

//                }



//            }


//        }




//    }



//}
