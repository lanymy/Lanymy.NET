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
    }


}
