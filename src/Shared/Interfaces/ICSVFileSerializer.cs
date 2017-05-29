/********************************************************************

时间: 2017年05月29日, AM 09:54:04

作者: lanyanmiyu@qq.com

描述: CSV 序列化 文件 功能 接口

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.General.Extension.Interfaces
{


    /// <summary>
    /// CSV 序列化 文件 功能 接口
    /// </summary>
    public interface ICsvFileSerializer<TModel> where TModel : class, new()
    {

        /// <summary>
        /// 序列化到CSV文件
        /// </summary>
        /// <param name="csvFileFullPath">CSV文件全路径</param>
        /// <param name="list">要序列化的实体类数据集合</param>
        /// <param name="ifWriteTitle">是否写入 标题 </param>
        void SerializeToCsvFile(string csvFileFullPath, IEnumerable<TModel> list, bool ifWriteTitle = true);
       
        /// <summary>
        /// 异步 序列化到CSV文件
        /// </summary>
        /// <param name="csvFileFullPath">CSV文件全路径</param>
        /// <param name="list">要序列化的实体类数据集合</param>
        /// <param name="ifWriteTitle">是否写入 标题 </param>
        Task SerializeToCsvFileAsync(string csvFileFullPath, IEnumerable<TModel> list, bool ifWriteTitle = true);

        /// <summary>
        /// 从CSV文件反序列化数据
        /// </summary>
        /// <param name="csvFileFullPath">CSV文件全路径</param>
        /// <param name="csvAnnotationSymbol">行首 注释符 默认 '#'</param>
        /// <returns></returns>
        List<TModel> DeserializeFromCsvFile(string csvFileFullPath, string csvAnnotationSymbol = GlobalSettings.CSV_ANNOTATION_SYMBOL);

        /// <summary>
        /// 异步 从CSV文件反序列化数据
        /// </summary>
        /// <param name="csvFileFullPath">CSV文件全路径</param>
        /// <param name="csvAnnotationSymbol">行首 注释符 默认 '#'</param>
        /// <returns></returns>
        Task<List<TModel>> DeserializeFromCsvFileAsync(string csvFileFullPath, string csvAnnotationSymbol = GlobalSettings.CSV_ANNOTATION_SYMBOL);


    }

}
