/********************************************************************

时间: 2017年05月29日, AM 10:17:37

作者: lanyanmiyu@qq.com

描述: CSV文件 数据 读取 功能接口

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.General.Extension.Interfaces
{


    /// <summary>
    /// CSV文件 数据 读取 功能接口
    /// </summary>
    public interface ICsvFileReader
    {

        /// <summary>
        /// 读取CSV文件
        /// </summary>
        /// <param name="csvFileFullPath">CSV文件全路径</param>
        /// <param name="csvAnnotationSymbol">CSV 开头 的 忽略 或者 注释符 ,默认值 '#'</param>
        /// <returns></returns>
        IEnumerable<string> ReadCsvFile(string csvFileFullPath, string csvAnnotationSymbol = GlobalSettings.CSV_ANNOTATION_SYMBOL);

    }


}
