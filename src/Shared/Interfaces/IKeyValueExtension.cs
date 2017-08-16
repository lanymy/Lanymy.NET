/********************************************************************

时间: 2017年06月12日, AM 10:33:25

作者: lanyanmiyu@qq.com

描述: Key Value  操作 扩展功能 接口

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.General.Extension.Interfaces
{


    /// <summary>
    /// Key Value  操作 扩展功能 接口
    /// </summary>
    public interface IKeyValueExtension
    {


        /// <summary>
        /// 获取默认Key值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetDefaultKey<T>();

        /// <summary>
        /// 获取默认Key值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetDefaultKey<T>(string key);


        /// <summary>
        /// 默认Key 是否存在
        /// </summary>
        /// <returns></returns>
        bool IfHaveKey<T>();


        /// <summary>
        /// key是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IfHaveKey<T>(string key);


        /// <summary>
        /// 使用 默认Key 获取Key 的 Value值
        /// </summary>
        /// <returns></returns>
        T GetValue<T>();


        /// <summary>
        /// 获取Key的Value值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetValue<T>(string key);



        /// <summary>
        /// 使用 默认Key 设置 Key的Value值
        /// </summary>
        /// <param name="value"></param>
        void SetValue<T>(T value);

        /// <summary>
        /// 根据 Key 设置 Value值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetValue<T>(string key, T value);


        /// <summary>
        /// 使用 默认Key 删除Key
        /// </summary>
        void RemoveValue<T>();

        /// <summary>
        /// 删除Key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        void RemoveValue<T>(string key);

    }


}
