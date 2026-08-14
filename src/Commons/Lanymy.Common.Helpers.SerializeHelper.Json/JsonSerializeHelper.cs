using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Helpers.ResultModels;
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
        public static Task<string> SerializeToJsonAsync<T>(T t, IJsonSerializer jsonSerializer = null) where T : class
        {
            return GenericityHelper.GetInterface(jsonSerializer, DefaultJsonSerializer).SerializeToJsonAsync(t);
        }



        /// <summary>
        /// 异步反序列化Json成对象
        /// </summary>
        /// <param name="json">字符串序列</param>
        /// <param name="jsonSerializer">反序列化Json使用的序列化器</param>
        public static Task<T> DeserializeFromJsonAsync<T>(string json, IJsonSerializer jsonSerializer = null) where T : class
        {
            return GenericityHelper.GetInterface(jsonSerializer, DefaultJsonSerializer).DeserializeFromJsonAsync<T>(json);
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
            var result = SerializeToJsonFileWithResult(t, jsonFileFullPath, encoding, jsonSerializer);
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }
        }

        /// <summary>
        /// 序列化对象成 JSON 文件，并返回详细结果。
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="t">要序列化的对象实例</param>
        /// <param name="jsonFileFullPath">要保存序列化成的 JSON 文件全路径</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="jsonSerializer">序列化 Json 使用的序列化器</param>
        /// <returns>JSON 文件序列化结果</returns>
        public static JsonFileSerializationResultModel<T> SerializeToJsonFileWithResult<T>(T t, string jsonFileFullPath, Encoding encoding = null, IJsonSerializer jsonSerializer = null) where T : class
        {
            var result = new JsonFileSerializationResultModel<T>
            {
                FilePath = jsonFileFullPath,
                Model = t,
            };

            try
            {
                var currentSerializer = GenericityHelper.GetInterface(jsonSerializer, DefaultJsonSerializer);
                result.JsonContent = currentSerializer.SerializeToJson(t);
                currentSerializer.SerializeToJsonFile(t, jsonFileFullPath, encoding);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
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
            var result = await SerializeToJsonFileWithResultAsync(t, jsonFileFullPath, encoding, jsonSerializer);
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }
        }

        /// <summary>
        /// 异步序列化对象成 JSON 文件，并返回详细结果。
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="t">要序列化的对象实例</param>
        /// <param name="jsonFileFullPath">要保存序列化成的 JSON 文件全路径</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="jsonSerializer">序列化 Json 使用的序列化器</param>
        /// <returns>JSON 文件序列化结果</returns>
        public static async Task<JsonFileSerializationResultModel<T>> SerializeToJsonFileWithResultAsync<T>(T t, string jsonFileFullPath, Encoding encoding = null, IJsonSerializer jsonSerializer = null) where T : class
        {
            var result = new JsonFileSerializationResultModel<T>
            {
                FilePath = jsonFileFullPath,
                Model = t,
            };

            try
            {
                var currentSerializer = GenericityHelper.GetInterface(jsonSerializer, DefaultJsonSerializer);
                result.JsonContent = currentSerializer.SerializeToJson(t);
                await currentSerializer.SerializeToJsonFileAsync(t, jsonFileFullPath, encoding);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
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
        /// 反序列化 JSON 文件成对象，并返回详细结果。
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的对象类型</typeparam>
        /// <param name="jsonFileFullPath">要反序列化处理的 JSON 文件</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="jsonSerializer">反序列化 Json 使用的序列化器</param>
        /// <returns>JSON 文件反序列化结果</returns>
        public static JsonFileSerializationResultModel<T> DeserializeFromJsonFileWithResult<T>(string jsonFileFullPath, Encoding encoding = null, IJsonSerializer jsonSerializer = null) where T : class
        {
            var result = new JsonFileSerializationResultModel<T>
            {
                FilePath = jsonFileFullPath,
            };

            try
            {
                var currentSerializer = GenericityHelper.GetInterface(jsonSerializer, DefaultJsonSerializer);
                if (!File.Exists(jsonFileFullPath))
                {
                    throw new FileNotFoundException("JSON file does not exist.", jsonFileFullPath);
                }

                result.Model = currentSerializer.DeserializeFromJsonFile<T>(jsonFileFullPath, encoding);
                result.JsonContent = currentSerializer.SerializeToJson(result.Model);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
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

        /// <summary>
        /// 异步反序列化 JSON 文件成对象，并返回详细结果。
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的对象类型</typeparam>
        /// <param name="jsonFileFullPath">要反序列化处理的 JSON 文件</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="jsonSerializer">反序列化 Json 使用的序列化器</param>
        /// <returns>JSON 文件反序列化结果</returns>
        public static async Task<JsonFileSerializationResultModel<T>> DeserializeFromJsonFileWithResultAsync<T>(string jsonFileFullPath, Encoding encoding = null, IJsonSerializer jsonSerializer = null) where T : class
        {
            var result = new JsonFileSerializationResultModel<T>
            {
                FilePath = jsonFileFullPath,
            };

            try
            {
                var currentSerializer = GenericityHelper.GetInterface(jsonSerializer, DefaultJsonSerializer);
                if (!File.Exists(jsonFileFullPath))
                {
                    throw new FileNotFoundException("JSON file does not exist.", jsonFileFullPath);
                }

                result.Model = await currentSerializer.DeserializeFromJsonFileAsync<T>(jsonFileFullPath, encoding);
                result.JsonContent = currentSerializer.SerializeToJson(result.Model);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }



        #endregion
    }
}
