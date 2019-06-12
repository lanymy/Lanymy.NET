using Lanymy.Common.Console.Models;

namespace Lanymy.Common.Console
{

    /// <summary>
    /// 创建 ConsoleArgumentModel 委托
    /// </summary>
    /// <param name="inputArgumentsTitle">命令行 参数 名称</param>
    /// <param name="inputArgumentData">命令行 参数 数据</param>
    /// <returns></returns>
    public delegate BaseConsoleArgumentModel CreateConsoleArgumentModelDelegate(string inputArgumentsTitle, string inputArgumentData);

}
