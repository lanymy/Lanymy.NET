/********************************************************************

时间: 2016年11月11日, PM 03:56:41

作者: lanyanmiyu@qq.com

描述: 格式化辅助类

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Lanymy.General.Extension.ExtensionFunctions;

namespace Lanymy.General.Extension
{

    /// <summary>
    /// 格式化辅助类
    /// </summary>
    public class FormatFunctions
    {



        /// <summary>
        /// 格式化XML
        /// </summary>
        /// <param name="xmlStr"></param>
        /// <returns></returns>
        public static string FormatXml(string xmlStr)
        {

            if (xmlStr.IfIsNullOrEmpty()) return string.Empty;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlStr);

            return FormatXml(doc);
        }


        /// <summary>
        /// 格式化XML
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        public static string FormatXml(XmlDocument xmlDocument)
        {

            if (xmlDocument.IfIsNullOrEmpty()) return string.Empty;

            string result;

            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;
                    xmlDocument.WriteTo(writer);
                    result = sw.ToString();
                }
            }

            return result;

        }


        /// <summary>
        /// 格式化原始Base64字符串 成 合法的 Base64字符串文件名
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static string FormatBase64StringToFileNameBase64String(string base64String)
        {
            return base64String.Replace("/", "@");
        }

        /// <summary>
        /// 格式化 合法的 Base64字符串文件名 成 原始Base64 字符串
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static string FormatBase64StringFromFileNameBase64String(string base64String)
        {
            return base64String.Replace("@", "/");
        }


        /// <summary>
        /// 格式化原始Base64字符串 成 合法的 Base64字符串 文件夹名
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static string FormatBase64StringToDirectoryNameBase64String(string base64String)
        {
            return FormatBase64StringToFileNameBase64String(base64String);
        }

        /// <summary>
        /// 格式化 合法的 Base64文件夹名 成 原始Base64 字符串
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static string FormatBase64StringFromDirectoryNameBase64String(string base64String)
        {
            return FormatBase64StringFromFileNameBase64String(base64String);
        }


    }

}
