using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.Interfaces;

namespace Lanymy.Common.Helpers
{

    public class JsonSerializeHelper
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
    }
}
