using Lanymy.Common.Helpers;

namespace Lanymy.Common.ExtensionFunctions
{



    public static class DeepCloneExtensions
    {

        /// <summary>
        /// 深度克隆模式 把 数据源类的基类数据 传递给 同一基类的目标类
        /// </summary>
        /// <typeparam name="TBase">The type of the t base.</typeparam>
        /// <typeparam name="TTarget">The type of the t target.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>TTarget.</returns>
        public static TTarget AsTypeByDeepClone<TBase, TTarget>(this TBase source)
            where TBase : class
            //where TSource : class, TBase
            where TTarget : class, TBase
        {
            return JsonSerializeHelper.DeserializeFromJson<TTarget>(JsonSerializeHelper.SerializeToJson(source));
        }

        /// <summary>
        /// 对象深度克隆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T t) where T : class
        {

            return t.DeepClone<T, T>();

        }

        /// <summary>
        /// 深度克隆 通过JSON形式 可以把 一个类型实体类数据 传递 给另一个不同类型的实体类 (只会传递两种类型实体类,属性名完全相同的属性值)
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <typeparam name="TTarget">The type of the t target.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>TTarget.</returns>
        public static TTarget DeepClone<TSource, TTarget>(this TSource source)
            where TSource : class
            where TTarget : class
        {
            return JsonSerializeHelper.DeserializeFromJson<TTarget>(JsonSerializeHelper.SerializeToJson(source));
        }

    }

}
