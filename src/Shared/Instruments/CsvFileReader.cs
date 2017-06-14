/********************************************************************

时间: 2017年06月13日, AM 08:27:48

作者: lanyanmiyu@qq.com

描述: CSV文件读取器

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Interfaces;

namespace Lanymy.General.Extension.Instruments
{

    /// <summary>
    /// CSV文件读取器
    /// </summary>
    public class CsvFileReader : FileTextReader, ICsvFileReader
    {

        /// <summary>
        /// CSV 开头 的 忽略 或者 注释符 ,默认值 '#'
        /// </summary>
        public string CsvAnnotationSymbol { get; } = GlobalSettings.CSV_ANNOTATION_SYMBOL;

        /// <summary>
        /// CSV文件读取器
        /// </summary>
        /// <param name="csvFileFullPath">CSV文件全路径</param>
        /// <param name="csvAnnotationSymbol">CSV 开头 的 忽略 或者 注释符 ,默认值 '#'</param>
        /// <param name="encoding">编码 null 则使用默认编码</param>
        public CsvFileReader(string csvFileFullPath, string csvAnnotationSymbol = null, Encoding encoding = null) : base(csvFileFullPath, encoding)
        {
            if (!csvAnnotationSymbol.IfIsNullOrEmpty())
            {
                CsvAnnotationSymbol = csvAnnotationSymbol;
            }
        }



        /// <summary>
        /// 读取CSV文件
        /// </summary>
        public virtual IEnumerable<string> ReadCsvFile()
        {

            if (StreamReader.IfIsNullOrEmpty())
            {
                StreamReader = new StreamReader(TextFileFullPath, CurrentEncoding);
            }


            if (!IfHaveString())
            {
                yield break;
            }

            string strLine = string.Empty;

            while (IfHaveString())
            {
                strLine = ReadLine();

                if (strLine.IfIsNullOrEmpty() || strLine.StartsWith(CsvAnnotationSymbol))
                {
                    continue;
                }

                yield return strLine;
            }

            Dispose();

        }


    }

}
