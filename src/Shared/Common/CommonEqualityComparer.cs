/********************************************************************

时间: 2015年11月03日, AM 11:05:53

作者: lanyanmiyu@qq.com

描述: 通用比较器

其它:     

********************************************************************/



using System;
using System.Collections.Generic;


namespace Lanymy.General.Extension.Common
{


    /// <summary>
    /// 通用比较器
    /// </summary>
    /// <typeparam name="T">要比较的实体类型</typeparam>
    /// <typeparam name="TValue">要比较实体中的具体属性类型</typeparam>
    public class CommonEqualityComparer<T, TValue> : IEqualityComparer<T>
    {

        private Func<T, TValue> _KeySelector;

        private IEqualityComparer<TValue> _Comparer;


        /// <summary>
        /// 通用比较器 构造方法
        /// </summary>
        /// <param name="keySelector">比较属性的选择器</param>
        /// <param name="comparer">自定义的属性比较器</param>
        public CommonEqualityComparer(Func<T, TValue> keySelector, IEqualityComparer<TValue> comparer)
        {
            _KeySelector = keySelector;
            _Comparer = comparer;
        }

        /// <summary>
        /// 通用比较器 构造方法 使用默认的属性比较器
        /// </summary>
        /// <param name="keySelector">比较属性的选择器</param>
        public CommonEqualityComparer(Func<T, TValue> keySelector) : this(keySelector, EqualityComparer<TValue>.Default)
        {

        }


        public bool Equals(T x, T y)
        {
            return _Comparer.Equals(_KeySelector(x), _KeySelector(y));
        }


        public int GetHashCode(T obj)
        {
            return _Comparer.GetHashCode(_KeySelector(obj));
        }


    }


}
