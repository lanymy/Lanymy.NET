/********************************************************************

时间: 2015年04月02日, AM 08:12:07

作者: lanyanmiyu@qq.com

描述: 注册表 文件 后缀 注册信息 实体类

其它:     

********************************************************************/




using System;

namespace Lanymy.General.Extension.Models
{

    /// <summary>
    /// 注册表 文件 后缀 注册信息 实体类
    /// 如果要操作注册表 应用程序 必须开启系统管理员权限 app.manifest 中设置
    /// </summary>
    [Serializable]
    public class FileExtensionRegeditInfoModel
    {

        /// <summary> 
        /// 文件后缀名(如: .zip)
        /// </summary> 
        public string FileExtensionName { get; set; }

        /// <summary> 
        /// 注册表中文件后缀名 说明信息  (如:XCodeFactory的项目文件)
        /// </summary> 
        public string FileExtensionDescription { get; set; }

        /// <summary> 
        /// 文件后缀关联的图标路径
        /// </summary> 
        public string IcoPath;


        /// <summary> 
        /// 关联打开文件后缀的应用程序路径 
        /// </summary> 
        public string ExePath;

        /// <summary>
        /// 构造方法
        /// </summary>
        public FileExtensionRegeditInfoModel()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="fileExtensionName">文件后缀名 (如: .zip)</param>
        public FileExtensionRegeditInfoModel(string fileExtensionName)
        {
            FileExtensionName = fileExtensionName;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="fileExtensionName">文件后缀名 (如: .zip)</param>
        /// <param name="fileExtensionDescription">文件后缀名描述</param>
        /// <param name="exePath">关联的应用程序路径</param>
        public FileExtensionRegeditInfoModel(string fileExtensionName, string fileExtensionDescription, string exePath)
        {
            ExePath = exePath;
            FileExtensionName = fileExtensionName;
            FileExtensionDescription = fileExtensionDescription;
            IcoPath = ExePath;
        }
    }
}
