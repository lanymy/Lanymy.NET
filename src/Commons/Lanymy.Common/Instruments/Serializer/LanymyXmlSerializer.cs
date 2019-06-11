using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.FileTextReadOrWrite;
using Lanymy.Common.Interfaces.ISerializers;

namespace Lanymy.Common.Instruments.Serializer
{
    /// <summary>
    /// Xml序列化器
    /// </summary>
    public class LanymyXmlSerializer : BaseSerializer, IXmlSerializer
    {


        /// <summary>
        /// Xml序列化器 构造方法
        /// </summary>
        /// <param name="encoding">编码</param>
        public LanymyXmlSerializer(Encoding encoding = null) : base(encoding)
        {

        }

        /// <summary>
        /// 指定编码序列化对象成XML
        /// </summary>
        /// <param name="t">要序列化的对象</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual string SerializeToXml<T>(T t, Encoding encoding = null) where T : class
        {
            if (t.IfIsNullOrEmpty()) return string.Empty;
            if (encoding.IfIsNullOrEmpty()) encoding = CurrentEncoding;

            string result;

            using (MemoryStream ms = new MemoryStream())
            {
                using (XmlTextWriter writer = new XmlTextWriter(ms, encoding))
                {
                    writer.Formatting = Formatting.Indented;
                    var serializer = new XmlSerializer(t.GetType());
                    serializer.Serialize(writer, t);
                    result = encoding.GetString(ms.ToArray());
                }
            }

            return result;
        }

        /// <summary>
        /// 异步 指定编码序列化对象成XML
        /// </summary>
        /// <param name="t">要序列化的对象</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual async Task<string> SerializeToXmlAsync<T>(T t, Encoding encoding = null) where T : class
        {
            return await GenericityHelper.DoTaskWorkAsync(SerializeToXml, t, encoding);
        }
        /// <summary>
        /// 指定编码反序列化XML成对象
        /// </summary>
        /// <typeparam name="T">需要反序列化成对象的类型</typeparam>
        /// <param name="xmlStr">需要反序列化处理的XML</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual T DeserializeFromXml<T>(string xmlStr, Encoding encoding = null) where T : class
        {
            T t = default(T);

            if (xmlStr.IfIsNullOrEmpty()) return t;
            if (encoding.IfIsNullOrEmpty()) encoding = CurrentEncoding;

            using (MemoryStream ms = new MemoryStream(encoding.GetBytes(xmlStr)))
            {
                var serializer = new XmlSerializer(typeof(T));
                t = (T)serializer.Deserialize(ms);
            }

            return t;
        }
        /// <summary>
        /// 异步 指定编码反序列化XML成对象
        /// </summary>
        /// <typeparam name="T">需要反序列化成对象的类型</typeparam>
        /// <param name="xmlStr">需要反序列化处理的XML</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual async Task<T> DeserializeFromXmlAsync<T>(string xmlStr, Encoding encoding = null) where T : class
        {
            return await GenericityHelper.DoTaskWorkAsync(DeserializeFromXml<T>, xmlStr, encoding);
        }
        /// <summary>
        /// 序列化对象成XML文件
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="xmlFileFullPath">要保存序列化成的XML文件全路径</param>
        /// <param name="t">要序列化的对象实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        public virtual void SerializeToXmlFile<T>(T t, string xmlFileFullPath, Encoding encoding = null) where T : class
        {
            using (var writer = new FileTextWriter(xmlFileFullPath, true, encoding))
            {
                writer.Write(SerializeToXml(t, encoding));
            }
        }
        /// <summary>
        /// 异步 序列化对象成XML文件
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="xmlFileFullPath">要保存序列化成的XML文件全路径</param>
        /// <param name="t">要序列化的对象实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        public virtual async Task SerializeToXmlFileAsync<T>(T t, string xmlFileFullPath, Encoding encoding = null) where T : class
        {
            await GenericityHelper.DoTaskWorkAsync(SerializeToXmlFile<T>, t, xmlFileFullPath, encoding);
        }
        /// <summary>
        /// 反序列化XML文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="xmlFileFullPath">要反序列化处理的XML文件</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual T DeserializeFromXmlFile<T>(string xmlFileFullPath, Encoding encoding = null) where T : class
        {
            T t;

            using (var reader = new FileTextReader(xmlFileFullPath, encoding))
            {
                t = DeserializeFromXml<T>(reader.ReadAll(), encoding);
            }

            return t;
        }
        /// <summary>
        /// 异步 反序列化XML文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="xmlFileFullPath">要反序列化处理的XML文件</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual async Task<T> DeserializeFromXmlFileAsync<T>(string xmlFileFullPath, Encoding encoding = null) where T : class
        {
            return await GenericityHelper.DoTaskWorkAsync(DeserializeFromXmlFile<T>, xmlFileFullPath, encoding);
        }


    }
}
