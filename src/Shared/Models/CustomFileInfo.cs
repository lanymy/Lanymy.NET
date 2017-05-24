/********************************************************************

时间: 2016年11月09日, PM 06:58:20

作者: lanyanmiyu@qq.com

描述: 文件扩展类

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.ExtensionFunctions;

namespace Lanymy.General.Extension.Models
{


    /// <summary>
    /// 文件扩展类
    /// </summary>
    public class CustomFileInfo
    {

        /// <summary>
        /// 文件全路径
        /// </summary>
        public string FileFullPath { get; private set; }

        private FileInfo _FileInfo;

        /// <summary>
        /// 返回FileInfo类
        /// </summary>
        public FileInfo FileInfo
        {
            get
            {
                if (_FileInfo.IfIsNullOrEmpty())
                {
                    _FileInfo = new FileInfo(FileFullPath);
                }

                _FileInfo.Refresh();

                return _FileInfo;
            }
        }


        private FileVersionInfo _FileVersionInfo;

        /// <summary>
        /// 返回FileVersionInfo类
        /// </summary>
        public FileVersionInfo FileVersionInfo
        {
            get
            {
                if (_FileVersionInfo.IfIsNullOrEmpty())
                {
                    if (File.Exists(FileFullPath))
                    {
                        _FileVersionInfo = FileVersionInfo.GetVersionInfo(FileFullPath);
                    }
                }

                return _FileVersionInfo;
            }
        }

        /// <summary>
        /// 获取文件版本号
        /// </summary>
        public string FileVersionString
        {
            get
            {
                return FileVersionInfo.IfIsNullOrEmpty() ? string.Empty : FileVersionInfo.FileVersion;
            }
        }


        /// <summary>
        /// 获取文件版本号
        /// </summary>
        public Version FileVersion
        {
            get
            {
                return FileVersionString.IfIsNullOrEmpty() ? null : new Version(FileVersionString);
            }
        }

        /// <summary>
        /// 文件扩展类 构造方法
        /// </summary>
        /// <param name="fileFullPath">文件全路径</param>
        public CustomFileInfo(string fileFullPath)
        {
            FileFullPath = fileFullPath;
        }


        /// <summary>
        /// 获取CustomFileInfo实例
        /// </summary>
        /// <param name="fileFullPath">文件全路径</param>
        /// <returns></returns>
        public static CustomFileInfo GetCustomFileInfo(string fileFullPath)
        {
            return new CustomFileInfo(fileFullPath);
        }


    }


}
