/********************************************************************

时间: 2017年06月02日, PM 10:50:58

作者: lanyanmiyu@qq.com

描述: Xml Config AppSettings 配置表节点  读取 功能接口

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Lanymy.General.Extension.Interfaces
{

    /// <summary>
    /// Xml Config AppSettings 配置表节点  读取 功能接口
    /// </summary>
    public interface IXmlConfigAppSettingsReader
    {


        /// <summary>
        /// 获取 AppSettings 配置 节点
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        XElement GetAppSettingsXmlElement(XDocument xmlDocument);

        /// <summary>
        /// 获取 AppSettings 配置 节点 根据 Key 所 对应的值
        /// </summary>
        /// <param name="appSettingsXmlElement">AppSettings 配置 节点</param>
        /// <param name="keyName">Key名称</param>
        /// <returns></returns>
        string GetAppSettingsValueByKey(XElement appSettingsXmlElement,string keyName);

    }

}
