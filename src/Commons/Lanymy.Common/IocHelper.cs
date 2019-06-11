using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommonServiceLocator;
using Lanymy.Common;
using Lanymy.Common.CustomAttributes;
using Lanymy.Common.ExtensionFunctions;
using Unity;


namespace Lanymy.Common
{
    /// <summary>
    /// IOC辅助操作类
    /// </summary>
    public class IocHelper
    {

        /// <summary>
        /// 当前 IOC 容器实例
        /// </summary>
        public static IUnityContainer CurrentContainer { get; }

        ///// <summary>
        ///// 当前 ServiceLocator 服务定位器 实例
        ///// </summary>
        //public static IServiceLocator CurrentServiceLocator { get; }

        static IocHelper()
        {

            CurrentContainer = new UnityContainer();
            //ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(CurrentContainer));
            //CurrentServiceLocator = ServiceLocator.Current;

        }




        /// <summary>
        /// 根据 IOC 特性 自动注入 程序集
        /// </summary>
        public static void AutoIocCallingAssembly()
        {
            AutoIocAssembly(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// 根据 IocRegisterAttribute 特性 自动注入 程序集
        /// </summary>
        /// <param name="assembly"></param>
        public static void AutoIocAssembly(Assembly assembly)
        {
            var container = CurrentContainer;
            foreach (var assemblyClassDefinedType in assembly.DefinedTypes.Where(o => o.IsClass))
            {

                foreach (var iocRegisterAttribute in ReflectionHelper.GetClassAttributeListFromModel<IocRegisterAttribute>(assemblyClassDefinedType))
                {
                    if (iocRegisterAttribute.RegisterType.IfIsNullOrEmpty())
                    {
                        container.RegisterType(assemblyClassDefinedType, iocRegisterAttribute.RegisterName);
                    }
                    else
                    {
                        container.RegisterType(iocRegisterAttribute.RegisterType, assemblyClassDefinedType, iocRegisterAttribute.RegisterName);
                    }
                }

            }
        }


        /// <summary>
        /// 根据 IocBaseClassRegisterAttribute 特性 自动注入 基类实例
        /// </summary>
        /// <param name="assembly"></param>
        public static void AutoIocBaseClassAssembly(Assembly assembly)
        {

            var container = CurrentContainer;


            //var childTypeList = assembly.GetTypes().Where(typeItem => typeItem.BaseType == parentType).ToList();
            var parentTypeList = new List<Type>();
            var childTypeList = new List<Type>();


            foreach (var assemblyClassDefinedType in assembly.DefinedTypes.Where(o => o.IsClass))
            {

                if (ReflectionHelper.GetClassAttributeListFromModel<IocBaseClassRegisterAttribute>(assemblyClassDefinedType).Any())
                {
                    parentTypeList.Add(assemblyClassDefinedType);
                }

            }

            foreach (var parentType in parentTypeList)
            {

                //var assemblyExpression = assembly.DefinedTypes.Where(o => o.IsClass);

                //if (parentType.IsGenericType)
                //{
                //    var parentTypeName = parentType.Name.LeftSubString("[");
                //    assemblyExpression = assemblyExpression.Where(o => o.BaseType.Name.StartsWith(parentTypeName));
                //}
                //else
                //{
                //    assemblyExpression = assemblyExpression.Where(o => o.BaseType == parentType);
                //}

                foreach (var assemblyClassDefinedType in assembly.DefinedTypes.Where(o => o.IsClass && o.BaseType.FullName != null && o.BaseType.FullName.StartsWith(parentType.FullName)))
                {
                    childTypeList.Add(assemblyClassDefinedType);
                }


            }


            foreach (var childType in childTypeList)
            {
                container.RegisterType(childType);
            }




        }






    }
}
