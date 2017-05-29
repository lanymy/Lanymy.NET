/********************************************************************

时间: 2015年10月23日, AM 10:32:01

作者: lanyanmiyu@qq.com

描述: CSV 序列化 反序列化  字段属性 描述特性

其它:     

********************************************************************/


using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Lanymy.General.Extension.CustomAttributes
{
    /// <summary>
    /// CSV 序列化 反序列化  字段属性 描述特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CSVDescriptionAttribute : Attribute
    {


        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 字段标题
        /// </summary>
        public string Title { get; set; }


        ///// <summary>
        ///// 序列化成csv属性值 字符串 格式化 格式 如果为空 则 使用默认格式ToString() 转换
        ///// </summary>
        //public string SerializeToCsvFormatString { get; set; }


        ///// <summary>
        ///// 属性值序列化成csv字符串的转换函数方法
        ///// </summary>
        //public Func<object, string> SerializeoCsvStringFunc { get; set; }


        /// <summary>
        /// 属性映射对象引用
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }


        ///// <summary>
        ///// 属性序列化方法
        ///// </summary>
        //public Func<object, string> PropertySerializeFunc { get; set; }


        ///// <summary>
        ///// 属性反序列化方法
        ///// </summary>
        //public Func<string, object> PropertyDeserializeFunc { get; set; }


        /// <summary>
        /// CSV 序列化 反序列化  字段属性 描述特性 构造方法
        /// </summary>
        /// <param name="index">CSV索引值</param>
        /// <param name="title">CSV标题</param>
        ///// <param name="propertySerializeFunc">属性序列化方法</param>
        ///// <param name="propertyDeserializeFunc">属性反序列化方法</param>
        //public CSVDescriptionAttribute(int index, string title = null, Func<object, string> propertySerializeFunc = null, Func<string, object> propertyDeserializeFunc = null)
        public CSVDescriptionAttribute(int index, string title = null)
        {
            Index = index;
            Title = title;
            //PropertySerializeFunc = propertySerializeFunc;
            //PropertyDeserializeFunc = propertyDeserializeFunc;
        }



    }
}
