/********************************************************************

时间: 2017年5月10日, PM 04:05:12

作者: lanyanmiyu@qq.com

描述: Json序列化器接口

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.General.Extension.Interfaces
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


    }


}
