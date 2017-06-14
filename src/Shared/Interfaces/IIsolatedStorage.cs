/********************************************************************

时间: 2017年06月14日, AM 07:33:24

作者: lanyanmiyu@qq.com

描述: 独立存储 功能 接口

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Text;

namespace Lanymy.General.Extension.Interfaces
{

    /// <summary>
    /// 独立存储 功能 接口
    /// </summary>
    public interface IIsolatedStorage : IIsolatedStorageString, IIsolatedStorageModel
    {

        ///// <summary>
        ///// 自定义独立存储区 跟目录 全路径
        ///// </summary>
        //string CustomIsolatedStorageRootDirectoryFullPath { get; }

        ///// <summary>
        ///// 获取默认的 自定义独立存储区 跟目录 全路径
        ///// </summary>
        ///// <returns></returns>
        //string GetDefaultCustomIsolatedStorageRootDirectoryFullPath();

        ///// <summary>
        ///// 获取 自定义独立存储区 中的 文件 全路径
        ///// </summary>
        ///// <param name="fileFullName"></param>
        ///// <returns></returns>
        //string GetCustomIsolatedStorageFileFullPath(string fileFullName);

        ///// <summary>
        ///// 获取系统中的独立存储区
        ///// </summary>
        ///// <returns></returns>
        //IsolatedStorageFile GetSystemIsolatedStorage();

    }

}
