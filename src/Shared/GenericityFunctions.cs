/********************************************************************

时间: 2016年07月01日, AM 09:43:46

作者: lanyanmiyu@qq.com

描述: 泛型辅助方法扩展类

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.ExtensionFunctions;

namespace Lanymy.General.Extension
{


    /// <summary>
    /// 泛型辅助方法扩展类
    /// </summary>
    public class GenericityFunctions
    {


        /// <summary>
        /// 获取接口实例
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="tInterface"></param>
        /// <param name="defaultInterface"></param>
        /// <returns></returns>
        public static TInterface GetInterface<TInterface>(TInterface tInterface, TInterface defaultInterface)
        {
            return tInterface.IfIsNullOrEmpty() ? defaultInterface : tInterface;
        }


        /// <summary>
        /// 通用泛型接口执行扩展方法
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <typeparam name="TReturn">返回值类型</typeparam>
        /// <param name="tInterface">接口实例</param>
        /// <param name="funcInvoke">执行接口的方法</param>
        /// <returns></returns>
        public static TReturn InvokeInterface<TInterface, TReturn>(TInterface tInterface, Func<TInterface, TReturn> funcInvoke)
        {
            return funcInvoke(tInterface);
        }

        /// <summary>
        /// 通用泛型接口执行扩展方法
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <typeparam name="TInParameter">接口方法传入的参数类型</typeparam>
        /// <typeparam name="TReturn">返回值类型</typeparam>
        /// <param name="tInterface">接口实例</param>
        /// <param name="tInParameter">接口执行方法的参数</param>
        /// <param name="funcInvoke">执行接口的方法</param>
        /// <returns></returns>
        public static TReturn InvokeInterface<TInterface, TInParameter, TReturn>(TInterface tInterface, TInParameter tInParameter, Func<TInterface, TInParameter, TReturn> funcInvoke)
        {
            return funcInvoke(tInterface, tInParameter);
        }


    }


}
