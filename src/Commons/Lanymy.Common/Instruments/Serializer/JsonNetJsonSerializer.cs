using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.FileTextReadOrWrite;
using Lanymy.Common.Interfaces.ISerializers;
using Newtonsoft.Json;

namespace Lanymy.Common.Instruments.Serializer
{
    /// <summary>
    /// 基于Json.Net的Json序列化器
    /// </summary>
    public class JsonNetJsonSerializer : IJsonSerializer
    {

        //public static readonly JsonSerializerSettings DEFAULT_JSON_SERIALIZER_SETTINGS = new JsonSerializerSettings
        //{
        //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        //    DateFormatString = DATE_FORMAT_STRING,
        //};

        /// <summary>
        /// 默认时间格式化字符串yyyy-MM-dd hh:mm:ss.fff
        /// </summary>
        public const string DATE_FORMAT_STRING = DateTimeFormatKeys.DATE_TIME_FORMAT_1;

        /// <summary>
        /// 序列化Json使用的格式化配置属性
        /// </summary>
        public readonly JsonSerializerSettings CurrentJsonSerializerSettings;



        //public JsonNetJsonSerializer()
        //{
        //    //CurrentJsonSerializerSettings = GetDefaultJsonSerializerSettings();
        //}

        /// <summary>
        /// Json序列化器 构造方法
        /// </summary>
        /// <param name="jsonSerializerSettings">格式化配置属性</param>
        public JsonNetJsonSerializer(JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (!jsonSerializerSettings.IfIsNullOrEmpty())
            {
                CurrentJsonSerializerSettings = jsonSerializerSettings;
            }
        }


        /// <summary>
        /// 获取默认的Json格式化配置属性
        /// </summary>
        /// <returns></returns>
        public static JsonSerializerSettings GetDefaultJsonSerializerSettings()
        {

            var jsonSerializerSettings = new JsonSerializerSettings
            {

                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatString = DATE_FORMAT_STRING,
                //Formatting = Formatting.Indented,
                //config.Formatters.JsonFormatter.SerializerSettings.Converters.Add

            };

            //jsonSerializerSettings.Converters.Add(new StringEnumConverter());

            return jsonSerializerSettings;

        }


        /// <summary>
        /// 对象序列化成Json
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="t">要序列化的对象</param>
        /// <returns></returns>
        public virtual string SerializeToJson<T>(T t) where T : class
        {
            return t.IfIsNullOrEmpty() ? string.Empty : CurrentJsonSerializerSettings.IfIsNullOrEmpty() ? JsonConvert.SerializeObject(t) : JsonConvert.SerializeObject(t, CurrentJsonSerializerSettings);
        }

        /// <summary>
        /// 反序列化Json成对象
        /// </summary>
        /// <typeparam name="T">反序列化成对象的对象类型</typeparam>
        /// <param name="json">要反序列化的Json字符串</param>
        /// <returns></returns>
        public virtual T DeserializeFromJson<T>(string json) where T : class
        {
            return json.IfIsNullOrEmpty() ? default(T) : CurrentJsonSerializerSettings.IfIsNullOrEmpty() ? JsonConvert.DeserializeObject<T>(json) : JsonConvert.DeserializeObject<T>(json, CurrentJsonSerializerSettings);
        }

        /// <summary>
        /// 异步对象序列化成Json
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="t">要序列化的对象</param>
        /// <returns></returns>
        public virtual async Task<string> SerializeToJsonAsync<T>(T t) where T : class
        {
            return await GenericityHelper.DoTaskWorkAsync(SerializeToJson, t);
        }

        /// <summary>
        /// 异步反序列化Json成对象
        /// </summary>
        /// <typeparam name="T">反序列化成对象的对象类型</typeparam>
        /// <param name="json">要反序列化的Json字符串</param>
        /// <returns></returns>
        public virtual async Task<T> DeserializeFromJsonAsync<T>(string json) where T : class
        {
            return await GenericityHelper.DoTaskWorkAsync(DeserializeFromJson<T>, json);
        }

        /// <summary>
        /// 序列化对象成JSON文件
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="jsonFileFullPath">要保存序列化成的JSON文件全路径</param>
        /// <param name="t">要序列化的对象实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        public virtual void SerializeToJsonFile<T>(T t, string jsonFileFullPath, Encoding encoding = null) where T : class
        {
            using (var writer = new FileTextWriter(jsonFileFullPath, true, encoding))
            {
                writer.Write(SerializeToJson(t));
            }
        }

        /// <summary>
        /// 异步 序列化对象成JSON文件
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="jsonFileFullPath">要保存序列化成的JSON文件全路径</param>
        /// <param name="t">要序列化的对象实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        public virtual async Task SerializeToJsonFileAsync<T>(T t, string jsonFileFullPath, Encoding encoding = null) where T : class
        {
            await GenericityHelper.DoTaskWorkAsync(SerializeToJsonFile<T>, t, jsonFileFullPath, encoding);
        }

        /// <summary>
        /// 反序列化JSON文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="jsonFileFullPath">要反序列化处理的JSON文件</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual T DeserializeFromJsonFile<T>(string jsonFileFullPath, Encoding encoding = null) where T : class
        {
            T t;

            using (var reader = new FileTextReader(jsonFileFullPath, encoding))
            {
                t = DeserializeFromJson<T>(reader.ReadAll());
            }

            return t;
        }

        /// <summary>
        /// 异步 反序列化JSON文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="jsonFileFullPath">要反序列化处理的JSON文件</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual async Task<T> DeserializeFromJsonFileAsync<T>(string jsonFileFullPath, Encoding encoding = null) where T : class
        {
            return await GenericityHelper.DoTaskWorkAsync(DeserializeFromJsonFile<T>, jsonFileFullPath, encoding);
        }
    }
}
