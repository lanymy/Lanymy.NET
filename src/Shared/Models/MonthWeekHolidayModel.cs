///********************************************************************

//时间: 2016年12月08日, PM 10:32:09

//作者: lanyanmiyu@qq.com

//描述: 某月 第几个星期几 节假日 数据实体类

//其它:     

//********************************************************************/




//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Lanymy.General.Extension.Models
//{

//    /// <summary>
//    /// 某月 第几个星期几 节假日 数据实体类
//    /// </summary>
//    public class MonthWeekHolidayModel : BaseCalendarHolidayModel
//    {


//        private static readonly List<MonthWeekHolidayModel> _MonthWeekHolidayList = new List<MonthWeekHolidayModel>()
//        {
//            new MonthWeekHolidayModel(5, 2, 1,0, "母亲节"),
//            new MonthWeekHolidayModel(5, 3, 1, 0,"全国助残日"),
//            new MonthWeekHolidayModel(6, 3, 1, 0,"父亲节"),
//            new MonthWeekHolidayModel(9, 3, 3,0, "国际和平日"),
//            new MonthWeekHolidayModel(9, 4, 1, 0,"国际聋人节"),
//            new MonthWeekHolidayModel(10, 1, 2, 0,"国际住房日"),
//            new MonthWeekHolidayModel(10, 1, 4, 0,"国际减轻自然灾害日"),
//            new MonthWeekHolidayModel(11, 4, 5, 0,"感恩节")
//        };


//        /// <summary>
//        /// 某月 第几个星期几节假日数据列表
//        /// </summary>
//        /// <returns></returns>
//        public static
//#if NET40
//            IList<MonthWeekHolidayModel>
//#else
//            IReadOnlyList<MonthWeekHolidayModel> 
//#endif
//            GetMonthWeekHolidayList()
//        {
//            return _MonthWeekHolidayList;
//        }


//        /// <summary>
//        /// 某月第几个星期几节假日数据实体类构造方法
//        /// </summary>
//        /// <param name="month">月份</param>
//        /// <param name="weekAtMonth">当前月第几周</param>
//        /// <param name="weekDay">一周的第几天</param>
//        /// <param name="recessDays">休息几天</param>
//        /// <param name="holidayName">节日名称</param>
//        public MonthWeekHolidayModel(int month, int weekAtMonth, int weekDay, int recessDays, string holidayName)
//        {
//            Month = month;
//            WeekAtMonth = weekAtMonth;
//            WeekDay = weekDay;
//            RecessDays = recessDays;
//            HolidayName = holidayName;
//        }

//    }

//}
