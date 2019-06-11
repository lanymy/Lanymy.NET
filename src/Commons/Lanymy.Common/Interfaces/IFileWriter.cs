using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.Common.Interfaces
{
    /// <summary>
    /// 写文件  功能 接口
    /// </summary>
    public interface IFileWriter
    {

        /// <summary>
        /// 写消息
        /// </summary>
        /// <param name="message"></param>
        void Write(string message);


        /// <summary>
        /// 写一行消息
        /// </summary>
        /// <param name="message"></param>
        void WriteLine(string message);

    }
}
