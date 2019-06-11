using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Serializer;
using Lanymy.Common.Interfaces.ISerializers;

namespace Lanymy.Common
{
    /// <summary>
    /// 序列化辅助类
    /// </summary>
    public class SerializeHelper
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
            return t.IfIsNullOrEmpty() ? string.Empty : GenericityHelper.GetInterface(jsonSerializer, DefaultJsonSerializer).SerializeToJson(t);
        }

        /// <summary>
        /// 反序列化Json成对象
        /// </summary>
        /// <param name="json">字符串序列</param>
        /// <param name="jsonSerializer">反序列化Json使用的序列化器</param>
        public static T DeserializeFromJson<T>(string json, IJsonSerializer jsonSerializer = null) where T : class
        {
            return json.IfIsNullOrEmpty() ? default(T) : GenericityHelper.GetInterface(jsonSerializer, DefaultJsonSerializer).DeserializeFromJson<T>(json);
        }

        /// <summary>
        /// 异步序列化对象成Json
        /// </summary>
        /// <param name="t">对象</param>
        /// <param name="jsonSerializer">序列化Json使用的序列化器</param>
        public static async Task<string> SerializeToJsonAsync<T>(T t, IJsonSerializer jsonSerializer = null) where T : class
        {
            return await GenericityHelper.GetInterface(jsonSerializer, DefaultJsonSerializer).SerializeToJsonAsync(t);
        }



        /// <summary>
        /// 异步反序列化Json成对象
        /// </summary>
        /// <param name="json">字符串序列</param>
        /// <param name="jsonSerializer">反序列化Json使用的序列化器</param>
        public static async Task<T> DeserializeFromJsonAsync<T>(string json, IJsonSerializer jsonSerializer = null) where T : class
        {
            return await GenericityHelper.GetInterface(jsonSerializer, DefaultJsonSerializer).DeserializeFromJsonAsync<T>(json);
        }


        /// <summary>
        /// 序列化对象成JSON文件
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="jsonFileFullPath">要保存序列化成的JSON文件全路径</param>
        /// <param name="t">要序列化的对象实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="jsonSerializer">反序列化Json使用的序列化器</param>
        public static void SerializeToJsonFile<T>(T t, string jsonFileFullPath, Encoding encoding = null, IJsonSerializer jsonSerializer = null) where T : class
        {
            GenericityHelper.GetInterface(jsonSerializer, DefaultJsonSerializer).SerializeToJsonFile(t, jsonFileFullPath, encoding);
        }

        /// <summary>
        /// 异步 序列化对象成JSON文件
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="jsonFileFullPath">要保存序列化成的JSON文件全路径</param>
        /// <param name="t">要序列化的对象实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="jsonSerializer">反序列化Json使用的序列化器</param>
        public static async Task SerializeToJsonFileAsync<T>(T t, string jsonFileFullPath, Encoding encoding = null, IJsonSerializer jsonSerializer = null) where T : class
        {
            await GenericityHelper.GetInterface(jsonSerializer, DefaultJsonSerializer).SerializeToJsonFileAsync(t, jsonFileFullPath, encoding);
        }

        /// <summary>
        /// 反序列化JSON文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="jsonFileFullPath">要反序列化处理的JSON文件</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="jsonSerializer">反序列化Json使用的序列化器</param>
        /// <returns></returns>
        public static T DeserializeFromJsonFile<T>(string jsonFileFullPath, Encoding encoding = null, IJsonSerializer jsonSerializer = null) where T : class
        {
            return GenericityHelper.GetInterface(jsonSerializer, DefaultJsonSerializer).DeserializeFromJsonFile<T>(jsonFileFullPath, encoding);
        }

        /// <summary>
        /// 异步 反序列化JSON文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="jsonFileFullPath">要反序列化处理的JSON文件</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="jsonSerializer">反序列化Json使用的序列化器</param>
        /// <returns></returns>
        public static async Task<T> DeserializeFromJsonFileAsync<T>(string jsonFileFullPath, Encoding encoding = null, IJsonSerializer jsonSerializer = null) where T : class
        {
            return await GenericityHelper.GetInterface(jsonSerializer, DefaultJsonSerializer).DeserializeFromJsonFileAsync<T>(jsonFileFullPath, encoding);
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
            return GenericityHelper.GetInterface(xmlSerializer, DefaultXmlSerializer).SerializeToXml(t, encoding);
        }

        /// <summary>
        /// 异步 指定编码序列化对象成XML
        /// </summary>
        /// <param name="t">要序列化的对象</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="xmlSerializer">Xml 序列化 功能 接口</param>
        /// <returns></returns>
        public static async Task<string> SerializeToXmlAsync<T>(T t, Encoding encoding = null, IXmlSerializer xmlSerializer = null) where T : class
        {
            return await GenericityHelper.GetInterface(xmlSerializer, DefaultXmlSerializer).SerializeToXmlAsync(t, encoding);
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
            return GenericityHelper.GetInterface(xmlSerializer, DefaultXmlSerializer).DeserializeFromXml<T>(xmlStr, encoding);
        }
        /// <summary>
        /// 异步 指定编码反序列化XML成对象
        /// </summary>
        /// <typeparam name="T">需要反序列化成对象的类型</typeparam>
        /// <param name="xmlStr">需要反序列化处理的XML</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="xmlSerializer">Xml 序列化 功能 接口</param>
        /// <returns></returns>
        public static async Task<T> DeserializeFromXmlAsync<T>(string xmlStr, Encoding encoding = null, IXmlSerializer xmlSerializer = null) where T : class
        {
            return await GenericityHelper.GetInterface(xmlSerializer, DefaultXmlSerializer).DeserializeFromXmlAsync<T>(xmlStr, encoding);
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
            GenericityHelper.GetInterface(xmlSerializer, DefaultXmlSerializer).SerializeToXmlFile(t, xmlFileFullPath, encoding);
        }
        /// <summary>
        /// 异步 序列化对象成XML文件
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="xmlFileFullPath">要保存序列化成的XML文件全路径</param>
        /// <param name="t">要序列化的对象实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="xmlSerializer">Xml 序列化 功能 接口</param>
        public static async Task SerializeToXmlFileAsync<T>(T t, string xmlFileFullPath, Encoding encoding = null, IXmlSerializer xmlSerializer = null) where T : class
        {
            await GenericityHelper.GetInterface(xmlSerializer, DefaultXmlSerializer).SerializeToXmlFileAsync(t, xmlFileFullPath, encoding);
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
            return GenericityHelper.GetInterface(xmlSerializer, DefaultXmlSerializer).DeserializeFromXmlFile<T>(xmlFileFullPath, encoding);
        }
        /// <summary>
        /// 异步 反序列化XML文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="xmlFileFullPath">要反序列化处理的XML文件</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="xmlSerializer">Xml 序列化 功能 接口</param>
        /// <returns></returns>
        public static async Task<T> DeserializeFromXmlFileAsync<T>(string xmlFileFullPath, Encoding encoding = null, IXmlSerializer xmlSerializer = null) where T : class
        {
            return await GenericityHelper.GetInterface(xmlSerializer, DefaultXmlSerializer).DeserializeFromXmlFileAsync<T>(xmlFileFullPath, encoding);
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
            return GenericityHelper.GetInterface(binarySerializer, DefaultBinarySerializer).SerializeToBytes(t, encoding);
        }
        /// <summary>
        /// 异步 把对象序列化成二进制数据
        /// </summary>
        /// <param name="t">要序列化的对象</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="binarySerializer">二进制 序列化 功能 接口</param>
        /// <returns></returns>
        public static async Task<byte[]> SerializeToBytesAsync<T>(T t, Encoding encoding = null, IBinarySerializer binarySerializer = null) where T : class
        {
            return await GenericityHelper.GetInterface(binarySerializer, DefaultBinarySerializer).SerializeToBytesAsync(t, encoding);
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
            return GenericityHelper.GetInterface(binarySerializer, DefaultBinarySerializer).DeserializeFromBytes<T>(bytes, encoding);
        }
        /// <summary>
        /// 异步 反序列化二进制数据成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象 的 对象类型</typeparam>
        /// <param name="bytes">需要反序列化处理的二进制数据</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="binarySerializer">二进制 序列化 功能 接口</param>
        /// <returns></returns>
        public static async Task<T> DeserializeFromBytesAsync<T>(byte[] bytes, Encoding encoding = null, IBinarySerializer binarySerializer = null) where T : class
        {
            return await GenericityHelper.GetInterface(binarySerializer, DefaultBinarySerializer).DeserializeFromBytesAsync<T>(bytes, encoding);
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
            GenericityHelper.GetInterface(binarySerializer, DefaultBinarySerializer).SerializeToBytesFile(t, binaryFileFullPath, encoding);
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
        public static async Task SerializeToBytesFileAsync<T>(T t, string binaryFileFullPath, Encoding encoding = null, bool ifCompressBytes = true, IBinarySerializer binarySerializer = null) where T : class
        {
            await GenericityHelper.GetInterface(binarySerializer, DefaultBinarySerializer).SerializeToBytesFileAsync(t, binaryFileFullPath, encoding);
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
            return GenericityHelper.GetInterface(binarySerializer, DefaultBinarySerializer).DeserializeFromBytesFile<T>(binaryFileFullPath, encoding);
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
        public static async Task<T> DeserializeFromBytesFileAsync<T>(string binaryFileFullPath, Encoding encoding = null, bool ifDecompressBytes = true, IBinarySerializer binarySerializer = null) where T : class
        {
            return await GenericityHelper.GetInterface(binarySerializer, DefaultBinarySerializer).DeserializeFromBytesFileAsync<T>(binaryFileFullPath, encoding);
        }


        //public static byte[] SerializeToSourceBytes<T>(T t) where T : class
        //{
        //    using (IFormatter serializer = new BinaryFormatter())
        //    {

        //    }

        //    return null;
        //}


        //public static T DeserializeFromSourceBytes<T>(byte[] bytes) where T : class
        //{
        //    return default(T);
        //}



        #endregion


        #region CSV序列化

        ///// <summary>
        ///// 默认CSV序列化器
        ///// </summary>
        //public static readonly Dictionary<Type, object> DefaultCsvSerializer = new Dictionary<Type, object>();

        ///// <summary>
        ///// 获取默认的CSV序列化器
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //private static ICsvSerializer<T> GetDefaultCsvSerializer<T>() where T : class, new()
        //{
        //    Type type = typeof(T);
        //    if (!DefaultCsvSerializer.ContainsKey(type))
        //    {
        //        DefaultCsvSerializer.AddOrReplace(type, new LanymyCsvSerializer<T>());
        //    }
        //    return DefaultCsvSerializer[type] as ICsvSerializer<T>;
        //}

        /////// <summary>
        /////// 获取CSV序列化器
        /////// </summary>
        /////// <typeparam name="T"></typeparam>
        /////// <param name="csvSerializer"></param>
        /////// <returns></returns>
        ////private static ICsvSerializer<T> GetCsvSerializer<T>(ICsvSerializer<T> csvSerializer) where T : class, new()
        ////{
        ////    if (csvSerializer.IfIsNullOrEmpty())
        ////    {
        ////        csvSerializer = GetDefaultCsvSerializer<T>();
        ////    }
        ////    return csvSerializer;
        ////}

        //private static ICsvModelSerializer<T> GetCsvSerializer<T>(ICsvModelSerializer<T> csvModelSerializer = null) where T : class, new()
        //{
        //    if (csvModelSerializer.IfIsNullOrEmpty())
        //    {
        //        csvModelSerializer = GetDefaultCsvSerializer<T>();
        //    }
        //    return csvModelSerializer;
        //}

        //private static ICsvFileSerializer<T> GetCsvSerializer<T>(ICsvFileSerializer<T> csvFileSerializer = null) where T : class, new()
        //{
        //    if (csvFileSerializer.IfIsNullOrEmpty())
        //    {
        //        csvFileSerializer = GetDefaultCsvSerializer<T>();
        //    }
        //    return csvFileSerializer;
        //}


        ///// <summary>
        ///// 获取CSV数据的标题
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="csvModelSerializer">CSV 序列化 / 反序列化 功能 接口</param>
        ///// <returns></returns>
        //public static string GetCsvTitle<T>(ICsvModelSerializer<T> csvModelSerializer = null) where T : class, new()
        //{
        //    return GetCsvSerializer(csvModelSerializer).GetCsvTitle();
        //}

        ///// <summary>
        ///// 异步 获取CSV数据的标题
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="csvModelSerializer">CSV 序列化 / 反序列化 功能 接口</param>
        ///// <returns></returns>
        //public static Task<string> GetCsvTitleAsync<T>(ICsvModelSerializer<T> csvModelSerializer = null) where T : class, new()
        //{
        //    return GetCsvSerializer(csvModelSerializer).GetCsvTitleAsync();
        //}


        ///// <summary>
        ///// 实体类序列化成CSV
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="t"></param>
        ///// <param name="csvSerializer">CSV 序列化 / 反序列化 功能 接口</param>
        ///// <returns></returns>
        //public static string SerializeToCsv<T>(T t, ICsvModelSerializer<T> csvSerializer = null) where T : class, new()
        //{
        //    return GetCsvSerializer(csvSerializer).SerializeToCsv(t);
        //}

        ///// <summary>
        ///// 异步 实体类序列化成CSV
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="t"></param>
        ///// <param name="csvSerializer">CSV 序列化 / 反序列化 功能 接口</param>
        ///// <returns></returns>
        //public static Task<string> SerializeToCsvAsync<T>(T t, ICsvModelSerializer<T> csvSerializer = null) where T : class, new()
        //{
        //    return GetCsvSerializer(csvSerializer).SerializeToCsvAsync(t);
        //}


        ///// <summary>
        ///// 反序列化CSV成实体类
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="csvString"></param>
        ///// <param name="csvSerializer">CSV 序列化 / 反序列化 功能 接口</param>
        ///// <returns></returns>
        //public static T DeserializeFromCsv<T>(string csvString, ICsvModelSerializer<T> csvSerializer = null) where T : class, new()
        //{
        //    return GetCsvSerializer(csvSerializer).DeserializeFromCsv(csvString);
        //}


        ///// <summary>
        ///// 异步 反序列化CSV成实体类
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="csvString"></param>
        ///// <param name="csvSerializer">CSV 序列化 / 反序列化 功能 接口</param>
        ///// <returns></returns>
        //public static Task<T> DeserializeFromCsvAsync<T>(string csvString, ICsvModelSerializer<T> csvSerializer = null) where T : class, new()
        //{
        //    return GetCsvSerializer(csvSerializer).DeserializeFromCsvAsync(csvString);
        //}


        ///// <summary>
        ///// 序列化到CSV文件
        ///// </summary>
        ///// <param name="csvFileFullPath">CSV文件全路径</param>
        ///// <param name="list">要序列化的实体类数据集合</param>
        ///// <param name="ifWriteTitle">是否写入 标题 </param>
        ///// <param name="csvFileSerializer">CSV 序列化 文件 功能 接口</param>
        //public static void SerializeToCsvFile<T>(string csvFileFullPath, IEnumerable<T> list, bool ifWriteTitle = true, ICsvFileSerializer<T> csvFileSerializer = null) where T : class, new()
        //{
        //    GetCsvSerializer(csvFileSerializer).SerializeToCsvFile(csvFileFullPath, list, ifWriteTitle);
        //}


        ///// <summary>
        ///// 异步 序列化到CSV文件
        ///// </summary>
        ///// <param name="csvFileFullPath">CSV文件全路径</param>
        ///// <param name="list">要序列化的实体类数据集合</param>
        ///// <param name="ifWriteTitle">是否写入 标题 </param>
        ///// <param name="csvFileSerializer">CSV 序列化 文件 功能 接口</param>
        //public static Task SerializeToCsvFileAsync<T>(string csvFileFullPath, IEnumerable<T> list, bool ifWriteTitle = true, ICsvFileSerializer<T> csvFileSerializer = null) where T : class, new()
        //{
        //    return GetCsvSerializer(csvFileSerializer).SerializeToCsvFileAsync(csvFileFullPath, list, ifWriteTitle);
        //}
        ///// <summary>
        ///// 从CSV文件反序列化数据
        ///// </summary>
        ///// <param name="csvFileFullPath">CSV文件全路径</param>
        ///// <param name="csvAnnotationSymbol">行首 注释符 默认 '#'</param>
        ///// <param name="encoding">编码 null 则使用默认编码</param>
        ///// <param name="csvFileReader">CSV文件 数据 读取 功能接口</param>
        ///// <param name="csvFileSerializer">CSV 序列化 文件 功能 接口</param>
        ///// <returns></returns>
        //public static List<T> DeserializeFromCsvFile<T>(string csvFileFullPath, string csvAnnotationSymbol = GlobalSettings.CSV_ANNOTATION_SYMBOL, Encoding encoding = null, ICsvFileReader csvFileReader = null, ICsvFileSerializer<T> csvFileSerializer = null) where T : class, new()
        //{
        //    return GetCsvSerializer(csvFileSerializer).DeserializeFromCsvFile(csvFileFullPath, csvAnnotationSymbol, encoding, csvFileReader);
        //}
        ///// <summary>
        ///// 异步 从CSV文件反序列化数据
        ///// </summary>
        ///// <param name="csvFileFullPath">CSV文件全路径</param>
        ///// <param name="csvAnnotationSymbol">行首 注释符 默认 '#'</param>
        ///// <param name="encoding">编码 null 则使用默认编码</param>
        ///// <param name="csvFileReader">CSV文件 数据 读取 功能接口</param>
        ///// <param name="csvFileSerializer">CSV 序列化 文件 功能 接口</param>
        ///// <returns></returns>
        //public static Task<List<T>> DeserializeFromCsvFileAsync<T>(string csvFileFullPath, string csvAnnotationSymbol = GlobalSettings.CSV_ANNOTATION_SYMBOL, Encoding encoding = null, ICsvFileReader csvFileReader = null, ICsvFileSerializer<T> csvFileSerializer = null) where T : class, new()
        //{
        //    return GetCsvSerializer(csvFileSerializer).DeserializeFromCsvFileAsync(csvFileFullPath, csvAnnotationSymbol, encoding, csvFileReader);
        //}


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

        #region DataTable 序列化

        /// <summary>
        /// 序列化实体类数据集合成 DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="ifClearList">是否清空 数据源 集合 释放资源</param>
        /// <returns></returns>
        public static DataTable SerializeToDataTable<T>(List<T> list, bool ifClearList = false) where T : class
        {
            var dt = DeserializeFromJson<DataTable>(SerializeToJson(list));

            if (ifClearList)
            {
                list?.Clear();
            }

            return dt;
        }

        /// <summary>
        /// 反序列化 DataTable 成 实体类数据集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <param name="ifClearDataTable">是否清空 数据源 集合 释放资源</param>
        /// <returns></returns>
        public static List<T> DeserializeFromDataTable<T>(DataTable dt, bool ifClearDataTable = false) where T : class
        {
            var list = DeserializeFromJson<List<T>>(SerializeToJson(dt));

            if (ifClearDataTable)
            {
                dt?.Clear();
            }

            return list;
        }

        #endregion


        #region ProtoBuf 序列化、反序列化

        ///// <summary>
        ///// ProtoBuf 序列化
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="list"></param>
        ///// <returns></returns>
        //public static byte[] ProtoBufSerialize<T>(List<T> list) where T : class
        //{
        //    if (list.IfIsNullOrEmpty())
        //    {
        //        return null;
        //    }
        //    var type = list[0].GetType();
        //    ProtoBufAddRuntimeType(type, RuntimeTypeModel.Default);
        //    byte[] bytes = null;
        //    using (var ms = new System.IO.MemoryStream())
        //    {
        //        ProtoBuf.Serializer.Serialize(ms, list);
        //        bytes = ms.ToArray();
        //    }
        //    return bytes;
        //}

        //private static void ProtoBufAddRuntimeType(Type t, RuntimeTypeModel model)
        //{
        //    if (model.IsDefined(t))
        //    {
        //        return;
        //    }
        //    model.Add(t, false);
        //    var propertyInfos = t.GetProperties();
        //    foreach (var p in propertyInfos)
        //    {
        //        model[t].Add(p.Name);
        //        if (p.PropertyType.IsClass && p.PropertyType != typeof(string))
        //        {
        //            ProtoBufAddRuntimeType(p.PropertyType, model);
        //        }
        //    }
        //}


        ///// <summary>
        ///// ProtoBuf 反序列化
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="bytes"></param>
        ///// <returns></returns>
        //public static List<T> ProtoBufDeserialize<T>(byte[] bytes) where T : class
        //{
        //    if (bytes == null)
        //    {
        //        return null;
        //    }
        //    var type = typeof(T);
        //    ProtoBufAddRuntimeType(type, RuntimeTypeModel.Default);
        //    using (var memory = new System.IO.MemoryStream(bytes))
        //    {
        //        return ProtoBuf.Serializer.Deserialize<List<T>>(memory);
        //    }
        //}

        ///// <summary>
        ///// ProtoBuf 反序列化
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="stream"></param>
        ///// <returns></returns>
        //public static List<T> ProtoBufDeserialize<T>(System.IO.Stream stream) where T : class
        //{
        //    if (stream == null)
        //    {
        //        return null;
        //    }
        //    var type = typeof(T);
        //    ProtoBufAddRuntimeType(type, RuntimeTypeModel.Default);
        //    return ProtoBuf.Serializer.Deserialize<List<T>>(stream);
        //}

        #endregion

    }
}
