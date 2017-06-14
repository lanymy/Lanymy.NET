/********************************************************************

时间: 2017年06月14日, AM 07:34:04

作者: lanyanmiyu@qq.com

描述: 独立存储 Model 功能 接口

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.General.Extension.Interfaces
{

    /// <summary>
    /// 独立存储 Model 功能 接口
    /// </summary>
    public interface IIsolatedStorageModel
    {
        /// <summary>
        /// 指定文件名 指定编码 指定密钥 保存实体类到独立存储区中
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="t">实体实例</param>
        /// <param name="token">序列化标识 (Null 表示使用 默认 标识名)</param>
        /// <param name="encoding">编码 (Null 表示使用 默认 编码)</param>
        /// <param name="securityKey">密钥 (Null 表示使用 默认 密钥)</param>
        void SaveModel<T>(T t, string token = null, string securityKey = null, Encoding encoding = null) where T : class;
        /// <summary>
        ///从独立存储区中获取实体类
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="token">序列化标识 (Null 表示使用 默认 标识名)</param>
        /// <param name="encoding">编码 (Null 表示使用 默认 编码)</param>
        /// <param name="securityKey">密钥 (Null 表示使用 默认 密钥)</param>
        /// <returns></returns>
        T GetModel<T>(string token = null, string securityKey = null, Encoding encoding = null) where T : class;


    }
}
