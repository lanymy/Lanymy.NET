/********************************************************************

时间: 2015年03月06日, PM 03:59:32

作者: lanyanmiyu@qq.com

描述: 配置文件辅助类

其它:     

********************************************************************/




using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Lanymy.General.Extension.ExtensionFunctions;



namespace Lanymy.General.Extension
{


    /// <summary>
    /// 配置文件辅助类
    /// </summary>
    public class ConfigFunctions
    {


        /// <summary>
        /// 获取Config Xml 文档
        /// </summary>
        /// <param name="configFileFullPath">Config Xml 文件 全路径</param>
        /// <returns></returns>
        public static XDocument GetConfigXmlDocument(string configFileFullPath)
        {
            if (!File.Exists(configFileFullPath))
            {
                throw new FileNotFoundException(configFileFullPath);
            }

            return XDocument.Load(configFileFullPath);
        }


        /// <summary>
        /// 获取Config文件 AppSettings 节点的 key 值 对应的 value 值
        /// </summary>
        /// <param name="doc">Config XML 文档</param>
        /// <param name="key">键名称</param>
        /// <returns></returns>
        public static string GetAppSettingsValueByKey(XDocument doc, string key)
        {

            if (doc.IfIsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(doc));
            }
            if (key.IfIsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(key));
            }

            XElement root = doc.Descendants("appSettings").FirstOrDefault();

            if (root.IfIsNullOrEmpty())
            {
                throw new Exception("没有appSettings节点");
            }

            XElement appElement = root.Descendants("add").Where(o => o.Attribute("key").Value == key).FirstOrDefault();
            if (appElement.IfIsNullOrEmpty()) return string.Format("没有找到key 为 {0} 的节点", key);

            return appElement.Attribute("value").Value;

        }


        /// <summary>
        /// 异步 获取Config文件 AppSettings 节点的 key 值 对应的 value 值
        /// </summary>
        /// <param name="doc">Config XML 文档</param>
        /// <param name="key">键名称</param>
        /// <returns></returns>
        public static Task<string> GetAppSettingsValueByKeyAsync(XDocument doc, string key)
        {
#if NET40
            return new Task<string>(() => GetAppSettingsValueByKey(doc, key));
#else
            return Task.FromResult(GetAppSettingsValueByKey(doc,key));
#endif
        }


        /// <summary>
        /// 获取Config文件 AppSettings 节点的 key 值 对应的 value 值
        /// </summary>
        /// <param name="key">键名称</param>
        /// <param name="configFileFullPath">Config Xml 文件 全路径</param>
        /// <returns></returns>
        public static string GetAppSettingsValueByKey(string key, string configFileFullPath)
        {
            if (key.IfIsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(key));
            }

            return GetAppSettingsValueByKey(GetConfigXmlDocument(configFileFullPath), key);
        }

        /// <summary>
        /// 异步 获取Config文件 AppSettings 节点的 key 值 对应的 value 值
        /// </summary>
        /// <param name="key">键名称</param>
        /// <param name="configFileFullPath">Config Xml 文件 全路径</param>
        /// <returns></returns>
        public static Task<string> GetAppSettingsValueByKeyAsync(string key, string configFileFullPath)
        {
#if NET40
            return new Task<string>(() => GetAppSettingsValueByKey(key, configFileFullPath));
#else
            return Task.FromResult(GetAppSettingsValueByKey(key, configFileFullPath));
#endif
        }

    }


}
