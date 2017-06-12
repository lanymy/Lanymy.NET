// *******************************************************************
// 创建时间：2015年01月14日, AM 11:10:57
// 作者：lanyanmiyu@qq.com
// 说明：枚举扩展类
// 其它:
// *******************************************************************



using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Lanymy.General.Extension.CustomAttributes;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Interfaces;
using Lanymy.General.Extension.Models;



namespace Lanymy.General.Extension
{
    /// <summary>
    /// 枚举扩展类
    /// </summary>
    public class EnumFunctions
    {


   /// <summary>
   /// 枚举内存缓存器
   /// </summary>
        private static IDataMemoryCache _DataMemoryCache = new DataMemoryCache();


        /// <summary>
        /// 获取 枚举类型 缓存 主键值
        /// </summary>
        /// <typeparam name="TEnumCustomAttribute">枚举中的特性标记类型</typeparam>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        private static string GetEnumMapCacheKey<TEnumCustomAttribute>(Type enumType)
        {
            Type tEnumCustomAttribute = typeof(TEnumCustomAttribute);
            return string.Format("{0}_{1}", enumType.FullName, tEnumCustomAttribute.FullName);
        }

        /// <summary>
        /// 获取 枚举 子项 缓存主键值
        /// </summary>
        /// <typeparam name="TEnumCustomAttribute">枚举中的特性标记类型</typeparam>
        /// <param name="enumItem">枚举子项</param>
        /// <returns></returns>
        private static string GetEnumItemCacheKey<TEnumCustomAttribute>(Enum enumItem)
        {
            return string.Format("{0}_{1}", GetEnumMapCacheKey<TEnumCustomAttribute>(enumItem.GetType()), enumItem);
        }

        /// <summary>
        /// 获取当前枚举映射导航器
        /// </summary>
        /// <typeparam name="TEnumCustomAttribute"></typeparam>
        /// <param name="enumType"></param>
        /// <returns></returns>
        private static EnumMapModel<TEnumCustomAttribute> GetMapper<TEnumCustomAttribute>(Type enumType)
            where TEnumCustomAttribute : EnumCustomAttribute
        {
            string cacheKey = GetEnumMapCacheKey<TEnumCustomAttribute>(enumType);
            EnumMapModel<TEnumCustomAttribute> mapper = _DataMemoryCache.GetValue<EnumMapModel<TEnumCustomAttribute>>(cacheKey);
            if (mapper.IfIsNullOrEmpty())
            {
                mapper = new EnumMapModel<TEnumCustomAttribute>(enumType);
                _DataMemoryCache.SetValue(cacheKey, mapper);
            }
            return mapper;
        }

        /// <summary>
        /// 获取Enum指定项
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static EnumItem<TEnumCustomAttribute> GetEnumItem<TEnumCustomAttribute>(Enum enumItem)
            //where TEnum : IComparable, IFormattable, IConvertible
            where TEnumCustomAttribute : EnumCustomAttribute
        {
            return GetMapper<TEnumCustomAttribute>(enumItem.GetType())[enumItem];
        }


        /// <summary>
        /// 获取Enum的子项 主键是 枚举子项 的字典类型集合
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static
#if NET40
            IDictionary<Enum, EnumItem<TEnumCustomAttribute>>

#else
            IReadOnlyDictionary<Enum, EnumItem<TEnumCustomAttribute>> 
#endif
            GetEnumItemDictionary<TEnum, TEnumCustomAttribute>()
            where TEnum : IComparable, IFormattable, IConvertible
            where TEnumCustomAttribute : EnumCustomAttribute
        {
            return GetMapper<TEnumCustomAttribute>(typeof(TEnum)).DicEnumMap;
        }


        /// <summary>
        /// 获取Enum的子项集合
        /// </summary>
        /// <returns></returns>
        public static List<EnumItem<TEnumCustomAttribute>> GetEnumItemList<TEnum, TEnumCustomAttribute>()
            where TEnum : IComparable, IFormattable, IConvertible
            where TEnumCustomAttribute : EnumCustomAttribute
        {
            return GetMapper<TEnumCustomAttribute>(typeof(TEnum)).DicEnumMap.Select(o => o.Value).ToList();
        }




        /// <summary>
        /// 获取枚举多选项列表 主键是 枚举子项 的字典类型集合
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Enum, EnumItem<TEnumCustomAttribute>> GetEnumFlagsItemDictionary<TEnumCustomAttribute>(Enum item)
            where TEnumCustomAttribute : EnumCustomAttribute
        {
            List<string> flags = item.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).ToList();
            if (flags.IfIsNullOrEmpty())
            {
                return new Dictionary<Enum, EnumItem<TEnumCustomAttribute>>();
            }
            var mapDic = GetMapper<TEnumCustomAttribute>(item.GetType()).DicEnumMap;
            return mapDic.Where(o => flags.Contains(o.Value.EnumKey)).ToDictionary(dicItem => dicItem.Key, dicItem => dicItem.Value);
        }


        /// <summary>
        /// 获取枚举多选项列表
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static List<EnumItem<TEnumCustomAttribute>> GetEnumFlagsItemList<TEnumCustomAttribute>(Enum item)
            where TEnumCustomAttribute : EnumCustomAttribute
        {
            return GetEnumFlagsItemDictionary<TEnumCustomAttribute>(item).Select(o => o.Value).ToList();
        }


        /// <summary>
        /// 获取默认EnumCustomAttribute特性标记的枚举列表
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <returns></returns>
        public static List<EnumItem<EnumCustomAttribute>> GetDefaultAttributeEnumItemList<TEnum>()
            where TEnum : IComparable, IFormattable, IConvertible
        {
            return GetEnumItemList<TEnum, EnumCustomAttribute>();
        }

        /// <summary>
        /// 获取默认EnumCustomAttribute特性标记的枚举列表
        /// </summary>
        /// <param name="enumItem">具体 枚举 子项</param>
        /// <returns></returns>
        public static EnumItem<EnumCustomAttribute> GetDefaultAttributeEnumItem(Enum enumItem)
        {
            return GetEnumItem<EnumCustomAttribute>(enumItem);
        }


    }
}
