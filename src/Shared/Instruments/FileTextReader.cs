/********************************************************************

时间: 2017年06月13日, AM 07:53:32

作者: lanyanmiyu@qq.com

描述: 文本文件读取器

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
    /// 文本文件读取器
    /// </summary>
    public class FileTextReader: BaseFileTextReadOrWrite,IFileReader
    {

        /// <summary>
        /// 读数据流对象
        /// </summary>
        public StreamReader StreamReader { get; private set; }

        /// <summary>
        /// 文本文件读取器 构造方法
        /// </summary>
        /// <param name="textFileFullPath">文本文件全路径</param>
        /// <param name="encoding">编码 null 则使用默认编码</param>
        public FileTextReader(string textFileFullPath, Encoding encoding = null) : base(textFileFullPath, false, encoding)
        {
            StreamReader = new StreamReader(TextFileFullPath, CurrentEncoding);
        }


        /// <summary>
        /// 资源释放
        /// </summary>
        public override void Dispose()
        {
            if (!StreamReader.IfIsNullOrEmpty())
            {
                StreamReader.Dispose();
                StreamReader = null;
            }
        }

        /// <summary>
        /// 读取全部消息
        /// </summary>
        /// <returns></returns>
        public virtual string ReadAll()
        {
            return StreamReader.ReadToEnd();
        }

        /// <summary>
        /// 一行 一行 读取数据
        /// </summary>
        public virtual string ReadLine()
        {
            return StreamReader.ReadLine();
        }

        /// <summary>
        /// 判断是否还有字符串
        /// </summary>
        /// <returns></returns>
        public virtual bool IfHaveString()
        {
            return StreamReader.Peek() != -1;
        }

    }
}
