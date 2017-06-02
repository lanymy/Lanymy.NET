// *******************************************************************
// 创建时间：2015年01月14日, AM 10:38:02
// 作者：lanyanmiyu@qq.com
// 说明：CSV 转换
// 其它:
// *******************************************************************


using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Interfaces;
using Lanymy.General.Extension.Models;

namespace Lanymy.General.Extension
{



    /// <summary>
    /// CSV 转换
    /// </summary>
    public class CsvFunctions
    {

        ///// <summary>
        ///// 导出报表为Csv
        ///// </summary>
        ///// <param name="dt">DataTable</param>
        ///// <param name="strFilePath">物理路径</param>
        ///// <param name="tableheader">表头</param>
        ///// <param name="columname">字段标题,逗号分隔</param>
        //public static bool DataTableToCsv(DataTable dt, string strFilePath, string tableheader, string columname)
        //{
        //    try
        //    {
        //        string strBufferLine = "";
        //        StreamWriter strmWriterObj = new StreamWriter(strFilePath, false, System.Text.Encoding.UTF8);
        //        strmWriterObj.WriteLine(tableheader);
        //        strmWriterObj.WriteLine(columname);
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            strBufferLine = "";
        //            for (int j = 0; j < dt.Columns.Count; j++)
        //            {
        //                if (j > 0)
        //                    strBufferLine += ",";
        //                strBufferLine += dt.Rows[i][j].ToString();
        //            }
        //            strmWriterObj.WriteLine(strBufferLine);
        //        }
        //        strmWriterObj.Close();
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// 将Csv读入DataTable
        ///// </summary>
        ///// <param name="filePath">csv文件路径</param>
        ///// <param name="n">表示第n行是字段title,第n+1行是记录开始</param>
        //public static DataTable CsvToDataTable(string filePath, int n, DataTable dt)
        //{
        //    StreamReader reader = new StreamReader(filePath, System.Text.Encoding.UTF8, false);
        //    int i = 0, m = 0;
        //    reader.Peek();
        //    while (reader.Peek() > 0)
        //    {
        //        m = m + 1;
        //        string str = reader.ReadLine();
        //        if (m >= n + 1)
        //        {
        //            string[] split = str.Split(',');

        //            System.Data.DataRow dr = dt.NewRow();
        //            for (i = 0; i < split.Length; i++)
        //            {
        //                dr[i] = split[i];
        //            }
        //            dt.Rows.Add(dr);
        //        }
        //    }
        //    return dt;
        //}


        //public static IEnumerable<string> ReadCSVFile(string csvFileFullPath, string csvAnnotationSymbol = GlobalSettings.CSV_ANNOTATION_SYMBOL)


        /// <summary>
        /// 默认CSV文件读取器
        /// </summary>
        public static readonly ICsvFileReader DefaultCsvFileReader = new CsvFileReader();

        /// <summary>
        /// 读取CSV文件
        /// </summary>
        /// <param name="csvFileFullPath">CSV文件全路径</param>
        /// <param name="csvAnnotationSymbol">CSV 开头 的 忽略 或者 注释符 ,默认值 '#'</param>
        /// <param name="csvFileReader">CSV文件 数据 读取 功能接口</param>
        /// <returns></returns>
        public static IEnumerable<string> ReadCsvFile(string csvFileFullPath, string csvAnnotationSymbol = GlobalSettings.CSV_ANNOTATION_SYMBOL, ICsvFileReader csvFileReader = null)
        {
            return GenericityFunctions.GetInterface(csvFileReader, DefaultCsvFileReader).ReadCsvFile(csvFileFullPath, csvAnnotationSymbol);
        }





    }


}
