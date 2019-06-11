using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Interfaces.ISerializers
{


    /// <summary>
    /// Xml 序列化 功能 接口
    /// </summary>
    public interface IXmlSerializer
    {


        /// <summary>
        /// 指定编码序列化对象成XML
        /// </summary>
        /// <param name="t">要序列化的对象</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        string SerializeToXml<T>(T t, Encoding encoding = null) where T : class;

        /// <summary>
        /// 异步 指定编码序列化对象成XML
        /// </summary>
        /// <param name="t">要序列化的对象</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        Task<string> SerializeToXmlAsync<T>(T t, Encoding encoding = null) where T : class;


        /// <summary>
        /// 指定编码反序列化XML成对象
        /// </summary>
        /// <typeparam name="T">需要反序列化成对象的类型</typeparam>
        /// <param name="xmlStr">需要反序列化处理的XML</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        T DeserializeFromXml<T>(string xmlStr, Encoding encoding = null) where T : class;

        /// <summary>
        /// 异步 指定编码反序列化XML成对象
        /// </summary>
        /// <typeparam name="T">需要反序列化成对象的类型</typeparam>
        /// <param name="xmlStr">需要反序列化处理的XML</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        Task<T> DeserializeFromXmlAsync<T>(string xmlStr, Encoding encoding = null) where T : class;


        /// <summary>
        /// 序列化对象成XML文件
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="xmlFileFullPath">要保存序列化成的XML文件全路径</param>
        /// <param name="t">要序列化的对象实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        void SerializeToXmlFile<T>(T t, string xmlFileFullPath, Encoding encoding = null) where T : class;

        /// <summary>
        /// 异步 序列化对象成XML文件
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="xmlFileFullPath">要保存序列化成的XML文件全路径</param>
        /// <param name="t">要序列化的对象实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        Task SerializeToXmlFileAsync<T>(T t, string xmlFileFullPath, Encoding encoding = null) where T : class;


        /// <summary>
        /// 反序列化XML文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="xmlFileFullPath">要反序列化处理的XML文件</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        T DeserializeFromXmlFile<T>(string xmlFileFullPath, Encoding encoding = null) where T : class;
        /// <summary>
        /// 异步 反序列化XML文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="xmlFileFullPath">要反序列化处理的XML文件</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        Task<T> DeserializeFromXmlFileAsync<T>(string xmlFileFullPath, Encoding encoding = null) where T : class;

    }

}
