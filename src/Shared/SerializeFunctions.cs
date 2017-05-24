// *******************************************************************
// 创建时间：2015年01月14日, PM 01:20:48
// 作者：lanyanmiyu@qq.com
// 说明：序列化辅助类
// 其它:
// *******************************************************************



using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
//using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Interfaces;
using Lanymy.General.Extension.Serializer;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace Lanymy.General.Extension
{


    /// <summary>
    /// 序列化辅助类
    /// </summary>
    public class SerializeFunctions
    {


        #region Json序列化


        /// <summary>
        /// 默认Json序列化器
        /// </summary>
        public static readonly IJsonSerializer DefaultJsonSerializer = new JsonNetJsonSerializer(JsonNetJsonSerializer.GetDefaultJsonSerializerSettings());


        /// <summary>
        /// 序列化对象成Json
        /// </summary>
        /// <param name="t">对象</param>
        /// <param name="jsonSerializer">序列化Json使用的序列化器</param>
        public static string SerializeToJson<T>(T t, IJsonSerializer jsonSerializer = null) where T : class
        {
            if (jsonSerializer.IfIsNullOrEmpty())
            {
                jsonSerializer = DefaultJsonSerializer;
            }
            return t.IfIsNullOrEmpty() ? string.Empty : jsonSerializer.SerializeToJson(t);
        }

        /// <summary>
        /// 反序列化Json成对象
        /// </summary>
        /// <param name="json">字符串序列</param>
        /// <param name="jsonSerializer">反序列化Json使用的序列化器</param>
        public static T DeserializeFromJson<T>(string json, IJsonSerializer jsonSerializer = null) where T : class
        {

            if (jsonSerializer.IfIsNullOrEmpty())
            {
                jsonSerializer = DefaultJsonSerializer;
            }

            return json.IfIsNullOrEmpty() ? default(T) : jsonSerializer.DeserializeFromJson<T>(json);
        }

        /// <summary>
        /// 异步序列化对象成Json
        /// </summary>
        /// <param name="t">对象</param>
        /// <param name="jsonSerializer">序列化Json使用的序列化器</param>
        public static Task<string> SerializeToJsonAsync<T>(T t, IJsonSerializer jsonSerializer = null) where T : class
        {
            return SerializeToJsonAsync(t, jsonSerializer);
        }



        /// <summary>
        /// 异步反序列化Json成对象
        /// </summary>
        /// <param name="json">字符串序列</param>
        /// <param name="jsonSerializer">反序列化Json使用的序列化器</param>
        public static Task<T> DeserializeFromJsonAsync<T>(string json, IJsonSerializer jsonSerializer = null) where T : class
        {
            return DeserializeFromJsonAsync<T>(json, jsonSerializer);
        }



        #endregion


        #region XML序列化

        /// <summary>
        /// 默认Xml序列化器
        /// </summary>
        public static readonly IXmlSerializer DefaultXmlSerializer = new LanymyXmlSerializer();


        /// <summary>
        /// 指定编码序列化对象成XML
        /// </summary>
        /// <param name="t">要序列化的对象</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="xmlSerializer">Xml 序列化 功能 接口</param>
        /// <returns></returns>
        public static string SerializeToXml<T>(T t, Encoding encoding = null, IXmlSerializer xmlSerializer = null) where T : class
        {
            if (xmlSerializer.IfIsNullOrEmpty())
            {
                xmlSerializer = DefaultXmlSerializer;
            }
            return xmlSerializer.SerializeToXml(t, encoding);
        }

        /// <summary>
        /// 异步 指定编码序列化对象成XML
        /// </summary>
        /// <param name="t">要序列化的对象</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="xmlSerializer">Xml 序列化 功能 接口</param>
        /// <returns></returns>
        public static Task<string> SerializeToXmlAsync<T>(T t, Encoding encoding = null, IXmlSerializer xmlSerializer = null) where T : class
        {
            if (xmlSerializer.IfIsNullOrEmpty())
            {
                xmlSerializer = DefaultXmlSerializer;
            }
            return xmlSerializer.SerializeToXmlAsync(t, encoding);
        }
        /// <summary>
        /// 指定编码反序列化XML成对象
        /// </summary>
        /// <typeparam name="T">需要反序列化成对象的类型</typeparam>
        /// <param name="xmlStr">需要反序列化处理的XML</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="xmlSerializer">Xml 序列化 功能 接口</param>
        /// <returns></returns>
        public static T DeserializeFromXml<T>(string xmlStr, Encoding encoding = null, IXmlSerializer xmlSerializer = null) where T : class
        {
            if (xmlSerializer.IfIsNullOrEmpty())
            {
                xmlSerializer = DefaultXmlSerializer;
            }
            return xmlSerializer.DeserializeFromXml<T>(xmlStr, encoding);
        }
        /// <summary>
        /// 异步 指定编码反序列化XML成对象
        /// </summary>
        /// <typeparam name="T">需要反序列化成对象的类型</typeparam>
        /// <param name="xmlStr">需要反序列化处理的XML</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="xmlSerializer">Xml 序列化 功能 接口</param>
        /// <returns></returns>
        public static Task<T> DeserializeFromXmlAsync<T>(string xmlStr, Encoding encoding = null, IXmlSerializer xmlSerializer = null) where T : class
        {
            if (xmlSerializer.IfIsNullOrEmpty())
            {
                xmlSerializer = DefaultXmlSerializer;
            }
            return xmlSerializer.DeserializeFromXmlAsync<T>(xmlStr, encoding);
        }
        /// <summary>
        /// 序列化对象成XML文件
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="xmlFileFullPath">要保存序列化成的XML文件全路径</param>
        /// <param name="t">要序列化的对象实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="xmlSerializer">Xml 序列化 功能 接口</param>
        public static void SerializeToXmlFile<T>(T t, string xmlFileFullPath, Encoding encoding = null, IXmlSerializer xmlSerializer = null) where T : class
        {
            if (xmlSerializer.IfIsNullOrEmpty())
            {
                xmlSerializer = DefaultXmlSerializer;
            }
            xmlSerializer.SerializeToXmlFile(t, xmlFileFullPath, encoding);
        }
        /// <summary>
        /// 异步 序列化对象成XML文件
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="xmlFileFullPath">要保存序列化成的XML文件全路径</param>
        /// <param name="t">要序列化的对象实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="xmlSerializer">Xml 序列化 功能 接口</param>
        public static Task SerializeToXmlFileAsync<T>(T t, string xmlFileFullPath, Encoding encoding = null, IXmlSerializer xmlSerializer = null) where T : class
        {
            if (xmlSerializer.IfIsNullOrEmpty())
            {
                xmlSerializer = DefaultXmlSerializer;
            }
            return xmlSerializer.SerializeToXmlFileAsync(t, xmlFileFullPath, encoding);
        }
        /// <summary>
        /// 反序列化XML文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="xmlFileFullPath">要反序列化处理的XML文件</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="xmlSerializer">Xml 序列化 功能 接口</param>
        /// <returns></returns>
        public static T DeserializeFromXmlFile<T>(string xmlFileFullPath, Encoding encoding = null, IXmlSerializer xmlSerializer = null) where T : class
        {
            if (xmlSerializer.IfIsNullOrEmpty())
            {
                xmlSerializer = DefaultXmlSerializer;
            }
            return xmlSerializer.DeserializeFromXmlFile<T>(xmlFileFullPath, encoding);
        }
        /// <summary>
        /// 异步 反序列化XML文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="xmlFileFullPath">要反序列化处理的XML文件</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="xmlSerializer">Xml 序列化 功能 接口</param>
        /// <returns></returns>
        public static Task<T> DeserializeFromXmlFileAsync<T>(string xmlFileFullPath, Encoding encoding = null, IXmlSerializer xmlSerializer = null) where T : class
        {
            if (xmlSerializer.IfIsNullOrEmpty())
            {
                xmlSerializer = DefaultXmlSerializer;
            }
            return xmlSerializer.DeserializeFromXmlFileAsync<T>(xmlFileFullPath, encoding);
        }




        #endregion


        #region Binary序列化


        /// <summary>
        /// 默认Binary序列化器
        /// </summary>
        public static readonly IBinarySerializer DefaultBinarySerializer = new LanymyBinarySerializer();


        /// <summary>
        /// 把对象序列化成二进制数据
        /// </summary>
        /// <param name="t">要序列化的对象</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="binarySerializer">二进制 序列化 功能 接口</param>
        /// <returns></returns>
        public static byte[] SerializeToBytes<T>(T t, Encoding encoding = null, IBinarySerializer binarySerializer = null) where T : class
        {
            if (binarySerializer.IfIsNullOrEmpty())
            {
                binarySerializer = DefaultBinarySerializer;
            }
            return binarySerializer.SerializeToBytes(t, encoding);
        }
        /// <summary>
        /// 异步 把对象序列化成二进制数据
        /// </summary>
        /// <param name="t">要序列化的对象</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="binarySerializer">二进制 序列化 功能 接口</param>
        /// <returns></returns>
        public static Task<byte[]> SerializeToBytesAsync<T>(T t, Encoding encoding = null, IBinarySerializer binarySerializer = null) where T : class
        {
            if (binarySerializer.IfIsNullOrEmpty())
            {
                binarySerializer = DefaultBinarySerializer;
            }
            return binarySerializer.SerializeToBytesAsync(t, encoding);
        }
        /// <summary>
        /// 反序列化二进制数据成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象 的 对象类型</typeparam>
        /// <param name="bytes">需要反序列化处理的二进制数据</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="binarySerializer">二进制 序列化 功能 接口</param>
        /// <returns></returns>
        public static T DeserializeFromBytes<T>(byte[] bytes, Encoding encoding = null, IBinarySerializer binarySerializer = null) where T : class
        {
            if (binarySerializer.IfIsNullOrEmpty())
            {
                binarySerializer = DefaultBinarySerializer;
            }
            return binarySerializer.DeserializeFromBytes<T>(bytes, encoding);

        }
        /// <summary>
        /// 异步 反序列化二进制数据成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象 的 对象类型</typeparam>
        /// <param name="bytes">需要反序列化处理的二进制数据</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="binarySerializer">二进制 序列化 功能 接口</param>
        /// <returns></returns>
        public static Task<T> DeserializeFromBytesAsync<T>(byte[] bytes, Encoding encoding = null, IBinarySerializer binarySerializer = null) where T : class
        {
            if (binarySerializer.IfIsNullOrEmpty())
            {
                binarySerializer = DefaultBinarySerializer;
            }
            return binarySerializer.DeserializeFromBytesAsync<T>(bytes, encoding);
        }
        /// <summary>
        /// 把对象序列化成二进制文件
        /// </summary>
        /// <typeparam name="T">要序列化对象 的 对象类型</typeparam>
        /// <param name="binaryFileFullPath">要序列化成二进制文件的全路径</param>
        /// <param name="t">要序列化对象的实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="ifCompressBytes">是否压缩字节数组 默认值 True 压缩形式序列化字节数组</param>
        /// <param name="binarySerializer">二进制 序列化 功能 接口</param>
        /// <returns></returns>
        public static void SerializeToBytesFile<T>(T t, string binaryFileFullPath, Encoding encoding = null, bool ifCompressBytes = true, IBinarySerializer binarySerializer = null) where T : class
        {
            if (binarySerializer.IfIsNullOrEmpty())
            {
                binarySerializer = DefaultBinarySerializer;
            }
            binarySerializer.SerializeToBytesFile(t, binaryFileFullPath, encoding);
        }
        /// <summary>
        /// 异步 把对象序列化成二进制文件
        /// </summary>
        /// <typeparam name="T">要序列化对象 的 对象类型</typeparam>
        /// <param name="binaryFileFullPath">要序列化成二进制文件的全路径</param>
        /// <param name="t">要序列化对象的实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="ifCompressBytes">是否压缩字节数组 默认值 True 压缩形式序列化字节数组</param>
        /// <param name="binarySerializer">二进制 序列化 功能 接口</param>
        /// <returns></returns>
        public static Task SerializeToBytesFileAsync<T>(T t, string binaryFileFullPath, Encoding encoding = null, bool ifCompressBytes = true, IBinarySerializer binarySerializer = null) where T : class
        {
            if (binarySerializer.IfIsNullOrEmpty())
            {
                binarySerializer = DefaultBinarySerializer;
            }
            return binarySerializer.SerializeToBytesFileAsync(t, binaryFileFullPath, encoding);
        }

        /// <summary>
        /// 反序列化二进制文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="binaryFileFullPath">二进制文件全路径</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="ifDecompressBytes">是否解压缩字节数组 默认值 True 解压缩形式反序列化字节数组</param>
        /// <param name="binarySerializer">二进制 序列化 功能 接口</param>
        /// <returns></returns>
        public static T DeserializeFromBytesFile<T>(string binaryFileFullPath, Encoding encoding = null, bool ifDecompressBytes = true, IBinarySerializer binarySerializer = null) where T : class
        {
            if (binarySerializer.IfIsNullOrEmpty())
            {
                binarySerializer = DefaultBinarySerializer;
            }
            return binarySerializer.DeserializeFromBytesFile<T>(binaryFileFullPath, encoding);
        }

        /// <summary>
        /// 异步 反序列化二进制文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="binaryFileFullPath">二进制文件全路径</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="ifDecompressBytes">是否解压缩字节数组 默认值 True 解压缩形式反序列化字节数组</param>
        /// <param name="binarySerializer">二进制 序列化 功能 接口</param>
        /// <returns></returns>
        public static Task<T> DeserializeFromBytesFileAsync<T>(string binaryFileFullPath, Encoding encoding = null, bool ifDecompressBytes = true, IBinarySerializer binarySerializer = null) where T : class
        {
            if (binarySerializer.IfIsNullOrEmpty())
            {
                binarySerializer = DefaultBinarySerializer;
            }
            return binarySerializer.DeserializeFromBytesFileAsync<T>(binaryFileFullPath, encoding);
        }


        #endregion


        //#region SoapFormatter序列化


        ///// <summary>
        ///// 对象序列化成Soap数据
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="t"></param>
        ///// <returns></returns>
        //public static string SerializeToSoap<T>(List<T> t) where T : class
        //{
        //    return SerializeToSoap(t.ToArray());
        //}


        ///// <summary>
        ///// 对象序列化成Soap数据 如果是列表 只接受Array类型 
        ///// </summary>
        ///// <param name="t">对象实例</param>
        //public static string SerializeToSoap<T>(T t) where T : class
        //{

        //    string result = string.Empty;

        //    if (t.IfIsNullOrEmpty()) return result;

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        SoapFormatter formatter = new SoapFormatter();
        //        formatter.Serialize(ms, t);
        //        ms.Position = 0;
        //        XmlDocument xmlDoc = new XmlDocument();
        //        xmlDoc.Load(ms);
        //        result = FormatFunctions.FormatXml(xmlDoc);
        //    }

        //    return result;
        //}


        ///// <summary>
        ///// 反序列化Soap成List列表
        ///// </summary>
        ///// <typeparam name="T">List列表中对象类型</typeparam>
        ///// <param name="strSoap">Soap数据</param>
        ///// <returns></returns>
        //public static List<T> DeserializeFromSoapWithList<T>(string strSoap) where T : class
        //{
        //    return new List<T>(DeserializeFromSoap<T[]>(strSoap));
        //}


        ///// <summary>
        ///// 反序列化Soap数据成对象
        ///// </summary>
        ///// <typeparam name="T">对象数据类型</typeparam>
        ///// <param name="strSoap">Soap数据</param>
        ///// <returns></returns>
        //public static T DeserializeFromSoap<T>(string strSoap) where T : class
        //{

        //    T t = default(T);

        //    if (strSoap.IfIsNullOrEmpty()) return t;

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        XmlDocument xmlDoc = new XmlDocument();
        //        xmlDoc.LoadXml(strSoap);
        //        xmlDoc.Save(ms);
        //        ms.Position = 0;

        //        SoapFormatter formatter = new SoapFormatter();
        //        t = (T)formatter.Deserialize(ms);
        //    }

        //    return t;
        //}

        //#endregion


    }
}
