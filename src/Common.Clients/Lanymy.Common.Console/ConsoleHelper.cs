using System;
using System.Linq;
using Lanymy.Common.Console.CustomAttributes;
using Lanymy.Common.Console.Models;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Console
{


    /// <summary>
    /// 控制台 通用 辅助类
    /// </summary>
    public class ConsoleHelper
    {


        /// <summary>
        /// 解析 命令行 参数
        /// </summary>
        /// <typeparam name="TConsoleArgumentEnum">命令行参数枚举类型</typeparam>
        /// <typeparam name="TConsoleArgumentEnumAttribute">命令行参数枚举特性类型</typeparam>
        /// <param name="consoleArgs">命令行 参数 数组</param>
        /// <param name="createConsoleArgumentModelDelegate">创建 传入的 命令行参数 实体类  委托 默认值 null 则使用"ConsoleArgumentModel"默认的构造类</param>
        /// <returns></returns>
        public static ConsoleArgumentMatchResultModel MatchConsoleArguments<TConsoleArgumentEnum, TConsoleArgumentEnumAttribute>(string[] consoleArgs, CreateConsoleArgumentModelDelegate createConsoleArgumentModelDelegate = null)
            where TConsoleArgumentEnum : Enum
            where TConsoleArgumentEnumAttribute : BaseConsoleArgumentEnumAttribute
        {

            var resultModel = new ConsoleArgumentMatchResultModel
            {
                IsSuccess = false,
            };

            if (createConsoleArgumentModelDelegate.IfIsNullOrEmpty())
            {
                createConsoleArgumentModelDelegate = (inputArgumentsTitle, inputArgumentData) => new ConsoleArgumentModel(inputArgumentsTitle, inputArgumentData);
            }

            string errorMessage = string.Empty;

            var argsLength = consoleArgs.Length;

            System.Console.WriteLine("命令行参数解析开始");

            var enumList = EnumHelper.GetEnumItemList<TConsoleArgumentEnum>();

            if (argsLength % 2 == 0)
            {


                string argsTitle = string.Empty;
                string argsData = string.Empty;

                for (int i = 0; i < argsLength; i += 2)
                {

                    argsTitle = consoleArgs[i];
                    argsData = consoleArgs[i + 1];

                    System.Console.WriteLine();
                    System.Console.WriteLine("命令行参数名 [ {0} ]", argsTitle);
                    System.Console.WriteLine("命令行参数数据 [ {0} ]", argsData);

                    if (!argsTitle.StartsWith("-"))
                    {
                        errorMessage += string.Format("命令行参数名 [ {0} ] 格式不正确,命令行参数名请以'-'符号开头", argsTitle) + Environment.NewLine;
                        continue;
                    }

                    argsTitle = argsTitle.Substring(1);

                    var consoleArgumentModel = createConsoleArgumentModelDelegate?.Invoke(argsTitle, argsData);

                    //var currentConsoleArgumentEnumItem = enumList.Where(o => o.CurrentEnum.ToString() == consoleArgumentModel?.InputArgumentTitle).FirstOrDefault();
                    var currentConsoleArgumentEnumItem = enumList.Where(o => (o.EnumCustomAttribute as BaseConsoleArgumentEnumAttribute)?.SourceArgumentTitle == consoleArgumentModel.InputArgumentTitle).FirstOrDefault();

                    if (!currentConsoleArgumentEnumItem.IfIsNullOrEmpty())
                    {
                        currentConsoleArgumentEnumItem.EnumCustomAttribute.AsType<TConsoleArgumentEnumAttribute>().ConsoleArgumentModel = consoleArgumentModel;
                    }
                    else
                    {
                        errorMessage += string.Format("命令行参数名 [ {0} ] , 属于未定义命令行参数,此命令行参数无效", argsTitle) + Environment.NewLine;
                    }

                }


                foreach (var enumItem in enumList)
                {

                    var currentConsoleArgumentEnumAttribute = enumItem.EnumCustomAttribute.AsType<TConsoleArgumentEnumAttribute>();
                    if (!currentConsoleArgumentEnumAttribute.IfIsNullOrEmpty() && currentConsoleArgumentEnumAttribute.IsRequired && currentConsoleArgumentEnumAttribute.ConsoleArgumentModel.IfIsNullOrEmpty())
                    {
                        errorMessage += string.Format("命令行参数名 [ {0} ] , 属于必填项", enumItem.CurrentEnum) + Environment.NewLine;
                    }

                }


            }
            else
            {
                errorMessage += "命令行参数 个数不匹配" + Environment.NewLine; ;
            }


            System.Console.WriteLine("命令行参数解析结束");

            if (errorMessage.IfIsNullOrEmpty())
            {
                resultModel.IsSuccess = true;
            }
            else
            {
                resultModel.IsSuccess = false;
                resultModel.ErrorMessage = errorMessage;
            }


            return resultModel;

        }

    }

}
