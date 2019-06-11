using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common
{
    /// <summary>
    /// 反射辅助类
    /// </summary>
    public class ReflectionHelper
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
        /// 通过 反射 获取 实体类 类上面标记的特定特性集合
        /// </summary>
        /// <param name="modelType">实体类类型</param>
        /// <typeparam name="TAttribute">特性</typeparam>
        /// <returns></returns>
        public static List<TAttribute> GetClassAttributeListFromModel<TAttribute>(Type modelType)
            where TAttribute : Attribute
        {

            Type currentModelType = modelType;
            Type currentAttributeType = typeof(TAttribute);


            List<TAttribute> list = new List<TAttribute>();

            if (currentModelType.IsClass)
            {
                foreach (var customAttribute in currentModelType.GetCustomAttributes(currentAttributeType, true))
                {
                    list.Add(customAttribute as TAttribute);
                }
            }

            return list;

        }


        /// <summary>
        /// 通过 反射 获取 实体类 类上面标记的特定特性集合
        /// </summary>
        /// <typeparam name="TModel">实体类</typeparam>
        /// <typeparam name="TAttribute">特性</typeparam>
        /// <returns></returns>
        public static List<TAttribute> GetClassAttributeListFromModel<TModel, TAttribute>()
            where TModel : class
            where TAttribute : Attribute
        {

            Type currentModelType = typeof(TModel);
            return GetClassAttributeListFromModel<TAttribute>(currentModelType);

            //Type currentAttributeType = typeof(TAttribute);


            //List<TAttribute> list = new List<TAttribute>();

            //foreach (var customAttribute in currentModelType.GetCustomAttributes(currentAttributeType, true))
            //{
            //    list.Add(customAttribute as TAttribute);
            //}

            //return list;

        }


        /// <summary>
        /// 通过反射获取实体类属性上面标记的特定特性集合
        /// </summary>
        /// <typeparam name="TModel">实体类</typeparam>
        /// <typeparam name="TAttribute">特性</typeparam>
        /// <param name="doWorkForEachAttributeModel">对每个特性需要操作的回调方法,提供两个参数可以进行操作 第一个参数 当前特性 第二个参数 当前属性的反射信息实体类</param>
        /// <returns></returns>
        public static List<TAttribute> GetPropertyAttributeListFromModel<TModel, TAttribute>(Action<PropertyInfo, TAttribute> doWorkForEachAttributeModel = null)
            where TModel : class
            where TAttribute : Attribute
        {

            Type currentModelType = typeof(TModel);

            List<TAttribute> list = new List<TAttribute>();

            foreach (var propertyInfo in currentModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (var currentAttribute in propertyInfo.GetCustomAttributes<TAttribute>(true))
                {
                    if (!doWorkForEachAttributeModel.IfIsNullOrEmpty())
                    {
                        doWorkForEachAttributeModel(propertyInfo, currentAttribute);
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
        public static List<TAttribute> GetAttributeListFromEnum<TEnum, TAttribute>(Action<Enum, FieldInfo, TAttribute> doWorkForEachAttributeModel = null)
            where TEnum : IComparable, IFormattable, IConvertible
            where TAttribute : Attribute
        {

            Type enumType = typeof(TEnum);

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("传入的参数必须是枚举类型！", "TEnum");
            }


            List<TAttribute> list = new List<TAttribute>();

            foreach (Enum enumValue in Enum.GetValues(enumType))
            {

                var fieldInfo = enumType.GetField(enumValue.ToString());
                var attribute = Attribute.GetCustomAttribute(fieldInfo, typeof(TAttribute)) as TAttribute;

                if (!doWorkForEachAttributeModel.IfIsNullOrEmpty())
                {
                    doWorkForEachAttributeModel(enumValue, fieldInfo, attribute);
                }

                list.Add(attribute);

                //foreach (var currentAttribute in fieldInfo.GetCustomAttributes<TAttribute>(true))
                //{

                //    if (!doWorkForEachAttributeModel.IfIsNullOrEmpty())
                //    {
                //        doWorkForEachAttributeModel(currentAttribute, fieldInfo);
                //    }

                //    list.Add(currentAttribute);

                //}

            }

            return list;

        }



        /// <summary>
        /// Gets the dictionary parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        public static Dictionary<string, object> GetDictionaryParameters<T>(T t)
        //where T : class
        {

            //var aaa = SerializeHelper.SerializeToJson(t);
            var dicParameters = new Dictionary<string, object>();
            //Type type = t.GetType();
            foreach (PropertyInfo p in t.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                MethodInfo mi = p.GetGetMethod();
                if (mi != null && mi.IsPublic)
                {
                    //dicParameters.Add(p.Name, mi.Invoke(o, new object[] { }));
                    dicParameters.Add(p.Name, p.GetValue(t));
                }
            }
            return dicParameters;
        }



        /// <summary>
        /// 从程序集中 找出 所有继承基类 的子类 集合
        /// </summary>
        /// <param name="findAssembly">要查找的程序集</param>
        /// <param name="baseType">基类类型</param>
        /// <returns>List&lt;Type&gt;.</returns>
        public static List<Type> GetChildTypesByBaseType(Assembly findAssembly, Type baseType)
        {
            return findAssembly.GetTypes().Where(typeItem => typeItem.BaseType == baseType).ToList();
        }



    }
}
