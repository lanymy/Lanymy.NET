/********************************************************************

时间: 2017年06月14日, AM 07:44:25

作者: lanyanmiyu@qq.com

描述: 独立存储 基类

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Interfaces;

namespace Lanymy.General.Extension.Instruments
{

    /// <summary>
    /// 独立存储 基类
    /// </summary>
    public abstract class BaseIsolatedStorage : IIsolatedStorage
    {

        /// <summary>
        /// 是否是 自定义 存储区 模式
        /// </summary>
        public bool IfIsCustomIsolatedStorageMode { get; } = false;

        private string _CustomIsolatedStorageRootDirectoryFullPath;

        /// <summary>
        /// 自定义独立存储区 跟目录 全路径
        /// </summary>
        protected string CustomIsolatedStorageRootDirectoryFullPath
        {
            get
            {
                if (_CustomIsolatedStorageRootDirectoryFullPath.IfIsNullOrEmpty())
                {
                    _CustomIsolatedStorageRootDirectoryFullPath = GetDefaultCustomIsolatedStorageRootDirectoryFullPath();
                    PathFunctions.InitDirectoryPath(_CustomIsolatedStorageRootDirectoryFullPath);
                }
                return _CustomIsolatedStorageRootDirectoryFullPath;
            }
        }

        /// <summary>
        /// 独立存储 基类 构造方法
        /// </summary>
        /// <param name="customIsolatedStorageRootDirectoryFullPath">自定义独立存储区 根目录 全路径 ; null 则使用托管的 独立存储区; 不为null 则使用 自定义独立存储区</param>
        protected BaseIsolatedStorage(string customIsolatedStorageRootDirectoryFullPath = null)
        {
            if (!customIsolatedStorageRootDirectoryFullPath.IfIsNullOrEmpty())
            {
                IfIsCustomIsolatedStorageMode = true;
                _CustomIsolatedStorageRootDirectoryFullPath = customIsolatedStorageRootDirectoryFullPath;
                PathFunctions.InitDirectoryPath(_CustomIsolatedStorageRootDirectoryFullPath);
            }
        }


        /// <summary>
        /// 获取默认的 自定义独立存储区 跟目录 全路径
        /// </summary>
        /// <returns></returns>
        protected abstract string GetDefaultCustomIsolatedStorageRootDirectoryFullPath();

        /// <summary>
        /// 获取 自定义独立存储区 中的 文件 全路径
        /// </summary>
        /// <param name="fileFullName">文件全名称</param>
        /// <returns></returns>
        protected virtual string GetCustomIsolatedStorageFileFullPath(string fileFullName)
        {
            return Path.Combine(CustomIsolatedStorageRootDirectoryFullPath, fileFullName);
        }

        /// <summary>
        /// 获取系统中的独立存储区
        /// </summary>
        /// <returns></returns>
        protected virtual IsolatedStorageFile GetSystemIsolatedStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 序列化 字符串 到 持久化文件
        /// </summary>
        /// <param name="sourceString">要序列化的字符串</param>
        /// <param name="token">持久化标识</param>
        /// <param name="encoding">编码 (Null表示使用 默认编码)</param>
        /// <param name="securityKey">密钥 (Null表示使用 默认密钥)</param>
        public abstract void SaveString(string sourceString, string token, string securityKey = null, Encoding encoding = null);


        /// <summary>
        /// 从持久化文件 获取 字符串
        /// </summary>
        /// <param name="token">持久化标识</param>
        /// <param name="encoding">编码 (Null表示使用 默认编码)</param>
        /// <param name="securityKey">密钥 (Null表示使用 默认密钥)</param>
        /// <returns></returns>
        public abstract string GetString(string token, string securityKey = null, Encoding encoding = null);


        /// <summary>
        /// 指定文件名 指定编码 指定密钥 保存实体类到独立存储区中
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="t">实体实例</param>
        /// <param name="token">序列化标识 (Null 表示使用 默认 标识名)</param>
        /// <param name="encoding">编码 (Null 表示使用 默认 编码)</param>
        /// <param name="securityKey">密钥 (Null 表示使用 默认 密钥)</param>
        public abstract void SaveModel<T>(T t, string token = null, string securityKey = null, Encoding encoding = null) where T : class;


        /// <summary>
        ///从独立存储区中获取实体类
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="token">序列化标识 (Null 表示使用 默认 标识名)</param>
        /// <param name="encoding">编码 (Null 表示使用 默认 编码)</param>
        /// <param name="securityKey">密钥 (Null 表示使用 默认 密钥)</param>
        /// <returns></returns>
        public abstract T GetModel<T>(string token = null, string securityKey = null, Encoding encoding = null) where T : class;



    }

}
