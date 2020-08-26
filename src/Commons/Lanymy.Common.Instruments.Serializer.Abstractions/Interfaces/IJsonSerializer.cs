using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Interfaces
{
    /// <summary>
    /// Json序列化器接口
    /// </summary>
    public interface IJsonSerializer
    {


        /// <summary>
        /// 对象序列化成Json
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="t">要序列化的对象</param>
        /// <returns></returns>
        string SerializeToJson<T>(T t) where T : class;


        /// <summary>
        /// 反序列化Json成对象
        /// </summary>
        /// <typeparam name="T">反序列化成对象的对象类型</typeparam>
        /// <param name="json">要反序列化的Json字符串</param>
        /// <returns></returns>
        T DeserializeFromJson<T>(string json) where T : class;


        /// <summary>
        /// 异步对象序列化成Json
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="t">要序列化的对象</param>
        /// <returns></returns>
        Task<string> SerializeToJsonAsync<T>(T t) where T : class;


        /// <summary>
        /// 异步反序列化Json成对象
        /// </summary>
        /// <typeparam name="T">反序列化成对象的对象类型</typeparam>
        /// <param name="json">要反序列化的Json字符串</param>
        /// <returns></returns>
        Task<T> DeserializeFromJsonAsync<T>(string json) where T : class;


        /// <summary>
        /// 序列化对象成JSON文件
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="jsonFileFullPath">要保存序列化成的JSON文件全路径</param>
        /// <param name="t">要序列化的对象实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        void SerializeToJsonFile<T>(T t, string jsonFileFullPath, Encoding encoding = null) where T : class;

        /// <summary>
        /// 异步 序列化对象成JSON文件
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="jsonFileFullPath">要保存序列化成的JSON文件全路径</param>
        /// <param name="t">要序列化的对象实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        Task SerializeToJsonFileAsync<T>(T t, string jsonFileFullPath, Encoding encoding = null) where T : class;


        /// <summary>
        /// 反序列化JSON文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="jsonFileFullPath">要反序列化处理的JSON文件</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        T DeserializeFromJsonFile<T>(string jsonFileFullPath, Encoding encoding = null) where T : class;
        /// <summary>
        /// 异步 反序列化JSON文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="jsonFileFullPath">要反序列化处理的JSON文件</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        Task<T> DeserializeFromJsonFileAsync<T>(string jsonFileFullPath, Encoding encoding = null) where T : class;



    }
}
