/********************************************************************

时间: 2017年05月28日, PM 06:41:30

作者: lanyanmiyu@qq.com

描述: CSV自定义序列化 映射信息 实体类

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.General.Extension.Models
{


    /// <summary>
    /// CSV自定义序列化 映射信息 实体类
    /// </summary>
    public class CSVSerializeMapModel<TModel> where TModel:class 
    {

        /// <summary>
        /// 序列化CSV实体类的 类型
        /// </summary>
        public Type CsvModelType { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 属性序列化方法
        /// </summary>
        public Func<TModel, string> PropertySerializeFunc { get; set; }


        /// <summary>
        /// 属性反序列化方法
        /// </summary>
        public Func<string, object> PropertyDeserializeFunc { get; set; }

    }



}
