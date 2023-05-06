using System;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.Enums;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Helpers
{

    /// <summary>
    /// DateTime辅助类
    /// </summary>
    public class DateTimeHelper
    {

        private static readonly DateTime START_DATE_TIME_1970 = DateTimeFormatKeys.START_DATE_TIME_1970;
        private static readonly DateTime START_DATE_TIME_2000 = DateTimeFormatKeys.START_DATE_TIME_2000;
        private static readonly DateTime START_DATE_TIME_INSTANTIATION = DateTimeFormatKeys.START_DATE_TIME_INSTANTIATION;


        /// <summary>
        /// 获取两个时间差 总 间隔 毫秒数
        /// </summary>
        /// <param name="dt">被减数时间</param>
        /// <param name="subtractDT">减数时间</param>
        /// <returns></returns>
        public static ulong GetTotalMilliseconds(DateTime dt, DateTime subtractDT)
        {
            return (ulong)(dt.Subtract(subtractDT).TotalMilliseconds);
        }

        /// <summary>
        /// 获取两个时间差 总 间隔 秒数
        /// </summary>
        /// <param name="dt">被减数时间</param>
        /// <param name="subtractDT">减数时间</param>
        /// <returns></returns>
        public static ulong GetTotalSeconds(DateTime dt, DateTime subtractDT)
        {
            return (ulong)(dt.Subtract(subtractDT).TotalSeconds);
        }






        /// <summary>
        /// 获取从1970开始到参数的总秒数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static ulong GetTotalSecondsFrom1970(DateTime dt)
        {
            return GetTotalSeconds(dt, START_DATE_TIME_1970);
        }


        /// <summary>
        /// 获取从1970开始到参数的总毫秒数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static ulong GetTotalMillisecondsFrom1970(DateTime dt)
        {
            return GetTotalMilliseconds(dt, START_DATE_TIME_1970);
        }


        /// <summary>
        /// 从1970开始的总秒数 计算出 时间
        /// </summary>
        /// <param name="totalSeconds1970"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeFrom1970TotalSeconds(ulong totalSeconds1970)
        {
            return START_DATE_TIME_1970.AddSeconds(totalSeconds1970);
        }


        /// <summary>
        /// 从1970开始的总毫秒数 计算出 时间
        /// </summary>
        /// <param name="totalMilliseconds1970"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeFrom1970TotalMilliseconds(ulong totalMilliseconds1970)
        {
            return START_DATE_TIME_1970.AddMilliseconds(totalMilliseconds1970);
        }






        /// <summary>
        /// 获取从2000开始到参数的总秒数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static ulong GetTotalSecondsFrom2000(DateTime dt)
        {
            return GetTotalSeconds(dt, START_DATE_TIME_2000);
        }


        /// <summary>
        /// 获取从2000开始到参数的总毫秒数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static ulong GetTotalMillisecondsFrom2000(DateTime dt)
        {
            return GetTotalMilliseconds(dt, START_DATE_TIME_2000);
        }

        /// <summary>
        /// 从2000开始的总秒数 计算出 时间
        /// </summary>
        /// <param name="totalSeconds2000"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeFrom2000TotalSeconds(ulong totalSeconds2000)
        {
            return START_DATE_TIME_2000.AddSeconds(totalSeconds2000);
        }


        /// <summary>
        /// 从2000开始的总毫秒数 计算出 时间
        /// </summary>
        /// <param name="totalMilliseconds2000"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeFrom2000TotalMilliseconds(ulong totalMilliseconds2000)
        {
            return START_DATE_TIME_2000.AddMilliseconds(totalMilliseconds2000);
        }








        /// <summary>
        /// 获取从实例化时间开始到参数的总秒数; 无最大值溢出风险;
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static uint GetTotalSecondsFromInstantiation(DateTime dt)
        {
            return (uint)(dt.Subtract(START_DATE_TIME_INSTANTIATION).TotalSeconds);
        }


        /// <summary>
        /// 获取从实例化时间开始到参数的总毫秒数; 有最大值溢出风险 => 最大时间 为 当前实例时间戳 加上 2个月
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static uint GetTotalMillisecondsFromInstantiation(DateTime dt)
        {
            return (uint)(dt.Subtract(START_DATE_TIME_INSTANTIATION).TotalMilliseconds);
        }

        /// <summary>
        /// 从实例化时间开始的总秒数 计算出 时间
        /// </summary>
        /// <param name="totalSecondsInstantiation"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromInstantiationTotalSeconds(ulong totalSecondsInstantiation)
        {
            return START_DATE_TIME_INSTANTIATION.AddSeconds(totalSecondsInstantiation);
        }


        /// <summary>
        /// 从实例化时间开始的总毫秒数 计算出 时间
        /// </summary>
        /// <param name="totalMillisecondsInstantiation"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromInstantiationTotalMilliseconds(ulong totalMillisecondsInstantiation)
        {
            return START_DATE_TIME_INSTANTIATION.AddMilliseconds(totalMillisecondsInstantiation);
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


        /// <summary>
        /// 从 Java的LongDateTime 值 转换成 对应时区 的 C# DateTime 等效值
        /// </summary>
        /// <param name="javaLongDateTime">Java的LongDateTime值</param>
        /// <param name="timeZone">时区 默认值 东八区 8</param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromJavaLongDateTime(long javaLongDateTime, int timeZone = 8)
        {

            long ticks1970 = START_DATE_TIME_1970.Ticks;
            long timeTotalTicks = ticks1970 + javaLongDateTime * 10000;
            return new DateTime(timeTotalTicks).AddHours(timeZone);

        }



    }
}
