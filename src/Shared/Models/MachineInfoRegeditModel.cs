/********************************************************************

时间: 2015年04月02日, AM 09:38:34

作者: lanyanmiyu@qq.com

描述: 注册表 应用程序配置信息 实体类

其它:     

********************************************************************/



using System;

namespace Lanymy.General.Extension.Models
{
    /// <summary>
    /// 注册表 应用程序配置信息 实体类
    /// </summary>
    [Serializable]
    public class MachineInfoRegeditModel
    {

        private const string ROOT_SUBKEY = "SOFTWARE";

        /// <summary>
        /// 注册表 应用程序 根项 名称 (如:ZCAPP)
        /// </summary>
        public string ApplicationRootName { get; set; }

        /// <summary>
        /// 注册表 应用程序 子项 路径 (如:\\Config  则自动匹配出来的全路径是 HKEY_LOCAL_MACHINE\SOFTWARE\ZCAPP\Config )
        /// </summary>
        public string ChildPath { get; set; }

        /// <summary>
        /// 键值Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 键值Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        public MachineInfoRegeditModel() { }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="applicationRootName">注册表 应用程序 根项 名称 (如:ZCAPP)</param>
        /// <param name="childPath">注册表 应用程序 子项 路径 (如:\\Config  则自动匹配出来的全路径是 HKEY_LOCAL_MACHINE\SOFTWARE\ZCAPP\Config )</param>
        /// <param name="key">键值Key</param>
        /// <param name="value">键值Value</param>
        public MachineInfoRegeditModel(string applicationRootName, string childPath, string key, string value)
        {

            ApplicationRootName = applicationRootName;
            ChildPath = childPath;
            Key = key;
            Value = value;

        }

        /// <summary>
        /// 获取应用程序相对路径 (如: SOFTWARE\ZCAPP)
        /// </summary>
        /// <returns></returns>
        public string GetApplicationRootSubKeyPath()
        {

            return ROOT_SUBKEY + "\\" + ApplicationRootName;

        }


        /// <summary>
        /// 获取应用程序完全路径 (如: HKEY_LOCAL_MACHINE\SOFTWARE\ZCAPP)
        /// </summary>
        /// <returns></returns>
        public string GetApplicationRootSubKeyFullPath()
        {
            return RegeditRootEnum.HKEY_LOCAL_MACHINE + "\\" + GetApplicationRootSubKeyPath();
        }

        /// <summary>
        /// 获取子键相对路径 (如: SOFTWARE\ZCAPP\Config)
        /// </summary>
        /// <returns></returns>
        public string GetChildPath()
        {

            return GetApplicationRootSubKeyPath() + "\\" + ChildPath;

        }

        /// <summary>
        /// 获取子键完全路径 (如: HKEY_LOCAL_MACHINE\SOFTWARE\ZCAPP\Config)
        /// </summary>
        /// <returns></returns>
        public string GetChildFullPath()
        {

            return GetApplicationRootSubKeyFullPath() + "\\" + ChildPath;

        }

    }
}
