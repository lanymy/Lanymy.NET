namespace Lanymy.Common.Console.Models
{

    /// <summary>
    /// 控制台 命令行参数 元数据 实体类 基类
    /// </summary>
    public abstract class BaseConsoleArgumentModel
    {

        /// <summary>
        /// 命令行参数 字符串名称
        /// </summary>
        public string InputArgumentTitle { get; }

        /// <summary>
        /// 命令行参数 数据
        /// </summary>
        public string InputArgumentData { get; }

        /// <summary>
        /// 控制台 命令行参数 元数据 实体类 基类 构造方法
        /// </summary>
        /// <param name="inputArgumentTitle">传入的命令行参数 名称</param>
        /// <param name="inputArgumentData">传入的命令行参数 原始数据</param>
        protected BaseConsoleArgumentModel(string inputArgumentTitle, string inputArgumentData)
        {

            InputArgumentTitle = inputArgumentTitle;
            InputArgumentData = inputArgumentData;

        }

    }

}
