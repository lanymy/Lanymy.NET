/********************************************************************

时间: 2016年02月24日, PM 02:41:29

作者: lanyanmiyu@qq.com

描述: FTP路径信息数据实体类

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.ExtensionFunctions;

namespace Lanymy.General.Extension.Models
{

    /// <summary>
    /// FTP路径信息数据实体类
    /// </summary>
    public class FtpPathInfoModel
    {

        /// <summary>
        /// 本地映射路径信息
        /// </summary>
        public string LocalFullPath { get; set; }

        private string _FtpFullPath;

        /// <summary>
        /// FTP服务器映射路径信息
        /// <para>FTP服务器路径格式 如 "/" 根目录  ; "/Dir1/" 为FTP服务器Dir1文件夹路径 ; "/Dir1/1.txt" 为FTP服务器 Dir1 文件夹下 1.txt" 文件全路径</para>
        /// </summary>
        public string FtpFullPath
        {
            set { _FtpFullPath = value; }
            get
            {
                if (!_FtpFullPath.IfIsNullOrEmpty() && !_FtpFullPath.StartsWith("/"))
                {
                    _FtpFullPath = "/" + _FtpFullPath;
                }
                return _FtpFullPath;
            }
        }


    }


}
