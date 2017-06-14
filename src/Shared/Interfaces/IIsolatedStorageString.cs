/********************************************************************

时间: 

作者: lanyanmiyu@qq.com

描述: 独立存储 字符串 功能 接口

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.General.Extension.Interfaces
{

    /// <summary>
    /// 独立存储 字符串 功能 接口
    /// </summary>
    public interface IIsolatedStorageString
    {
        /// <summary>
        /// 序列化 字符串 到 持久化文件
        /// </summary>
        /// <param name="sourceString">要序列化的字符串</param>
        /// <param name="token">持久化标识</param>
        /// <param name="encoding">编码 (Null表示使用 默认编码)</param>
        /// <param name="securityKey">密钥 (Null表示使用 默认密钥)</param>
        void SaveString(string sourceString, string token, string securityKey = null, Encoding encoding = null);
        /// <summary>
        /// 从持久化文件 获取 字符串
        /// </summary>
        /// <param name="token">持久化标识</param>
        /// <param name="encoding">编码 (Null表示使用 默认编码)</param>
        /// <param name="securityKey">密钥 (Null表示使用 默认密钥)</param>
        /// <returns></returns>
        string GetString(string token, string securityKey = null, Encoding encoding = null);

    }
}
