/********************************************************************

时间: 2017年06月12日, AM 10:19:23

作者: lanyanmiyu@qq.com

描述: Key Value 操作 功能 接口

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.General.Extension.Interfaces
{


    /// <summary>
    /// Key Value 操作 功能 接口
    /// </summary>
    public interface IKeyValue
    {

        /// <summary>
        /// Key是否存在
        /// </summary>
        /// <param name="key">Key值</param>
        /// <returns></returns>
        bool IfHaveKey(string key);

        /// <summary>
        /// 设置Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetValue(string key, object value);


        /// <summary>
        /// 获取Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetValue(string key);

        /// <summary>
        /// 删除Key
        /// </summary>
        void RemoveValue(string key);




    }


}
