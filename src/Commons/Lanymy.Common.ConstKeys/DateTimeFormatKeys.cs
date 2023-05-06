


using System;

namespace Lanymy.Common.ConstKeys
{


    public class DateTimeFormatKeys
    {

        /// <summary>
        /// yyyy-MM-dd hh:mm:ss.fff 
        /// </summary>
        public const string DATE_TIME_FORMAT_1 = "yyyy-MM-dd HH:mm:ss.fff";

        /// <summary>
        /// yyyyMMddHHmmssfff
        /// </summary>
        public const string DATE_TIME_FORMAT_2 = "yyyyMMddHHmmssfff";

        /// <summary>
        /// 1970年1月1日 00:00:00
        /// </summary>
        public static readonly DateTime START_DATE_TIME_1970 = new DateTime(1970, 1, 1);
        /// <summary>
        /// 2000年1月1日 00:00:00
        /// </summary>
        public static readonly DateTime START_DATE_TIME_2000 = new DateTime(2000, 1, 1);
        /// <summary>
        /// 2020年1月1日 00:00:00
        /// </summary>
        public static readonly DateTime START_DATE_TIME_2020 = new DateTime(2020, 1, 1);

        /// <summary>
        /// 当前实例 实体化  的时间戳 
        /// </summary>
        public static readonly DateTime START_DATE_TIME_INSTANTIATION = DateTime.Now;



    }
}
