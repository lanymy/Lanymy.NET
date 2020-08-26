using System;
using Lanymy.Common.Instruments.CustomAttributes;

namespace Lanymy.Common.Instruments.Models
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
        public BaseEnumAttribute EnumCustomAttribute { get; }



        /// <summary>
        /// 枚举单项实体类 构造方法
        /// </summary>
        /// <param name="currentEnum">当前 导航的 枚举项</param>
        /// <param name="enumCustomAttribute">当前 枚举 自定义 扩展 特性</param>
        public EnumItem(Enum currentEnum, BaseEnumAttribute enumCustomAttribute)
        {
            CurrentEnum = currentEnum;
            EnumCustomAttribute = enumCustomAttribute;
        }

    }
}
