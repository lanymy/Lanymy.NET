/********************************************************************

时间: 2017年5月10日, PM 04:49:06

作者: lanyanmiyu@qq.com

描述: 基于Json.Net的Json序列化器

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Interfaces;
using Newtonsoft.Json;

namespace Lanymy.General.Extension.Serializer
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
        public const string DATE_FORMAT_STRING = GlobalSettings.DEFAULT_DATE_FORMAT_STRING;

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
            return new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatString = DATE_FORMAT_STRING,
            };
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
        public virtual Task<string> SerializeToJsonAsync<T>(T t) where T : class
        {

#if NET40
            return new Task<string>(() => SerializeToJson(t));
#else
            return Task.FromResult(SerializeToJson(t));
#endif

        }

        /// <summary>
        /// 异步反序列化Json成对象
        /// </summary>
        /// <typeparam name="T">反序列化成对象的对象类型</typeparam>
        /// <param name="json">要反序列化的Json字符串</param>
        /// <returns></returns>
        public virtual Task<T> DeserializeFromJsonAsync<T>(string json) where T : class
        {
#if NET40
            return new Task<T>(() => DeserializeFromJson<T>(json));
#else
            return Task.FromResult(DeserializeFromJson<T>(json));
#endif
        }

    }



}
