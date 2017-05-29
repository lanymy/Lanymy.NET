/********************************************************************

时间: 2017年05月29日, AM 10:20:02

作者: lanyanmiyu@qq.com

描述: CSV文件 数据 读取器

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Interfaces;

namespace Lanymy.General.Extension.Models
{
    /// <summary>
    /// CSV文件 数据 读取器
    /// </summary>
    public class CsvFileReader: ICsvFileReader
    {

        /// <summary>
        /// 读取CSV文件
        /// </summary>
        /// <param name="csvFileFullPath">CSV文件全路径</param>
        /// <param name="csvAnnotationSymbol">CSV 开头 的 忽略 或者 注释符 ,默认值 '#'</param>
        /// <returns></returns>
        public virtual IEnumerable<string> ReadCsvFile(string csvFileFullPath, string csvAnnotationSymbol = GlobalSettings.CSV_ANNOTATION_SYMBOL)
        {
            if (!File.Exists(csvFileFullPath))
                throw new FileNotFoundException(csvFileFullPath);

            using (FileReadWriteHelper reader = new FileReadWriteHelper(csvFileFullPath))
            {
                if (!reader.IfHaveString())
                {
                    yield break;
                }

                string strLine = string.Empty;

                while (reader.IfHaveString())
                {

                    strLine = reader.ReadLine();

                    if (strLine.IfIsNullOrEmpty() || strLine.StartsWith(csvAnnotationSymbol))
                    {
                        continue;
                    }

                    yield return strLine;

                }
            }
        }
    }
}
