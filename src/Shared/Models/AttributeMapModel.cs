/********************************************************************

时间: 2016年05月19日, PM 01:53:04

作者: lanyanmiyu@qq.com

描述: 实体类 特性 映射 操作  类

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.ExtensionFunctions;

namespace Lanymy.General.Extension.Models
{


    /// <summary>
    /// 实体类 特性 映射 操作  类
    /// </summary>
    public class AttributeMapModel<TModel, TAttribute> : IDisposable
        where TModel : class
        where TAttribute : Attribute
    {

        /// <summary>
        /// 特性列表
        /// </summary>
        public List<TAttribute> AttributeList { get; private set; }

        /// <summary>
        /// 反射属性信息列表
        /// </summary>
        public List<PropertyInfo> PropertyInfoList { get; private set; }


        /// <summary>
        /// 表达式和索引映射关系字典
        /// </summary>
        private Dictionary<Expression<Func<TModel, object>>, int> _DictionaryAttributeProperty;


        public void Dispose()
        {
            if (!AttributeList.IfIsNullOrEmpty())
            {
                AttributeList.Clear();
                AttributeList = null;
            }

            if (!PropertyInfoList.IfIsNullOrEmpty())
            {
                PropertyInfoList.Clear();
                PropertyInfoList = null;
            }

            if (!_DictionaryAttributeProperty.IfIsNullOrEmpty())
            {
                _DictionaryAttributeProperty.Clear();
                _DictionaryAttributeProperty = null;
            }
        }

        public AttributeMapModel()
        {
            PropertyInfoList = new List<PropertyInfo>();
            _DictionaryAttributeProperty = new Dictionary<Expression<Func<TModel, object>>, int>();
            AttributeList = ReflectionFunctions.GetAttributeListFromModel<TModel, TAttribute>
            (
                (attribute, propertyInfo) =>
                {
                    PropertyInfoList.Add(propertyInfo);
                }
            );
        }


        /// <summary>
        /// 根据属性表达式 获取 在列表中的索引值
        /// </summary>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public int GetItemIndex(Expression<Func<TModel, object>> propertyExpression)
        {
            if (!_DictionaryAttributeProperty.ContainsKey(propertyExpression))
            {
                var currentPropertyInfo = PropertyInfoList.Where(o => o.Name == ReflectionFunctions.GetPropertyName(propertyExpression)).FirstOrDefault();
                var attributeIndex = PropertyInfoList.IndexOf(currentPropertyInfo);
                _DictionaryAttributeProperty[propertyExpression] = attributeIndex;
            }

            return _DictionaryAttributeProperty[propertyExpression];
        }

        /// <summary>
        /// 根据属性表达式 获取 当前属性的 特性
        /// </summary>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public TAttribute GetAttributeByPropertyExpression(Expression<Func<TModel, object>> propertyExpression)
        {
            return AttributeList[GetItemIndex(propertyExpression)];
        }


        /// <summary>
        /// 根据属性表达式 获取 当前属性的 反射信息
        /// </summary>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public PropertyInfo GetPropertyInfoByPropertyExpression(Expression<Func<TModel, object>> propertyExpression)
        {
            return PropertyInfoList[GetItemIndex(propertyExpression)];
        }


        /// <summary>
        /// 根据特性获取对应的属性 反射 信息 
        /// </summary>
        /// <param name="tAttribute"></param>
        /// <returns></returns>
        public PropertyInfo GetPropertyInfoByAttribute(TAttribute tAttribute)
        {
            return PropertyInfoList[AttributeList.IndexOf(tAttribute)];
        }


        /// <summary>
        /// 根据特性 获取 对应 的属性 值
        /// </summary>
        /// <param name="tAttribute"></param>
        /// <param name="tModel"></param>
        /// <returns></returns>
        public object GetPropertyValueByAttribute(TAttribute tAttribute, TModel tModel)
        {
            return GetPropertyInfoByAttribute(tAttribute).GetValue(tModel, null);
        }


    }


}
