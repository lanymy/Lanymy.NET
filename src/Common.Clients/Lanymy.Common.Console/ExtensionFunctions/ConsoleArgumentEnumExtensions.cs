using System;
using System.Collections.Generic;
using System.Text;
using Lanymy.Common.Console.CustomAttributes;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Console.ExtensionFunctions
{


    public static class ConsoleArgumentEnumExtensions
    {

        /// <summary>
        /// 提取当前控制台传入的参数值
        /// </summary>
        /// <param name="currentEnum"></param>
        /// <returns></returns>
        public static string GetConsoleInputArgumentData<TConsoleArgumentEnumAttribute>(this Enum currentEnum) where TConsoleArgumentEnumAttribute : BaseConsoleArgumentEnumAttribute
        {

            return EnumHelper.GetEnumItem(currentEnum).EnumCustomAttribute.AsType<TConsoleArgumentEnumAttribute>().ConsoleArgumentModel.InputArgumentData;

        }

    }


}
