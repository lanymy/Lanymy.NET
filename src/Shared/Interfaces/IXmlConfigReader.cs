/********************************************************************

时间: 2017年06月02日, PM 08:57:42

作者: lanyanmiyu@qq.com

描述: Xml配置文件  读取  功能 接口

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Lanymy.General.Extension.Interfaces
{

    /// <summary>
    /// Xml配置文件  读取  功能 接口
    /// </summary>
    public interface IXmlConfigReader
    {

        /// <summary>
        /// 获取Config Xml 文档
        /// </summary>
        /// <param name="configFileFullPath">Config Xml 文件 全路径</param>
        /// <returns></returns>
        XDocument GetXmlDocument(string configFileFullPath);

        /// <summary>
        /// 获取Xml元素集合
        /// </summary>
        /// <param name="xDocument">Xml文档对象</param>
        /// <param name="parentElementName">要获取Xml元素的 父元素 标记 名称</param>
        /// <param name="currentElementName">要获取Xml元素的 标记 名称</param>
        /// <returns></returns>
        IEnumerable<XElement> GetXmlElements(XDocument xDocument, string parentElementName, string currentElementName);


        /// <summary>
        /// 获取Xml元素集合
        /// </summary>
        /// <param name="xmlElements">当前Xml元素集合</param>
        /// <param name="xmlElementName">要匹配的元素标记名称</param>
        /// <returns></returns>
        IEnumerable<XElement> GetXmlElements(IEnumerable<XElement> xmlElements, string xmlElementName);

        /// <summary>
        /// 获取Xml元素集合
        /// </summary>
        /// <param name="xmlElements">当前Xml元素集合</param>
        /// <param name="xmlElementName">要匹配的元素标记名称</param>
        /// <param name="xmlAttributeName">要匹配元素的 特性 筛选 名称</param>
        /// <param name="xmlAttributeValue">要匹配元素的 特性 筛选 内容值 </param>
        /// <returns></returns>
        IEnumerable<XElement> GetXmlElements(IEnumerable<XElement> xmlElements, string xmlElementName, string xmlAttributeName, string xmlAttributeValue);


        /// <summary>
        /// 获取Xml元素
        /// </summary>
        /// <param name="xmlElements">当前Xml元素集合</param>
        /// <param name="xmlElementName">要匹配的元素标记名称</param>
        /// <returns></returns>
        XElement GetXmlElement(IEnumerable<XElement> xmlElements, string xmlElementName);

        /// <summary>
        /// 获取Xml元素
        /// </summary>
        /// <param name="xmlElements">当前Xml元素集合</param>
        /// <param name="xmlElementName">要匹配的元素标记名称</param>
        /// <param name="xmlAttributeName">要匹配元素的 特性 筛选 名称</param>
        /// <param name="xmlAttributeValue">要匹配元素的 特性 筛选 内容值 </param>
        /// <returns></returns>
        XElement GetXmlElement(IEnumerable<XElement> xmlElements, string xmlElementName, string xmlAttributeName, string xmlAttributeValue);


        /// <summary>
        /// 获取Xml元素的内容值
        /// </summary>
        /// <param name="xmlElement">当前Xml元素</param>
        /// <param name="ifTrim">返回的内容值 是否 Trim</param>
        /// <returns></returns>
        string GetXmlElementValue(XElement xmlElement, bool ifTrim = true);

        /// <summary>
        /// 获取Xml元素的特性值
        /// </summary>
        /// <param name="xmlElement">当前Xml元素</param>
        /// <param name="xmlAttributeName">要获取的 特性值 名称</param>
        /// <returns></returns>
        string GetXmlAttributeValue(XElement xmlElement, string xmlAttributeName);


    }


}
