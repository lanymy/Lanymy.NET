/********************************************************************

时间: 2016年12月19日, PM 03:46:59

作者: lanyanmiyu@qq.com

描述: 枚举自定义扩展标记 映射 实体类

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.CustomAttributes;

namespace Lanymy.General.Extension.Models
{

    /// <summary>
    /// 枚举自定义扩展标记 映射 实体类
    /// </summary>
    /// <typeparam name="TEnumCustomAttribute">枚举自定义扩展标记类型</typeparam>
    public class EnumMapModel<TEnumCustomAttribute> where TEnumCustomAttribute : EnumCustomAttribute
        //where TEnum : IComparable, IFormattable, IConvertible
    {

        private Dictionary<Enum, EnumItem<TEnumCustomAttribute>> _DicEnumMap = new Dictionary<Enum, EnumItem<TEnumCustomAttribute>>();

        /// <summary>
        /// 枚举自定义扩展标记缓存字典
        /// </summary>
        public
#if NET40
            IDictionary<Enum, EnumItem<TEnumCustomAttribute>>

#else
            IReadOnlyDictionary<Enum, EnumItem<TEnumCustomAttribute>> 
#endif
            DicEnumMap
        {
            get
            {
                return _DicEnumMap;
            }
        }

        public EnumItem<TEnumCustomAttribute> this[Enum item]
        {
            get
            {
                return _DicEnumMap[item];
            }
        }

        public EnumMapModel(Type enumType)
        {
            //Type tEnum = typeof(TEnum);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("传入的参数必须是枚举类型！", "TEnum");
            }

            var underlyingType = Enum.GetUnderlyingType(enumType);
            foreach (Enum enumValue in Enum.GetValues(enumType))
            {
                FieldInfo field = enumType.GetField(enumValue.ToString());
                TEnumCustomAttribute attribute = Attribute.GetCustomAttribute(field, typeof(TEnumCustomAttribute)) as TEnumCustomAttribute;
                _DicEnumMap.Add(enumValue, new EnumItem<TEnumCustomAttribute>(enumValue.ToString(), Convert.ChangeType(enumValue, underlyingType, null), attribute));
            }
        }


        //private Dictionary<Enum, EnumItem> _DicEnumMap;

        //public Dictionary<Enum, EnumItem> DicEnumMap
        //{
        //    get
        //    {
        //        return _DicEnumMap;
        //    }
        //}

        //public EnumMap(Type enumType)
        //{

        //    if (!enumType.IsEnum)
        //    {
        //        throw new ArgumentException("传入的参数必须是枚举类型！", "enumType");
        //    }

        //    _DicEnumMap = new Dictionary<Enum, EnumItem>();
        //    var underlyingType = Enum.GetUnderlyingType(enumType);

        //    foreach (Enum enumValue in Enum.GetValues(enumType))
        //    {
        //        FieldInfo field = enumType.GetField(enumValue.ToString());
        //        EnumCustomAttribute attribute = Attribute.GetCustomAttribute(field, typeof(EnumCustomAttribute)) as EnumCustomAttribute;
        //        _DicEnumMap.Add(enumValue, new EnumItem(enumValue.ToString(), Convert.ChangeType(enumValue, underlyingType, null), attribute));
        //    }

        //}

        //public EnumItem this[Enum item]
        //{
        //    get
        //    {
        //        return _DicEnumMap[item];
        //    }
        //}


    }
}
