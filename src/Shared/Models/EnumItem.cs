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
    public class EnumItem
    {


        /// <summary>
        /// 当前枚举项
        /// </summary>
        public Enum CurrentEnum { get; }



        /// <summary>
        /// 枚举自定义扩展特性 基类 
        /// </summary>
        public BaseEnumCustomAttribute EnumCustomAttribute { get; }



        /// <summary>
        /// 枚举单项实体类 构造方法
        /// </summary>
        /// <param name="currentEnum">当前 导航的 枚举项</param>
        /// <param name="enumCustomAttribute">当前 枚举 自定义 扩展 特性</param>
        public EnumItem(Enum currentEnum, BaseEnumCustomAttribute enumCustomAttribute)
        {
            CurrentEnum = currentEnum;
            EnumCustomAttribute = enumCustomAttribute;
        }

    }



}
