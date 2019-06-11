using System.Collections.Generic;
using System.Xml.Linq;
using Lanymy.Common.Interfaces.IConfigs;
using Lanymy.Common.Models;

namespace Lanymy.Common
{
    /// <summary>
    /// 配置文件辅助类
    /// </summary>
    public class ConfigHelper
    {

        /// <summary>
        /// 默认Xml Config配置 文件 操作器
        /// </summary>
        public static readonly IXmlConfig DefaultXmlConfig = new XmlConfiger();



        /// <summary>
        /// 获取Config Xml 文档
        /// </summary>
        /// <param name="configFileFullPath">Config Xml 文件 全路径</param>
        /// <param name="xmlConfigReader">Xml配置文件  读取  功能 接口</param>
        /// <returns></returns>
        public static XDocument GetXmlDocument(string configFileFullPath, IXmlConfigReader xmlConfigReader = null)
        {
            return GenericityHelper.GetInterface(xmlConfigReader, DefaultXmlConfig).GetXmlDocument(configFileFullPath);
        }

        /// <summary>
        /// 获取Xml元素集合
        /// </summary>
        /// <param name="xDocument">Xml文档对象</param>
        /// <param name="parentElementName">要获取Xml元素的 父元素 标记 名称</param>
        /// <param name="currentElementName">要获取Xml元素的 标记 名称</param>
        /// <param name="xmlConfigReader">Xml配置文件  读取  功能 接口</param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetXmlElements(XDocument xDocument, string parentElementName, string currentElementName, IXmlConfigReader xmlConfigReader = null)
        {
            return GenericityHelper.GetInterface(xmlConfigReader, DefaultXmlConfig).GetXmlElements(xDocument, parentElementName, currentElementName);
        }

        /// <summary>
        /// 获取Xml元素集合
        /// </summary>
        /// <param name="xmlElements">当前Xml元素集合</param>
        /// <param name="xmlElementName">要匹配的元素标记名称</param>
        /// <param name="xmlConfigReader">Xml配置文件  读取  功能 接口</param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetXmlElements(IEnumerable<XElement> xmlElements, string xmlElementName, IXmlConfigReader xmlConfigReader = null)
        {
            return GenericityHelper.GetInterface(xmlConfigReader, DefaultXmlConfig).GetXmlElements(xmlElements, xmlElementName);
        }
        /// <summary>
        /// 获取Xml元素集合
        /// </summary>
        /// <param name="xmlElements">当前Xml元素集合</param>
        /// <param name="xmlElementName">要匹配的元素标记名称</param>
        /// <param name="xmlAttributeName">要匹配元素的 特性 筛选 名称</param>
        /// <param name="xmlAttributeValue">要匹配元素的 特性 筛选 内容值 </param>
        /// <param name="xmlConfigReader">Xml配置文件  读取  功能 接口</param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetXmlElements(IEnumerable<XElement> xmlElements, string xmlElementName, string xmlAttributeName, string xmlAttributeValue, IXmlConfigReader xmlConfigReader = null)
        {
            return GenericityHelper.GetInterface(xmlConfigReader, DefaultXmlConfig).GetXmlElements(xmlElements, xmlElementName, xmlAttributeName, xmlAttributeValue);
        }
        /// <summary>
        /// 获取Xml元素
        /// </summary>
        /// <param name="xmlElements">当前Xml元素集合</param>
        /// <param name="xmlElementName">要匹配的元素标记名称</param>
        /// <param name="xmlConfigReader">Xml配置文件  读取  功能 接口</param>
        /// <returns></returns>
        public static XElement GetXmlElement(IEnumerable<XElement> xmlElements, string xmlElementName, IXmlConfigReader xmlConfigReader = null)
        {
            return GenericityHelper.GetInterface(xmlConfigReader, DefaultXmlConfig).GetXmlElement(xmlElements, xmlElementName);
        }
        /// <summary>
        /// 获取Xml元素
        /// </summary>
        /// <param name="xmlElements">当前Xml元素集合</param>
        /// <param name="xmlElementName">要匹配的元素标记名称</param>
        /// <param name="xmlAttributeName">要匹配元素的 特性 筛选 名称</param>
        /// <param name="xmlAttributeValue">要匹配元素的 特性 筛选 内容值 </param>
        /// <param name="xmlConfigReader">Xml配置文件  读取  功能 接口</param>
        /// <returns></returns>
        public static XElement GetXmlElement(IEnumerable<XElement> xmlElements, string xmlElementName, string xmlAttributeName, string xmlAttributeValue, IXmlConfigReader xmlConfigReader = null)
        {
            return GenericityHelper.GetInterface(xmlConfigReader, DefaultXmlConfig).GetXmlElement(xmlElements, xmlElementName, xmlAttributeName, xmlAttributeValue);
        }
        /// <summary>
        /// 获取Xml元素的内容值
        /// </summary>
        /// <param name="xmlElement">当前Xml元素</param>
        /// <param name="ifTrim">返回的内容值 是否 Trim</param>
        /// <param name="xmlConfigReader">Xml配置文件  读取  功能 接口</param>
        /// <returns></returns>
        public static string GetXmlElementValue(XElement xmlElement, bool ifTrim = true, IXmlConfigReader xmlConfigReader = null)
        {
            return GenericityHelper.GetInterface(xmlConfigReader, DefaultXmlConfig).GetXmlElementValue(xmlElement, ifTrim);
        }
        /// <summary>
        /// 获取Xml元素的特性值
        /// </summary>
        /// <param name="xmlElement">当前Xml元素</param>
        /// <param name="xmlAttributeName">要获取的 特性值 名称</param>
        /// <param name="xmlConfigReader">Xml配置文件  读取  功能 接口</param>
        /// <returns></returns>
        public static string GetXmlAttributeValue(XElement xmlElement, string xmlAttributeName, IXmlConfigReader xmlConfigReader = null)
        {
            return GenericityHelper.GetInterface(xmlConfigReader, DefaultXmlConfig).GetXmlAttributeValue(xmlElement, xmlAttributeName);
        }

        /// <summary>
        /// 获取 AppSettings 配置 节点
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <param name="xmlConfigAppSettingsReader">Xml Config AppSettings 配置表节点  读取 功能接口</param>
        /// <returns></returns>
        public static XElement GetAppSettingsXmlElement(XDocument xmlDocument, IXmlConfigAppSettingsReader xmlConfigAppSettingsReader = null)
        {
            return GenericityHelper.GetInterface(xmlConfigAppSettingsReader, DefaultXmlConfig).GetAppSettingsXmlElement(xmlDocument);
        }

        /// <summary>
        /// 获取 AppSettings 配置 节点 根据 Key 所 对应的值
        /// </summary>
        /// <param name="appSettingsXmlElement">AppSettings 配置 节点</param>
        /// <param name="keyName">Key名称</param>
        /// <param name="xmlConfigAppSettingsReader">Xml Config AppSettings 配置表节点  读取 功能接口</param>
        /// <returns></returns>
        public static string GetAppSettingsValueByKey(XElement appSettingsXmlElement, string keyName, IXmlConfigAppSettingsReader xmlConfigAppSettingsReader = null)
        {
            return GenericityHelper.GetInterface(xmlConfigAppSettingsReader, DefaultXmlConfig).GetAppSettingsValueByKey(appSettingsXmlElement, keyName);
        }
    }
}
