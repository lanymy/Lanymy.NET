/********************************************************************

时间: 2016年05月19日, PM 02:47:10

作者: lanyanmiyu@qq.com

描述: CSV 序列化 / 反序列化 接口

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
    /// CSV 序列化 / 反序列化 接口
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface ICSVSerialize<TModel>
    {

        /// <summary>
        /// 获取CSV数据的标题
        /// </summary>
        /// <returns></returns>
        string GetCSVTitle();

        /// <summary>
        /// 实体类序列化成CSV
        /// </summary>
        /// <param name="tModel"></param>
        /// <returns></returns>
        string SerializeToCSV(TModel tModel);

        /// <summary>
        /// 反序列化CSV成实体类
        /// </summary>
        /// <param name="csvString"></param>
        /// <returns></returns>
        TModel DeserializeFromCSV(string csvString);

    }


}
