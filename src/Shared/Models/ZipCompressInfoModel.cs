/********************************************************************

时间: 2016年02月23日, AM 09:01:14

作者: lanyanmiyu@qq.com

描述: 压缩信息描述实体类

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.ExtensionFunctions;
using System.IO;

namespace Lanymy.General.Extension.Models
{


    /// <summary>
    /// 压缩信息描述实体类
    /// </summary>
    public class ZipCompressInfoModel
    {

        /// <summary>
        /// 要压缩的数据源全路径描述 ( 文件夹 / 文件 ) 
        /// </summary>
        public string SourceFullPath { get; set; }

        /// <summary>
        /// 压缩包内目标全路径 "/" 表示压缩包根目录
        /// </summary>
        public string TargetFullPath { get; set; }

        //private string _TargetFullPath = string.Empty;

        ///// <summary>
        ///// 压缩包内目标全路径 
        ///// <para>( 默认值 空 为 zip压缩包中的根目录 ; "/aaa/" 为 zip压缩包中根目录下的 "aaa"文件夹 路径描述 ; "/aaa/1.txt" 为 zip压缩包中根目录下的 "aaa" 文件夹 下的 "1.txt" 文件全路径描述 )</para>
        ///// <para>如果没有目标文件名描述 并且 SourceFullPath 描述中 有文件名描述时 默认 使用 SourceFullPath 的文件名</para>
        ///// </summary>
        //public string TargetFullPath
        //{

        //    set
        //    {
        //        _TargetFullPath = value;
        //    }
        //    get
        //    {
        //        if (PathFunctions.GetPathType(_TargetFullPath) == PathTypeEnum.Directory && !SourceFullPath.IfIsNullOrEmpty() && PathFunctions.GetPathType(SourceFullPath) == PathTypeEnum.File)
        //        {
        //            _TargetFullPath = PathFunctions.CombineRelativePath(_TargetFullPath, Path.GetFileName(SourceFullPath));
        //        }

        //        return _TargetFullPath;
        //    }

        //}


    }

}
