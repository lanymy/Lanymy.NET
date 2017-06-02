/********************************************************************

时间: 2017年05月29日, AM 09:53:22

作者: lanyanmiyu@qq.com

描述: CSV 序列化 Model 功能 接口

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.General.Extension.Interfaces
{


    /// <summary>
    /// CSV 序列化 Model 功能 接口
    /// </summary>
    public interface ICsvModelSerializer<TModel> where TModel : class, new()
    {

        /// <summary>
        /// 获取CSV数据的标题
        /// </summary>
        /// <returns></returns>
        string GetCsvTitle();

        /// <summary>
        /// 异步 获取CSV数据的标题
        /// </summary>
        /// <returns></returns>
        Task<string> GetCsvTitleAsync();

        /// <summary>
        /// 实体类序列化成CSV
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        string SerializeToCsv(TModel t);
        /// <summary>
        /// 异步 实体类序列化成CSV
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<string> SerializeToCsvAsync(TModel t);

        /// <summary>
        /// 反序列化CSV成实体类
        /// </summary>
        /// <param name="csvString"></param>
        /// <returns></returns>
        TModel DeserializeFromCsv(string csvString);
        /// <summary>
        /// 异步 反序列化CSV成实体类
        /// </summary>
        /// <param name="csvString"></param>
        /// <returns></returns>
        Task<TModel> DeserializeFromCsvAsync(string csvString);
    }


}
