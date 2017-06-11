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
        /// 执行Task任务
        /// </summary>
        /// <typeparam name="TReturn">返回参数</typeparam>
        /// <param name="taskWork">要执行的Task任务</param>
        /// <returns></returns>
        public static Task<TReturn> DoTaskWork<TReturn>(Func<TReturn> taskWork)
        {

#if NET40
            return new Task<TReturn>(taskWork);
#else
            return Task.FromResult(taskWork());
#endif
        }

        /// <summary>
        /// 执行Task任务
        /// </summary>
        /// <typeparam name="TParameter1">传入参数1</typeparam>
        /// <typeparam name="TReturn">返回参数</typeparam>
        /// <param name="taskWork">要执行的Task任务</param>
        /// <param name="parameter1">参数1</param>
        /// <returns></returns>
        public static Task<TReturn> DoTaskWork<TParameter1, TReturn>(Func<TParameter1, TReturn> taskWork, TParameter1 parameter1)
        {

#if NET40
            return new Task<TReturn>(() => taskWork(parameter1));
#else
            return Task.FromResult(taskWork(parameter1));
#endif
        }

        /// <summary>
        /// 执行Task任务
        /// </summary>
        /// <typeparam name="TParameter1">传入参数1</typeparam>
        /// <typeparam name="TParameter2">传入参数2</typeparam>
        /// <typeparam name="TReturn">返回参数</typeparam>
        /// <param name="taskWork">要执行的Task任务</param>
        /// <param name="parameter1">参数1</param>
        /// <param name="parameter2">参数2</param>
        /// <returns></returns>
        public static Task<TReturn> DoTaskWork<TParameter1, TParameter2, TReturn>(Func<TParameter1, TParameter2, TReturn> taskWork, TParameter1 parameter1, TParameter2 parameter2)
        {
#if NET40
            return new Task<TReturn>(() => taskWork(parameter1, parameter2));
#else
            return Task.FromResult(taskWork(parameter1, parameter2));
#endif
        }

        /// <summary>
        /// 执行Task任务
        /// </summary>
        /// <typeparam name="TParameter1">传入参数1</typeparam>
        /// <typeparam name="TParameter2">传入参数2</typeparam>
        /// <typeparam name="TParameter3">传入参数3</typeparam>
        /// <typeparam name="TReturn">返回参数</typeparam>
        /// <param name="taskWork">要执行的Task任务</param>
        /// <param name="parameter1">参数1</param>
        /// <param name="parameter2">参数2</param>
        /// <param name="parameter3">参数3</param>
        /// <returns></returns>
        public static Task<TReturn> DoTaskWork<TParameter1, TParameter2, TParameter3, TReturn>(Func<TParameter1, TParameter2, TParameter3, TReturn> taskWork, TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3)
        {
#if NET40
            return new Task<TReturn>(() => taskWork(parameter1, parameter2, parameter3));
#else
            return Task.FromResult(taskWork(parameter1, parameter2, parameter3));
#endif
        }


        /// <summary>
        /// 执行Task任务
        /// </summary>
        /// <typeparam name="TParameter1">传入参数1</typeparam>
        /// <typeparam name="TParameter2">传入参数2</typeparam>
        /// <typeparam name="TParameter3">传入参数3</typeparam>
        /// <typeparam name="TParameter4">传入参数4</typeparam>
        /// <typeparam name="TReturn">返回参数</typeparam>
        /// <param name="taskWork">要执行的Task任务</param>
        /// <param name="parameter1">参数1</param>
        /// <param name="parameter2">参数2</param>
        /// <param name="parameter3">参数3</param>
        /// <param name="parameter4">参数4</param>
        /// <returns></returns>
        public static Task<TReturn> DoTaskWork<TParameter1, TParameter2, TParameter3, TParameter4, TReturn>(Func<TParameter1, TParameter2, TParameter3, TParameter4, TReturn> taskWork, TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4)
        {
#if NET40
            return new Task<TReturn>(() => taskWork(parameter1, parameter2, parameter3, parameter4));
#else
            return Task.FromResult(taskWork(parameter1, parameter2, parameter3, parameter4));
#endif
        }


        /// <summary>
        /// 执行Task任务
        /// </summary>
        /// <typeparam name="TParameter1">传入参数1</typeparam>
        /// <typeparam name="TParameter2">传入参数2</typeparam>
        /// <typeparam name="TParameter3">传入参数3</typeparam>
        /// <typeparam name="TParameter4">传入参数4</typeparam>
        /// <typeparam name="TParameter5">传入参数5</typeparam>
        /// <typeparam name="TReturn">返回参数</typeparam>
        /// <param name="taskWork">要执行的Task任务</param>
        /// <param name="parameter1">参数1</param>
        /// <param name="parameter2">参数2</param>
        /// <param name="parameter3">参数3</param>
        /// <param name="parameter4">参数4</param>
        /// <param name="parameter5">参数5</param>
        /// <returns></returns>
        public static Task<TReturn> DoTaskWork<TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TReturn>(Func<TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TReturn> taskWork, TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, TParameter5 parameter5)
        {
#if NET40
            return new Task<TReturn>(() => taskWork(parameter1, parameter2, parameter3, parameter4, parameter5));
#else
            return Task.FromResult(taskWork(parameter1, parameter2, parameter3, parameter4, parameter5));
#endif
        }


        /// <summary>
        /// 执行Task任务
        /// </summary>
        /// <typeparam name="TParameter1">传入参数1</typeparam>
        /// <typeparam name="TParameter2">传入参数2</typeparam>
        /// <typeparam name="TParameter3">传入参数3</typeparam>
        /// <typeparam name="TParameter4">传入参数4</typeparam>
        /// <typeparam name="TParameter5">传入参数5</typeparam>
        /// <typeparam name="TParameter6">传入参数6</typeparam>
        /// <typeparam name="TReturn">返回参数</typeparam>
        /// <param name="taskWork">要执行的Task任务</param>
        /// <param name="parameter1">参数1</param>
        /// <param name="parameter2">参数2</param>
        /// <param name="parameter3">参数3</param>
        /// <param name="parameter4">参数4</param>
        /// <param name="parameter5">参数5</param>
        /// <param name="parameter6">参数6</param>
        /// <returns></returns>
        public static Task<TReturn> DoTaskWork<TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TReturn>(Func<TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TReturn> taskWork, TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, TParameter5 parameter5, TParameter6 parameter6)
        {
#if NET40
            return new Task<TReturn>(() => taskWork(parameter1, parameter2, parameter3, parameter4, parameter5, parameter6));
#else
            return Task.FromResult(taskWork(parameter1, parameter2, parameter3, parameter4, parameter5, parameter6));
#endif
        }



        /// <summary>
        /// 执行Task任务 无返回参数
        /// </summary>
        /// <param name="taskWork">要执行的Task任务</param>
        /// <returns></returns>
        public static Task DoTaskWork(Action taskWork)
        {

#if NET40
            return new Task(taskWork);
#else
            return Task.Run(taskWork);
#endif

        }



        /// <summary>
        /// 执行Task任务 无返回参数
        /// </summary>
        /// <typeparam name="TParameter1">传入参数1</typeparam>
        /// <param name="taskWork">要执行的Task任务</param>
        /// <param name="parameter1">参数1</param>
        /// <returns></returns>
        public static Task DoTaskWork<TParameter1>(Action<TParameter1> taskWork, TParameter1 parameter1)
        {

#if NET40
            return new Task(() => taskWork(parameter1));
#else
            return Task.Run(() => taskWork(parameter1));
#endif

        }

        /// <summary>
        /// 执行Task任务 无返回参数
        /// </summary>
        /// <typeparam name="TParameter1">传入参数1</typeparam>
        /// <typeparam name="TParameter2">传入参数2</typeparam>
        /// <param name="taskWork">要执行的Task任务</param>
        /// <param name="parameter1">参数1</param>
        /// <param name="parameter2">参数2</param>
        /// <returns></returns>
        public static Task DoTaskWork<TParameter1, TParameter2>(Action<TParameter1, TParameter2> taskWork, TParameter1 parameter1, TParameter2 parameter2)
        {
#if NET40
            return new Task(() => taskWork(parameter1, parameter2));
#else
            return Task.Run(() => taskWork(parameter1, parameter2));
#endif
        }

        /// <summary>
        /// 执行Task任务 无返回参数
        /// </summary>
        /// <typeparam name="TParameter1">传入参数1</typeparam>
        /// <typeparam name="TParameter2">传入参数2</typeparam>
        /// <typeparam name="TParameter3">传入参数3</typeparam>
        /// <param name="taskWork">要执行的Task任务</param>
        /// <param name="parameter1">参数1</param>
        /// <param name="parameter2">参数2</param>
        /// <param name="parameter3">参数3</param>
        /// <returns></returns>
        public static Task DoTaskWork<TParameter1, TParameter2, TParameter3>(Action<TParameter1, TParameter2, TParameter3> taskWork, TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3)
        {
#if NET40
            return new Task(() => taskWork(parameter1, parameter2, parameter3));
#else
            return Task.Run(() => taskWork(parameter1, parameter2, parameter3));
#endif
        }

        /// <summary>
        /// 执行Task任务 无返回参数
        /// </summary>
        /// <typeparam name="TParameter1">传入参数1</typeparam>
        /// <typeparam name="TParameter2">传入参数2</typeparam>
        /// <typeparam name="TParameter3">传入参数3</typeparam>
        /// <typeparam name="TParameter4">传入参数4</typeparam>
        /// <param name="taskWork">要执行的Task任务</param>
        /// <param name="parameter1">参数1</param>
        /// <param name="parameter2">参数2</param>
        /// <param name="parameter3">参数3</param>
        /// <param name="parameter4">参数4</param>
        /// <returns></returns>
        public static Task DoTaskWork<TParameter1, TParameter2, TParameter3, TParameter4>(Action<TParameter1, TParameter2, TParameter3, TParameter4> taskWork, TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4)
        {
#if NET40
            return new Task(() => taskWork(parameter1, parameter2, parameter3, parameter4));
#else
            return Task.Run(() => taskWork(parameter1, parameter2, parameter3, parameter4));
#endif
        }


        /// <summary>
        /// 执行Task任务 无返回参数
        /// </summary>
        /// <typeparam name="TParameter1">传入参数1</typeparam>
        /// <typeparam name="TParameter2">传入参数2</typeparam>
        /// <typeparam name="TParameter3">传入参数3</typeparam>
        /// <typeparam name="TParameter4">传入参数4</typeparam>
        /// <typeparam name="TParameter5">传入参数5</typeparam>
        /// <param name="taskWork">要执行的Task任务</param>
        /// <param name="parameter1">参数1</param>
        /// <param name="parameter2">参数2</param>
        /// <param name="parameter3">参数3</param>
        /// <param name="parameter4">参数4</param>
        /// <param name="parameter5">参数5</param>
        /// <returns></returns>
        public static Task DoTaskWork<TParameter1, TParameter2, TParameter3, TParameter4, TParameter5>(Action<TParameter1, TParameter2, TParameter3, TParameter4, TParameter5> taskWork, TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, TParameter5 parameter5)
        {
#if NET40
            return new Task(() => taskWork(parameter1, parameter2, parameter3, parameter4, parameter5));
#else
            return Task.Run(() => taskWork(parameter1, parameter2, parameter3, parameter4, parameter5));
#endif
        }


        /// <summary>
        /// 执行Task任务 无返回参数
        /// </summary>
        /// <typeparam name="TParameter1">传入参数1</typeparam>
        /// <typeparam name="TParameter2">传入参数2</typeparam>
        /// <typeparam name="TParameter3">传入参数3</typeparam>
        /// <typeparam name="TParameter4">传入参数4</typeparam>
        /// <typeparam name="TParameter5">传入参数5</typeparam>
        /// <typeparam name="TParameter6"></typeparam>
        /// <param name="taskWork">要执行的Task任务</param>
        /// <param name="parameter1">参数1</param>
        /// <param name="parameter2">参数2</param>
        /// <param name="parameter3">参数3</param>
        /// <param name="parameter4">参数4</param>
        /// <param name="parameter5">参数5</param>
        /// <param name="parameter6"></param>
        /// <returns></returns>
        public static Task DoTaskWork<TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6>(Action<TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6> taskWork, TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, TParameter5 parameter5, TParameter6 parameter6)
        {
#if NET40
            return new Task(() => taskWork(parameter1, parameter2, parameter3, parameter4, parameter5, parameter6));
#else
            return Task.Run(() => taskWork(parameter1, parameter2, parameter3, parameter4, parameter5, parameter6));
#endif
        }





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
