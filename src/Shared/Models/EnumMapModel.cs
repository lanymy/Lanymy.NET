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
    public class EnumMapModel
    {

        private Dictionary<Enum, EnumItem> _DicEnumMap = new Dictionary<Enum, EnumItem>();

        /// <summary>
        /// 枚举自定义扩展标记缓存字典
        /// </summary>
        public
#if NET40
            IDictionary<Enum, EnumItem>
#else
            IReadOnlyDictionary<Enum, EnumItem> 
#endif
            DicEnumMap
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
        public EnumMapModel(Type enumType)
        {

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("传入的参数必须是枚举类型！", nameof(enumType));
            }


            foreach (Enum enumValue in Enum.GetValues(enumType))
            {
                FieldInfo field = enumType.GetField(enumValue.ToString());
                var attribute = Attribute.GetCustomAttribute(field, typeof(BaseEnumCustomAttribute)) as BaseEnumCustomAttribute;
                _DicEnumMap.Add(enumValue, new EnumItem(enumValue, attribute));
            }

        }



    }
}
