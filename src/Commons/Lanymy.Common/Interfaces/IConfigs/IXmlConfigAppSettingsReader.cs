using System.Xml.Linq;

namespace Lanymy.Common.Interfaces.IConfigs
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
        string GetAppSettingsValueByKey(XElement appSettingsXmlElement, string keyName);

    }
}
