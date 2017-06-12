/********************************************************************

时间: 2017年06月12日, AM 10:36:17

作者: lanyanmiyu@qq.com

描述: Key Value  操作 Key指定为枚举 扩展功能 接口

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.General.Extension.Interfaces
{


    /// <summary>
    /// Key Value  操作 Key指定为枚举 扩展功能 接口
    /// </summary>
    public interface IEnumKeyValue
    {

        /// <summary>
        /// 是否存在Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IfHaveKey(Enum key);


        /// <summary>
        /// 设置Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="t"></param>
        void SetValue<T>(Enum key,T t);


        /// <summary>
        /// 获取Key的Value值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetValue<T>(Enum key);


        /// <summary>
        /// 删除Key
        /// </summary>
        /// <param name="key"></param>
        void RemoveValue(Enum key);

         
    }


}
