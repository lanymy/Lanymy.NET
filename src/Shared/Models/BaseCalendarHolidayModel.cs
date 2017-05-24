/********************************************************************

时间: 2016年12月08日, PM 10:10:37

作者: lanyanmiyu@qq.com

描述: 日期节假日数据基类

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
    /// 日期节假日数据基类
    /// </summary>
    public abstract class BaseCalendarHolidayModel
    {

        /// <summary>
        /// 月份
        /// </summary>
        public int Month { get; set; }
        /// <summary>
        /// 当月中的第几天
        /// </summary>
        public int Day { get; set; }
        /// <summary>
        /// 当月第几周
        /// </summary>
        public int WeekAtMonth { get; set; }
        /// <summary>
        /// 一周的第几天
        /// </summary>
        public int WeekDay { get; set; }
        /// <summary>
        /// 休息几天
        /// </summary>
        public int RecessDays { get; set; }
        /// <summary>
        /// 节日名称
        /// </summary>
        public string HolidayName { get; set; }


    }

}
