/********************************************************************

时间: 2015年10月23日, AM 10:22:10

作者: lanyanmiyu@qq.com

描述: 枚举单项实体类

其它:     

********************************************************************/



using System;
using Lanymy.General.Extension.CustomAttributes;

namespace Lanymy.General.Extension.Models
{


    /// <summary>
    /// 枚举单项实体类
    /// </summary>
    /// <typeparam name="TEnumCustomAttribute">枚举自定义扩展标记</typeparam>
    public class EnumItem<TEnumCustomAttribute> where TEnumCustomAttribute : EnumCustomAttribute
    {


        ///// <summary>
        ///// 当前枚举项
        ///// </summary>
        //public Enum CurrentEnum { get; set; }



        /// <summary>
        /// 枚举项对应的Key值
        /// </summary>
        public string EnumKey { get; set; }


        /// <summary>
        /// 枚举项导航索引值
        /// </summary>
        public object EnumValue { get; set; }

        /// <summary>
        /// 枚举自定义扩展标记
        /// </summary>
        public TEnumCustomAttribute EnumCustomAttribute { get; set; }



        ///// <summary>
        ///// 标题
        ///// </summary>
        //public string Title { get; set; }


        ///// <summary>
        ///// 枚举项描述
        ///// </summary>
        //public string Description { get; set; }


        ///// <summary>
        ///// 枚举项其他附加属性值
        ///// </summary>
        //public object Other { get; set; }


        /// <summary>
        /// 枚举单项实体类 构造方法
        /// </summary>
        /// <param name="enumKey">枚举项对应的Key值</param>
        /// <param name="enumValue">枚举项导航索引值</param>
        /// <param name="enumCustomAttribute">枚举自定义扩展标记</param>
        public EnumItem(string enumKey, object enumValue, TEnumCustomAttribute enumCustomAttribute = null)
        {

            //CurrentEnum = enumItem;
            EnumKey = enumKey;
            EnumValue = enumValue;
            EnumCustomAttribute = enumCustomAttribute;
            //if (!description.IfIsNullOrEmpty())
            //{
            //    _CustomDescriptionAttribute = description;
            //    //Title = description.Title;
            //    //Description = description.Description;
            //    //Other = description.Other;
            //}
        }


        //public Enum CurrentEnum
        //{

        //    get
        //    {
        //        return (T)Enum.ToObject(t, EnumValue);
        //    }
        //}





    }
}
