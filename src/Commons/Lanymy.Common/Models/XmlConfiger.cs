using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Lanymy.Common.Interfaces.IConfigs;

namespace Lanymy.Common.Models
{
    /// <summary>
    /// Xml Config 配置表 操作器
    /// </summary>
    public class XmlConfiger : IXmlConfig
    {

        /// <summary>
        /// appSettings 元素节点名称
        /// </summary>
        public const string APP_SETTINGS_XML_ELEMENT_NAME = "appSettings";
        /// <summary>
        /// appSettings 元素 子节点 名称
        /// </summary>
        public const string APP_SETTINGS_CHILD_XML_ELEMENT_NAME = "add";
        /// <summary>
        /// appSettings 元素 子节点 key 特性 名称
        /// </summary>
        public const string APP_SETTINGS_CHILD_XML_ELEMENT_KEY_ATTRIBUTE_NAME = "key";
        /// <summary>
        /// appSettings 元素 子节点 value 特性 名称
        /// </summary>
        public const string APP_SETTINGS_CHILD_XML_ELEMENT_VALUE_ATTRIBUTE_NAME = "value";




        /// <summary>
        /// 获取Config Xml 文档
        /// </summary>
        /// <param name="configFileFullPath">Config Xml 文件 全路径</param>
        /// <returns></returns>
        public virtual XDocument GetXmlDocument(string configFileFullPath)
        {
            if (!File.Exists(configFileFullPath))
                throw new FileNotFoundException(configFileFullPath);

            return XDocument.Load(configFileFullPath);
        }

        /// <summary>
        /// 获取Xml元素集合
        /// </summary>
        /// <param name="xDocument">Xml文档对象</param>
        /// <param name="parentElementName">要获取Xml元素的 父元素 标记 名称</param>
        /// <param name="currentElementName">要获取Xml元素的 标记 名称</param>
        /// <returns></returns>
        public virtual IEnumerable<XElement> GetXmlElements(XDocument xDocument, string parentElementName, string currentElementName)
        {
            return xDocument.Descendants(currentElementName).Where(o => o.Parent != null && o.Parent.Name.LocalName == parentElementName);
        }

        /// <summary>
        /// 获取Xml元素集合
        /// </summary>
        /// <param name="xmlElements">当前Xml元素集合</param>
        /// <param name="xmlElementName">要匹配的元素标记名称</param>
        /// <returns></returns>
        public virtual IEnumerable<XElement> GetXmlElements(IEnumerable<XElement> xmlElements, string xmlElementName)
        {
            return xmlElements.Descendants(xmlElementName);
        }
        /// <summary>
        /// 获取Xml元素集合
        /// </summary>
        /// <param name="xmlElements">当前Xml元素集合</param>
        /// <param name="xmlElementName">要匹配的元素标记名称</param>
        /// <param name="xmlAttributeName">要匹配元素的 特性 筛选 名称</param>
        /// <param name="xmlAttributeValue">要匹配元素的 特性 筛选 内容值 </param>
        /// <returns></returns>
        public virtual IEnumerable<XElement> GetXmlElements(IEnumerable<XElement> xmlElements, string xmlElementName, string xmlAttributeName, string xmlAttributeValue)
        {
            return GetXmlElements(xmlElements, xmlElementName).Where(o => o.Attribute(xmlAttributeName)?.Value == xmlAttributeValue);
        }
        /// <summary>
        /// 获取Xml元素
        /// </summary>
        /// <param name="xmlElements">当前Xml元素集合</param>
        /// <param name="xmlElementName">要匹配的元素标记名称</param>
        /// <returns></returns>
        public virtual XElement GetXmlElement(IEnumerable<XElement> xmlElements, string xmlElementName)
        {
            return GetXmlElements(xmlElements, xmlElementName).FirstOrDefault();
        }
        /// <summary>
        /// 获取Xml元素
        /// </summary>
        /// <param name="xmlElements">当前Xml元素集合</param>
        /// <param name="xmlElementName">要匹配的元素标记名称</param>
        /// <param name="xmlAttributeName">要匹配元素的 特性 筛选 名称</param>
        /// <param name="xmlAttributeValue">要匹配元素的 特性 筛选 内容值 </param>
        /// <returns></returns>
        public virtual XElement GetXmlElement(IEnumerable<XElement> xmlElements, string xmlElementName, string xmlAttributeName, string xmlAttributeValue)
        {
            return GetXmlElements(xmlElements, xmlElementName, xmlAttributeName, xmlAttributeValue).FirstOrDefault();
        }

        /// <summary>
        /// 获取Xml元素的内容值
        /// </summary>
        /// <param name="xmlElement">当前Xml元素</param>
        /// <param name="ifTrim">返回的内容值 是否 Trim</param>
        /// <returns></returns>
        public virtual string GetXmlElementValue(XElement xmlElement, bool ifTrim = true)
        {
            return ifTrim ? xmlElement?.Value.Trim() : xmlElement?.Value;
        }

        /// <summary>
        /// 获取Xml元素的特性值
        /// </summary>
        /// <param name="xmlElement">当前Xml元素</param>
        /// <param name="xmlAttributeName">要获取的 特性值 名称</param>
        /// <returns></returns>
        public virtual string GetXmlAttributeValue(XElement xmlElement, string xmlAttributeName)
        {
            return xmlElement.Attribute(xmlAttributeName)?.Value;
        }

        /// <summary>
        /// 获取 AppSettings 配置 节点
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        public virtual XElement GetAppSettingsXmlElement(XDocument xmlDocument)
        {
            return xmlDocument.Descendants(APP_SETTINGS_XML_ELEMENT_NAME).FirstOrDefault();
        }

        /// <summary>
        /// 获取 AppSettings 配置 节点 根据 Key 所 对应的值
        /// </summary>
        /// <param name="appSettingsXmlElement">AppSettings 配置 节点</param>
        /// <param name="keyName">Key名称</param>
        /// <returns></returns>
        public virtual string GetAppSettingsValueByKey(XElement appSettingsXmlElement, string keyName)
        {
            return appSettingsXmlElement.Descendants(APP_SETTINGS_CHILD_XML_ELEMENT_NAME).Where(o => o.Attribute(APP_SETTINGS_CHILD_XML_ELEMENT_KEY_ATTRIBUTE_NAME)?.Value == keyName).FirstOrDefault()?.Attribute("value")?.Value;
        }

    }
}
