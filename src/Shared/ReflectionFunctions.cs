/********************************************************************

时间: 2015年10月26日, AM 09:26:21

作者: lanyanmiyu@qq.com

描述: 反射辅助类

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.ExtensionFunctions;
using System.Reflection;

namespace Lanymy.General.Extension
{

    /// <summary>
    /// 反射辅助类
    /// </summary>
    public class ReflectionFunctions
    {

        /// <summary>
        /// 获取调用属性的 属性名称
        /// </summary>
        /// <typeparam name="T">属性所属的类</typeparam>
        /// <param name="expr">属性调用表达式</param>
        /// <returns></returns>
        public static string GetPropertyName<T>(Expression<Func<T, object>> expr)
        {

            string result = string.Empty;

            if (expr.Body is UnaryExpression)
            {
                result = ((MemberExpression)((UnaryExpression)expr.Body).Operand).Member.Name;
            }
            else if (expr.Body is MemberExpression)
            {
                result = ((MemberExpression)expr.Body).Member.Name;
            }
            else if (expr.Body is ParameterExpression)
            {
                result = ((ParameterExpression)expr.Body).Type.Name;
            }

            return result;

        }


        /// <summary>
        /// 通过反射获取实体类属性上面标记的特定特性集合
        /// </summary>
        /// <typeparam name="TModel">实体类</typeparam>
        /// <typeparam name="TAttribute">特性</typeparam>
        /// <param name="doWorkForEachAttributeModel">对每个特性需要操作的回调方法,提供两个参数可以进行操作 第一个参数 当前特性 第二个参数 当前属性的反射信息实体类</param>
        /// <returns></returns>
        public static List<TAttribute> GetAttributeListFromModel<TModel, TAttribute>(Action<TAttribute, PropertyInfo> doWorkForEachAttributeModel = null)
            where TModel : class
            where TAttribute : Attribute
        {

            Type currentModelType = typeof(TModel);

#if NET40
            Type currentAttributeType = typeof(TAttribute);
#endif

            List<TAttribute> list = new List<TAttribute>();


            foreach (var propertyInfo in currentModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {

#if NET40
                foreach (TAttribute currentAttribute in propertyInfo.GetCustomAttributes(currentAttributeType, true))
#else
                foreach (var currentAttribute in propertyInfo.GetCustomAttributes<TAttribute>(true))
#endif
                {
                    if (!doWorkForEachAttributeModel.IfIsNullOrEmpty())
                    {
                        doWorkForEachAttributeModel(currentAttribute, propertyInfo);
                    }

                    list.Add(currentAttribute);
                }
            }

            return list;

        }


        /// <summary>
        /// 获取枚举的所有特性标记
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <typeparam name="TAttribute">特性标记</typeparam>
        /// <param name="doWorkForEachAttributeModel">每个特性的回调方法</param>
        /// <returns></returns>
        public static List<TAttribute> GetAttributeListFromEnum<TEnum, TAttribute>(Action<TAttribute, FieldInfo> doWorkForEachAttributeModel = null)
            where TEnum : IComparable, IFormattable, IConvertible
            where TAttribute : Attribute
        {

            Type enumType = typeof(TEnum);

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("传入的参数必须是枚举类型！", "TEnum");
            }

#if NET40
            Type currentAttributeType = typeof(TAttribute);
#endif

            List<TAttribute> list = new List<TAttribute>();

            foreach (Enum enumValue in Enum.GetValues(enumType))
            {

                var fieldInfo = enumType.GetField(enumValue.ToString());
#if NET40
                foreach (TAttribute currentAttribute in fieldInfo.GetCustomAttributes(currentAttributeType, true))
#else
                foreach (var currentAttribute in fieldInfo.GetCustomAttributes<TAttribute>(true))
#endif
                {
                    if (!doWorkForEachAttributeModel.IfIsNullOrEmpty())
                    {
                        doWorkForEachAttributeModel(currentAttribute, fieldInfo);
                    }

                    list.Add(currentAttribute);
                }
            }

            return list;

        }


    }


}
