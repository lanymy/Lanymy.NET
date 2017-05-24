// *******************************************************************
// 创建时间：2015年01月14日, AM 10:10:38
// 作者：lanyanmiyu@qq.com
// 说明：Object类型静态扩展类库
// 其它:
// *******************************************************************




using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lanymy.General.Extension.ExtensionFunctions
{


    /// <summary>
    /// Object类型静态扩展类库
    /// </summary>
    public static class ObjectExtension
    {


        #region 判断是否为空

        /// <summary>
        /// 判断对象是否为null或者为空
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IfIsNullOrEmpty(this object o)
        {
            return null == o || o.Equals("");
        }

        /// <summary>
        /// 判断对象是否为null或者为空
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IfIsNullOrEmpty(this string o)
        {
            return string.IsNullOrEmpty(o) || string.IsNullOrEmpty(o.Trim());
        }


        /// <summary>
        /// 判断对象是否为null或者为空
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IfIsNullOrEmpty(this StringBuilder o)
        {
            return o == null || o.Length == 0 || o.ToString().IfIsNullOrEmpty();
        }


        /// <summary>
        /// 判断对象是否为null或者为空
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IfIsNullOrEmpty(this Guid o)
        {
            return o == Guid.Empty;
        }


        /// <summary>
        /// 判断对象是否为null或者为空
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IfIsNullOrEmpty<T>(this ICollection<T> o)
        {
            return null == o || o.Count == 0;
        }

        /// <summary>
        /// 判断对象是否为null或者为空
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IfIsNullOrEmpty<T>(this IEnumerable<T> o)
        {
            return null == o || o.Count() == 0;
        }

        /// <summary>
        /// 判断对象是否为null或者为空
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IfIsNullOrEmpty<TK, TV>(this IDictionary<TK, TV> o)
        {
            return null == o || o.Count == 0;
        }

        #endregion



        #region 带默认值的类型转换


        /// <summary>
        /// 类型转换带默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="defaultValue">如果转换失败返回的默认值</param>
        /// <returns></returns>
        public static T ConvertToType<T>(this object o, T defaultValue = default(T))
        {
            try
            {
                return (T)System.Convert.ChangeType(o, typeof(T), null);
            }
            catch
            {
                return defaultValue;
            }
        }


        /// <summary>
        /// 类型转换带默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="defaultValue">如果转换失败返回的默认值</param>
        /// <returns></returns>
        public static T ConvertToType<T>(this string o, T defaultValue = default(T))
        {
            try
            {
                return (T)System.Convert.ChangeType(o, typeof(T), null);
            }
            catch
            {
                return defaultValue;
            }
        }


        #region 枚举类型转换


        /// <summary>
        /// 字符串转换成Enum
        /// </summary>
        /// <typeparam name="T">Enum类型</typeparam>
        /// <param name="o">字符串</param>
        /// <param name="ignoreCase">True 忽略大小写 ; False 匹配大小写</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T ConvertToEnum<T>(this string o, bool ignoreCase = true, T defaultValue = default(T))
        {
            Type t = typeof(T);

            if (!t.IsEnum)
            {
                return defaultValue;
            }

            try
            {
                return (T)Enum.Parse(t, o, ignoreCase);
            }
            catch
            {
                return defaultValue;
            }
        }


        /// <summary>
        /// 转换成Enum
        /// </summary>
        /// <typeparam name="T">Enum类型</typeparam>
        /// <param name="o">字符串</param>
        /// <param name="ignoreCase">True 忽略大小写 ; False 匹配大小写</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T ConvertToEnum<T>(this byte o, bool ignoreCase = true, T defaultValue = default(T))
        {
            return o.ToString().ConvertToEnum(ignoreCase, defaultValue);
        }

        /// <summary>
        /// 转换成Enum
        /// </summary>
        /// <typeparam name="T">Enum类型</typeparam>
        /// <param name="o">字符串</param>
        /// <param name="ignoreCase">True 忽略大小写 ; False 匹配大小写</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T ConvertToEnum<T>(this sbyte o, bool ignoreCase = true, T defaultValue = default(T))
        {
            return o.ToString().ConvertToEnum(ignoreCase, defaultValue);
        }


        /// <summary>
        /// 转换成Enum
        /// </summary>
        /// <typeparam name="T">Enum类型</typeparam>
        /// <param name="o">字符串</param>
        /// <param name="ignoreCase">True 忽略大小写 ; False 匹配大小写</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T ConvertToEnum<T>(this short o, bool ignoreCase = true, T defaultValue = default(T))
        {
            return o.ToString().ConvertToEnum(ignoreCase, defaultValue);
        }


        /// <summary>
        /// 转换成Enum
        /// </summary>
        /// <typeparam name="T">Enum类型</typeparam>
        /// <param name="o">字符串</param>
        /// <param name="ignoreCase">True 忽略大小写 ; False 匹配大小写</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T ConvertToEnum<T>(this ushort o, bool ignoreCase = true, T defaultValue = default(T))
        {
            return o.ToString().ConvertToEnum(ignoreCase, defaultValue);
        }


        /// <summary>
        /// 转换成Enum
        /// </summary>
        /// <typeparam name="T">Enum类型</typeparam>
        /// <param name="o">字符串</param>
        /// <param name="ignoreCase">True 忽略大小写 ; False 匹配大小写</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T ConvertToEnum<T>(this int o, bool ignoreCase = true, T defaultValue = default(T))
        {
            return o.ToString().ConvertToEnum(ignoreCase, defaultValue);
        }

        /// <summary>
        /// 转换成Enum
        /// </summary>
        /// <typeparam name="T">Enum类型</typeparam>
        /// <param name="o">字符串</param>
        /// <param name="ignoreCase">True 忽略大小写 ; False 匹配大小写</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T ConvertToEnum<T>(this uint o, bool ignoreCase = true, T defaultValue = default(T))
        {
            return o.ToString().ConvertToEnum(ignoreCase, defaultValue);
        }

        /// <summary>
        /// 转换成Enum
        /// </summary>
        /// <typeparam name="T">Enum类型</typeparam>
        /// <param name="o">字符串</param>
        /// <param name="ignoreCase">True 忽略大小写 ; False 匹配大小写</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T ConvertToEnum<T>(this long o, bool ignoreCase = true, T defaultValue = default(T))
        {
            return o.ToString().ConvertToEnum(ignoreCase, defaultValue);
        }


        /// <summary>
        /// 转换成Enum
        /// </summary>
        /// <typeparam name="T">Enum类型</typeparam>
        /// <param name="o">字符串</param>
        /// <param name="ignoreCase">True 忽略大小写 ; False 匹配大小写</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T ConvertToEnum<T>(this ulong o, bool ignoreCase = true, T defaultValue = default(T))
        {
            return o.ToString().ConvertToEnum(ignoreCase, defaultValue);
        }


        #endregion




        /// <summary>
        /// 字符串转日期对象
        /// </summary>
        /// <param name="o">要转换的字符串</param>
        /// <param name="format">字符串日期表达式的格式 默认值 yyyyMMdd</param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(this string o, string format = "yyyyMMdd")
        {
            try
            {
                return DateTime.ParseExact(o, format, null);
            }
            catch
            {
                return default(DateTime);
            }
        }

        /// <summary>
        /// 把 object 通过字符串的形式 转换成 Booolean
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool ConvertToBoolean(this object o, bool defaultValue = default(bool))
        {
            return o.ToString().ConvertToBoolean(defaultValue);
        }

        /// <summary>
        /// 字符串转Boolean ( 支持true;false;True;False;0 (0表示False);1 (1表示True); )
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defaultValue">如果转换失败 返回的默认值</param>
        /// <returns></returns>
        public static bool ConvertToBoolean(this string o, bool defaultValue = default(bool))
        {
            o = o.ToLower();

            if (o == "true" || o == "1")
            {
                defaultValue = true;
            }
            else if (o == "false" || o == "0")
            {
                defaultValue = false;
            }

            return defaultValue;
        }



        #endregion



        /// <summary>
        /// 对象深度克隆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T t) where T : class
        {
            return SerializeFunctions.DeserializeFromJson<T>(SerializeFunctions.SerializeToJson(t));
        }

    }
}
