using System;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments.CustomAttributes;
using Lanymy.Common.Instruments.Models;

namespace Lanymy.Common.ExtensionFunctions
{

    /// <summary>
    /// 枚举 静态 扩展 方法
    /// </summary>
    public static class EnumExtensions
    {


        /// <summary>
        /// 获取Enum指定项
        /// </summary>
        /// <param name="o">The Enum.</param>
        /// <returns>EnumItem.</returns>
        public static EnumItem GetEnumItem(this Enum o)
        {
            return EnumHelper.GetEnumItem(o);
        }

        /// <summary>
        /// 提升 EnumCustomAttribute  类型
        /// </summary>
        /// <typeparam name="TTarget">EnumCustomAttribute 目标类型</typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static TTarget AsType<TTarget>(this BaseEnumAttribute o)
            where TTarget : BaseEnumAttribute
        {
            return o.AsType<BaseEnumAttribute, TTarget>();
        }


        /// <summary>
        /// 根据枚举项 提升 EnumCustomAttribute  类型
        /// </summary>
        /// <typeparam name="TTarget">EnumCustomAttribute 目标类型</typeparam>
        /// <param name="o">当前枚举项</param>
        /// <returns></returns>
        public static TTarget GetEnumAttribute<TTarget>(this Enum o)
            where TTarget : BaseEnumAttribute
        {
            return o.GetEnumItem().EnumCustomAttribute.AsType<BaseEnumAttribute, TTarget>();
        }

    }
}
