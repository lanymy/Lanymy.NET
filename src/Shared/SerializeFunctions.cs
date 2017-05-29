// *******************************************************************
// 创建时间：2015年01月14日, PM 01:20:48
// 作者：lanyanmiyu@qq.com
// 说明：序列化辅助类
// 其它:
// *******************************************************************



using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Interfaces;
using Lanymy.General.Extension.Serializer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        private static IJsonSerializer GetJsonSerializer(IJsonSerializer jsonSerializer = null)
        {
            if (jsonSerializer.IfIsNullOrEmpty())
            {
                jsonSerializer = DefaultJsonSerializer;
            }
            return jsonSerializer;
        }

        /// <summary>
        /// 序列化对象成Json
        /// </summary>
        /// <param name="t">对象</param>
        /// <param name="jsonSerializer">序列化Json使用的序列化器</param>
        public static string SerializeToJson<T>(T t, IJsonSerializer jsonSerializer = null) where T : class
        {
            return t.IfIsNullOrEmpty() ? string.Empty : GetJsonSerializer(jsonSerializer).SerializeToJson(t);
        }

        /// <summary>
        /// 反序列化Json成对象
        /// </summary>
        /// <param name="json">字符串序列</param>
        /// <param name="jsonSerializer">反序列化Json使用的序列化器</param>
        public static T DeserializeFromJson<T>(string json, IJsonSerializer jsonSerializer = null) where T : class
        {
            return json.IfIsNullOrEmpty() ? default(T) : GetJsonSerializer(jsonSerializer).DeserializeFromJson<T>(json);
        }

        /// <summary>
        /// 异步序列化对象成Json
        /// </summary>
        /// <param name="t">对象</param>
        /// <param name="jsonSerializer">序列化Json使用的序列化器</param>
        public static Task<string> SerializeToJsonAsync<T>(T t, IJsonSerializer jsonSerializer = null) where T : class
        {
            return GetJsonSerializer(jsonSerializer).SerializeToJsonAsync(t);
        }



        /// <summary>
        /// 异步反序列化Json成对象
        /// </summary>
        /// <param name="json">字符串序列</param>
        /// <param name="jsonSerializer">反序列化Json使用的序列化器</param>
        public static Task<T> DeserializeFromJsonAsync<T>(string json, IJsonSerializer jsonSerializer = null) where T : class
        {
            return GetJsonSerializer(jsonSerializer).DeserializeFromJsonAsync<T>(json);
        }



        #endregion


        #region XML序列化

        /// <summary>
        /// 默认Xml序列化器
        /// </summary>
        public static readonly IXmlSerializer DefaultXmlSerializer = new LanymyXmlSerializer();

        private static IXmlSerializer GetXmlSerializer( IXmlSerializer xmlSerializer = null) 
        {
            if (xmlSerializer.IfIsNullOrEmpty())
            {
                xmlSerializer = DefaultXmlSerializer;
            }
            return xmlSerializer;
        }

        /// <summary>
        /// 指定编码序列化对象成XML
        /// </summary>
        /// <param name="t">要序列化的对象</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="xmlSerializer">Xml 序列化 功能 接口</param>
        /// <returns></returns>
        public static string SerializeToXml<T>(T t, Encoding encoding = null, IXmlSerializer xmlSerializer = null) where T : class
        {
            return GetXmlSerializer(xmlSerializer).SerializeToXml(t, encoding);
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
            return GetXmlSerializer(xmlSerializer).SerializeToXmlAsync(t, encoding);
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
            return GetXmlSerializer(xmlSerializer).DeserializeFromXml<T>(xmlStr, encoding);
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
            return GetXmlSerializer(xmlSerializer).DeserializeFromXmlAsync<T>(xmlStr, encoding);
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
            GetXmlSerializer(xmlSerializer).SerializeToXmlFile(t, xmlFileFullPath, encoding);
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
            return GetXmlSerializer(xmlSerializer).SerializeToXmlFileAsync(t, xmlFileFullPath, encoding);
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
            return GetXmlSerializer(xmlSerializer).DeserializeFromXmlFile<T>(xmlFileFullPath, encoding);
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
            return GetXmlSerializer(xmlSerializer).DeserializeFromXmlFileAsync<T>(xmlFileFullPath, encoding);
        }




        #endregion


        #region Binary序列化


        /// <summary>
        /// 默认Binary序列化器
        /// </summary>
        public static readonly IBinarySerializer DefaultBinarySerializer = new LanymyBinarySerializer();

        private static IBinarySerializer GetBinarySerializer(IBinarySerializer binarySerializer = null)
        {
            if (binarySerializer.IfIsNullOrEmpty())
            {
                binarySerializer = DefaultBinarySerializer;
            }
            return binarySerializer;
        }

        /// <summary>
        /// 把对象序列化成二进制数据
        /// </summary>
        /// <param name="t">要序列化的对象</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="binarySerializer">二进制 序列化 功能 接口</param>
        /// <returns></returns>
        public static byte[] SerializeToBytes<T>(T t, Encoding encoding = null, IBinarySerializer binarySerializer = null) where T : class
        {
            return GetBinarySerializer(binarySerializer).SerializeToBytes(t, encoding);
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
            return GetBinarySerializer(binarySerializer).SerializeToBytesAsync(t, encoding);
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
            return GetBinarySerializer(binarySerializer).DeserializeFromBytes<T>(bytes, encoding);
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
            return GetBinarySerializer(binarySerializer).DeserializeFromBytesAsync<T>(bytes, encoding);
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
            GetBinarySerializer(binarySerializer).SerializeToBytesFile(t, binaryFileFullPath, encoding);
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
            return GetBinarySerializer(binarySerializer).SerializeToBytesFileAsync(t, binaryFileFullPath, encoding);
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
            return GetBinarySerializer(binarySerializer).DeserializeFromBytesFile<T>(binaryFileFullPath, encoding);
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
            return GetBinarySerializer(binarySerializer).DeserializeFromBytesFileAsync<T>(binaryFileFullPath, encoding);
        }


        #endregion


        #region CSV序列化

        /// <summary>
        /// 默认CSV序列化器
        /// </summary>
        public static readonly Dictionary<Type, object> DefaultCsvSerializer = new Dictionary<Type, object>();


        private static ICsvSerializer<T> GetDefaultCSVSerializer<T>() where T : class, new()
        {
            Type type = typeof(T);
            if (!DefaultCsvSerializer.ContainsKey(type))
            {
                DefaultCsvSerializer.AddOrReplace(type, new LanymyCSVSerializer<T>());
            }
            return DefaultCsvSerializer[type] as ICsvSerializer<T>;
        }

        private static ICsvSerializer<T> GetCSVSerializer<T>(ICsvSerializer<T> csvSerializer = null) where T : class, new()
        {
            if (csvSerializer.IfIsNullOrEmpty())
            {
                csvSerializer = GetDefaultCSVSerializer<T>();
            }
            return csvSerializer;
        }

        private static ICsvModelSerializer<T> GetCSVSerializer<T>(ICsvModelSerializer<T> csvModelSerializer = null) where T : class, new()
        {
            if (csvModelSerializer.IfIsNullOrEmpty())
            {
                csvModelSerializer = GetDefaultCSVSerializer<T>();
            }
            return csvModelSerializer;
        }

        private static ICsvFileSerializer<T> GetCSVSerializer<T>(ICsvFileSerializer<T> csvFileSerializer = null) where T : class, new()
        {
            if (csvFileSerializer.IfIsNullOrEmpty())
            {
                csvFileSerializer = GetDefaultCSVSerializer<T>();
            }
            return csvFileSerializer;
        }


        /// <summary>
        /// 获取CSV数据的标题
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvModelSerializer">CSV 序列化 / 反序列化 功能 接口</param>
        /// <returns></returns>
        public static string GetCSVTitle<T>(ICsvModelSerializer<T> csvModelSerializer = null) where T : class, new()
        {
            return GetCSVSerializer(csvModelSerializer).GetCSVTitle();
        }

        /// <summary>
        /// 异步 获取CSV数据的标题
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvModelSerializer">CSV 序列化 / 反序列化 功能 接口</param>
        /// <returns></returns>
        public static Task<string> GetCSVTitleAsync<T>(ICsvModelSerializer<T> csvModelSerializer = null) where T : class, new()
        {
            return GetCSVSerializer(csvModelSerializer).GetCSVTitleAsync();
        }


        /// <summary>
        /// 实体类序列化成CSV
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="csvSerializer">CSV 序列化 / 反序列化 功能 接口</param>
        /// <returns></returns>
        public static string SerializeToCSV<T>(T t, ICsvModelSerializer<T> csvSerializer = null) where T : class, new()
        {
            return GetCSVSerializer(csvSerializer).SerializeToCSV(t);
        }

        /// <summary>
        /// 异步 实体类序列化成CSV
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="csvSerializer">CSV 序列化 / 反序列化 功能 接口</param>
        /// <returns></returns>
        public static Task<string> SerializeToCsvAsync<T>(T t, ICsvModelSerializer<T> csvSerializer = null) where T : class, new()
        {
            return GetCSVSerializer(csvSerializer).SerializeToCsvAsync(t);
        }


        /// <summary>
        /// 反序列化CSV成实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvString"></param>
        /// <param name="csvSerializer">CSV 序列化 / 反序列化 功能 接口</param>
        /// <returns></returns>
        public static T DeserializeFromCSV<T>(string csvString, ICsvModelSerializer<T> csvSerializer = null) where T : class, new()
        {
            return GetCSVSerializer(csvSerializer).DeserializeFromCSV(csvString);
        }


        /// <summary>
        /// 异步 反序列化CSV成实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvString"></param>
        /// <param name="csvSerializer">CSV 序列化 / 反序列化 功能 接口</param>
        /// <returns></returns>
        public static Task<T> DeserializeFromCsvAsync<T>(string csvString, ICsvModelSerializer<T> csvSerializer = null) where T : class, new()
        {
            return GetCSVSerializer(csvSerializer).DeserializeFromCsvAsync(csvString);
        }


        /// <summary>
        /// 序列化到CSV文件
        /// </summary>
        /// <param name="csvFileFullPath">CSV文件全路径</param>
        /// <param name="list">要序列化的实体类数据集合</param>
        /// <param name="ifWriteTitle">是否写入 标题 </param>
        /// <param name="csvFileSerializer">CSV 序列化 文件 功能 接口</param>
        public static void SerializeToCsvFile<T>(string csvFileFullPath, IEnumerable<T> list, bool ifWriteTitle = true, ICsvFileSerializer<T> csvFileSerializer = null) where T : class, new()
        {
            GetCSVSerializer(csvFileSerializer).SerializeToCsvFile(csvFileFullPath, list, ifWriteTitle);
        }


        /// <summary>
        /// 异步 序列化到CSV文件
        /// </summary>
        /// <param name="csvFileFullPath">CSV文件全路径</param>
        /// <param name="list">要序列化的实体类数据集合</param>
        /// <param name="ifWriteTitle">是否写入 标题 </param>
        /// <param name="csvFileSerializer">CSV 序列化 文件 功能 接口</param>
        public static Task SerializeToCsvFileAsync<T>(string csvFileFullPath, IEnumerable<T> list, bool ifWriteTitle = true, ICsvFileSerializer<T> csvFileSerializer = null) where T : class, new()
        {
            return GetCSVSerializer(csvFileSerializer).SerializeToCsvFileAsync(csvFileFullPath, list, ifWriteTitle);
        }
        /// <summary>
        /// 从CSV文件反序列化数据
        /// </summary>
        /// <param name="csvFileFullPath">CSV文件全路径</param>
        /// <param name="csvAnnotationSymbol">行首 注释符 默认 '#'</param>
        /// <param name="csvFileSerializer">CSV 序列化 文件 功能 接口</param>
        /// <returns></returns>
        public static List<T> DeserializeFromCsvFile<T>(string csvFileFullPath, string csvAnnotationSymbol = GlobalSettings.CSV_ANNOTATION_SYMBOL, ICsvFileSerializer<T> csvFileSerializer = null) where T : class, new()
        {
            return GetCSVSerializer(csvFileSerializer).DeserializeFromCsvFile(csvFileFullPath, csvAnnotationSymbol);
        }
        /// <summary>
        /// 异步 从CSV文件反序列化数据
        /// </summary>
        /// <param name="csvFileFullPath">CSV文件全路径</param>
        /// <param name="csvAnnotationSymbol">行首 注释符 默认 '#'</param>
        /// <param name="csvFileSerializer">CSV 序列化 文件 功能 接口</param>
        /// <returns></returns>
        public static Task<List<T>> DeserializeFromCsvFileAsync<T>(string csvFileFullPath, string csvAnnotationSymbol = GlobalSettings.CSV_ANNOTATION_SYMBOL, ICsvFileSerializer<T> csvFileSerializer = null) where T : class, new()
        {
            return GetCSVSerializer(csvFileSerializer).DeserializeFromCsvFileAsync(csvFileFullPath, csvAnnotationSymbol);
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
