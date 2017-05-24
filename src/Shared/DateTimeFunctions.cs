/********************************************************************

时间: 2015年03月17日, PM 04:26:17

作者: lanyanmiyu@qq.com

描述: DateTime辅助类

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Models;

namespace Lanymy.General.Extension
{


    /// <summary>
    /// DateTime辅助类
    /// </summary>
    public class DateTimeFunctions
    {

        private static readonly DateTime START_DATE_TIME = new DateTime(1970, 1, 1);

        /// <summary>
        /// 获取从1970开始到参数的总秒数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static ulong GetTotalSecondsFrom1970(DateTime dt)
        {
            return dt.Subtract(START_DATE_TIME).TotalSeconds.ConvertToType<ulong>();
        }


        /// <summary>
        /// 从1970开始的总秒数 计算出 时间
        /// </summary>
        /// <param name="totalSeconds1970"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeFrom1970TotalSeconds(ulong totalSeconds1970)
        {
            return START_DATE_TIME.AddSeconds(totalSeconds1970);
        }


        /// <summary>
        /// 获取从1970开始到参数的总毫秒数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static ulong GetTotalMillisecondsFrom1970(DateTime dt)
        {
            return dt.Subtract(START_DATE_TIME).TotalMilliseconds.ConvertToType<ulong>();
        }


        /// <summary>
        /// 从1970开始的总毫秒数 计算出 时间
        /// </summary>
        /// <param name="totalMilliseconds1970"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeFrom1970TotalMilliseconds(ulong totalMilliseconds1970)
        {
            return START_DATE_TIME.AddMilliseconds(totalMilliseconds1970);
        }


        /// <summary>
        /// 获取日期所在季度的第一天起始日期
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetQuarterStartDate(DateTime dt)
        {
            dt = dt.AddMonths(0 - (dt.Month - 1) % 3);
            return new DateTime(dt.Year, dt.Month, 1);
        }

        /// <summary>
        /// 获取日期所在季度的最后一天日期
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetQuarterLastDate(DateTime dt)
        {
            return GetQuarterStartDate(dt).AddMonths(3).AddDays(-1);
        }

        /// <summary>
        /// 获取当前日期第几季度枚举值
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateQuarterEnum GetCurrentQuarter(DateTime dt)
        {
            return GetQuarterStartDate(dt).Month.ConvertToEnum<DateQuarterEnum>();
        }


        /// <summary>
        /// 获取两个日期时间 的间隔 总毫秒数
        /// </summary>
        /// <param name="oldDateTime"></param>
        /// <param name="newDateTIme"></param>
        /// <returns></returns>
        public static int GetIntervalMilliseconds(DateTime oldDateTime, DateTime newDateTIme)
        {
            return (newDateTIme - oldDateTime).TotalMilliseconds.ConvertToType<int>(-1);
        }


        /// <summary>
        /// 获取公历y年m月的总天数
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns></returns>
        public static int GetDaysInMonth(int year, int month)
        {
            //int[] days = new int[] { 31, DateTime.IsLeapYear(year) ? 29 : 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            //return days[m - 1];
            return DateTime.DaysInMonth(year, month);
        }


        /// <summary>
        /// 根据日期值获得周一的日期
        /// </summary>
        /// <param name="dt">输入日期</param>
        /// <returns>周一的日期</returns>
        public static DateTime GetMondayDateByDate(DateTime dt)
        {

            //Sunday = 0,
            //Monday = 1,
            //Tuesday = 2,
            //Wednesday = 3,
            //Thursday = 4,
            //Friday = 5,
            //Saturday = 6,

            double d = 0;
            switch ((int)dt.DayOfWeek)
            {
                //case 1: d = 0; break;
                case 2: d = -1; break;
                case 3: d = -2; break;
                case 4: d = -3; break;
                case 5: d = -4; break;
                case 6: d = -5; break;
                case 0: d = -6; break;
            }
            return dt.AddDays(d);
        }


        ///// <summary>
        ///// 根据阳历日期获取阴历日期
        ///// </summary>
        ///// <param name="dt"></param>
        ///// <returns></returns>
        //public static ChineseDate GetChineseDate(DateTime dt)
        //{
        //    return new ChineseDate(dt);
        //}


    }
}
