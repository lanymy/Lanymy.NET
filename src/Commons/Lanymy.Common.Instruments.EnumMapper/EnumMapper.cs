using System;
using System.Collections.Generic;
using System.Reflection;
using Lanymy.Common.Instruments.CustomAttributes;
using Lanymy.Common.Instruments.Models;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 枚举自定义扩展标记 映射 实体类
    /// </summary>
    public class EnumMapper
    {

        private Dictionary<Enum, EnumItem> _DicEnumMap = new Dictionary<Enum, EnumItem>();

        /// <summary>
        /// 枚举自定义扩展标记缓存字典
        /// </summary>
        public IReadOnlyDictionary<Enum, EnumItem> DicEnumMap
        {
            get
            {
                return _DicEnumMap;
            }
        }

        /// <summary>
        /// 枚举项
        /// </summary>
        /// <param name="item"></param>
        public EnumItem this[Enum item]
        {
            get
            {
                return _DicEnumMap[item];
            }
        }


        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        public EnumMapper(Type enumType)
        {

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("传入的参数必须是枚举类型！", nameof(enumType));
            }


            foreach (Enum enumValue in Enum.GetValues(enumType))
            {
                FieldInfo field = enumType.GetField(enumValue.ToString());
                var attribute = Attribute.GetCustomAttribute(field, typeof(BaseEnumAttribute)) as BaseEnumAttribute;
                _DicEnumMap.Add(enumValue, new EnumItem(enumValue, attribute));
            }

        }



    }
}

