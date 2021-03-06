﻿using System;

namespace Lanymy.Common.Abstractions.Interfaces.IKeyValues
{
    /// <summary>
    /// Key Value  操作 Key指定为枚举 扩展功能 接口
    /// </summary>
    public interface IEnumKeyValue
    {


        ///// <summary>
        ///// 获取 枚举项  默认Key 值
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //string GetEnumDefaultKey(Enum key);


        /// <summary>
        /// 获取 枚举项  默认Key 值   
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetEnumDefaultKey<T>(Enum key);


        ///// <summary>
        ///// 是否存在Key
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //bool IfHaveKey(Enum key);


        /// <summary>
        /// 是否存在Key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IfHaveKey<T>(Enum key);



        /// <summary>
        /// 获取Key的Value值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetValue<T>(Enum key);



        /// <summary>
        /// 设置Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="t"></param>
        void SetValue<T>(Enum key, T t);


        /// <summary>
        /// 删除Key
        /// </summary>
        /// <param name="key"></param>
        void RemoveValue<T>(Enum key);


    }
}
