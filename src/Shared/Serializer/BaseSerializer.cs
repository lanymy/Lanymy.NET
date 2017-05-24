/********************************************************************

时间: 2017年05月24日, PM 09:52:48

作者: lanyanmiyu@qq.com

描述: 序列化器基类

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using Lanymy.General.Extension.ExtensionFunctions;

namespace Lanymy.General.Extension.Serializer
{

    /// <summary>
    /// 序列化器基类
    /// </summary>
    public abstract class BaseSerializer
    {
        /// <summary>
        /// 编码
        /// </summary>
        public readonly Encoding CurrentEncoding = GlobalSettings.DEFAULT_ENCODING;

        /// <summary>
        /// 序列化器 构造方法
        /// </summary>
        /// <param name="encoding">编码</param>
        protected BaseSerializer(Encoding encoding)
        {
            if (!encoding.IfIsNullOrEmpty())
            {
                CurrentEncoding = encoding;
            }
        }

    }
}
