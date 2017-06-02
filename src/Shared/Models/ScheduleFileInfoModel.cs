/********************************************************************

时间: 2016年11月14日, AM 09:12:08

作者: lanyanmiyu@qq.com

描述: 调度文件 信息 数据实体类

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.ExtensionFunctions;

namespace Lanymy.General.Extension.Models
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
