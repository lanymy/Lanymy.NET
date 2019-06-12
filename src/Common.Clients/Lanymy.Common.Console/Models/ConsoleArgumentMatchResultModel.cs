namespace Lanymy.Common.Console.Models
{


    /// <summary>
    /// 控制台 命令行  参数  解析 返回信息 实体类
    /// </summary>
    public class ConsoleArgumentMatchResultModel
    {

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

    }

}
