/********************************************************************

时间: 2017年06月13日, AM 07:40:48

作者: lanyanmiyu@qq.com

描述: 写文件  功能 接口

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.General.Extension.Interfaces
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
