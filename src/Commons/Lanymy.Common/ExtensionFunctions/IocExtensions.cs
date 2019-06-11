using CommonServiceLocator;
using Lanymy.Common.Interfaces;
using Lanymy.Common.Models;

namespace Lanymy.Common.ExtensionFunctions
{
    /// <summary>
    /// IOC 静态 扩展方法
    /// </summary>
    public static class IocExtensions
    {


        /// <summary>
        /// 根据 注册的基类 提权 到目标 实例
        /// </summary>
        /// <typeparam name="TBaseService"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static TService GetInstanceFromBase<TBaseService, TService>(this IServiceLocator o) where TService : TBaseService
        {

            TService service = default(TService);

            try
            {
                service = (TService)o.GetInstance<TBaseService>();
            }
            catch
            {

            }

            return service;

        }

        /// <summary>
        /// 根据 注册的基类 提权 到目标 实例
        /// </summary>
        /// <typeparam name="TBaseService"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <param name="o"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TService GetInstanceFromBase<TBaseService, TService>(this IServiceLocator o, string key) where TService : TBaseService
        {

            TService service = default(TService);

            try
            {
                service = (TService)o.GetInstance<TBaseService>(key);
            }
            catch
            {

            }

            return service;

        }


        /// <summary>
        /// 根据 IBaseIocRegister 基类 注册 接口 提权 到 目标 实例
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static TService GetInstanceFromBaseIocInterface<TService>(this IServiceLocator o) where TService : IBaseIocRegister
        {

            TService service = default(TService);

            try
            {
                service = (TService)o.GetInstance<IBaseIocRegister>();
            }
            catch
            {

            }

            return service;

        }


        /// <summary>
        /// 根据 IBaseIocRegister 基类 注册 接口 提权 到 目标 实例
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="o"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TService GetInstanceFromBaseIocInterface<TService>(this IServiceLocator o, string key) where TService : IBaseIocRegister
        {

            TService service = default(TService);

            try
            {
                service = (TService)o.GetInstance<IBaseIocRegister>(key);
            }
            catch
            {

            }

            return service;

        }


        /// <summary>
        /// 根据 IBaseIocRegister 基类 注册 接口 提权 到 目标 实例
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static TService GetInstanceFromBaseIocClass<TService>(this IServiceLocator o) where TService : BaseIocRegister
        {

            TService service = default(TService);

            try
            {
                service = (TService)o.GetInstance<BaseIocRegister>();
            }
            catch
            {

            }

            return service;

        }


        /// <summary>
        /// 根据 IBaseIocRegister 基类 注册 接口 提权 到 目标 实例
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="o"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TService GetInstanceFromBaseIocClass<TService>(this IServiceLocator o, string key) where TService : BaseIocRegister
        {

            TService service = default(TService);

            try
            {
                service = (TService)o.GetInstance<BaseIocRegister>(key);
            }
            catch
            {

            }

            return service;

        }


    }
}
