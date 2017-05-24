/********************************************************************

时间: 2015年10月23日, AM 10:23:13

作者: lanyanmiyu@qq.com

描述: 自定义属性特性,用于扩展枚举项的描述

其它:     

********************************************************************/




using System;

namespace Lanymy.General.Extension.CustomAttributes
{


    /// <summary>
    /// 自定义属性特性,用于扩展枚举项的描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class EnumCustomAttribute : Attribute
    {

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }



        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }


        ///// <summary>
        ///// 其它附加信息
        ///// </summary>
        //public object Other { get; set; }


        /// <summary>
        /// 枚举自定义特性构造方法
        /// </summary>
        public EnumCustomAttribute()
        {

        }

        /// <summary>
        /// 枚举自定义特性构造方法
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        ///// <param name="other">其它附加信息</param>
        public EnumCustomAttribute(string title, string description = null)
        {
            Title = title;
            Description = description;
            //Other = other;
        }


    }
}
