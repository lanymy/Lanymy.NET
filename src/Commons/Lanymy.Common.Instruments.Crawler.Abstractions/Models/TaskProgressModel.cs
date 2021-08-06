using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Models
{
    public class TaskProgressModel
    {

        /// <summary>
        /// 总任务数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 已完成任务数
        /// </summary>
        public int CompleteCount { get; set; }

    }

}
