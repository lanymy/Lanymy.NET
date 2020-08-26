namespace Lanymy.Common.Abstractions.Models
{
    /// <summary>
    /// 调度文件 信息 数据实体类
    /// </summary>
    public class ScheduleFileInfoModel
    {


        /// <summary>
        /// 源文件全路径
        /// </summary>
        public string SourceFileFullPath { get; set; }

        /// <summary>
        /// 目标文件全路径
        /// </summary>
        public string TargetFileFullPath { get; set; }


        ///// <summary>
        ///// 为每个ScheduleFileInfoModel要执行的操作任务
        ///// </summary>
        ///// <param name="list"></param>
        ///// <param name="action"></param>
        //public static void DoWork(IEnumerable<ScheduleFileInfoModel> list, Action<ScheduleFileInfoModel> action)
        //{
        //    if (!action.IfIsNullOrEmpty())
        //    {

        //    }
        //}

    }
}
