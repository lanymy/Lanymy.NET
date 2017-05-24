///********************************************************************

//时间: 2016年12月08日, PM 10:26:22

//作者: lanyanmiyu@qq.com

//描述: 阴历节假日数据实体类

//其它:     

//********************************************************************/



//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Lanymy.General.Extension.Models
//{

//	/// <summary>
//	/// 阴历节假日数据实体类
//	/// </summary>
//	public class LunarCalendarHolidayModel : BaseCalendarHolidayModel
//	{

//		private static readonly List<LunarCalendarHolidayModel> _LunarCalendarHolidayList = new List<LunarCalendarHolidayModel>()
//		{
//			new LunarCalendarHolidayModel(1, 1, 1, "春节"),
//			new LunarCalendarHolidayModel(1, 15, 0, "元宵节"),
//			new LunarCalendarHolidayModel(5, 5, 0, "端午节"),
//			new LunarCalendarHolidayModel(7, 7, 0, "七夕情人节"),
//			new LunarCalendarHolidayModel(7, 15, 0, "中元节 盂兰盆节"),
//			new LunarCalendarHolidayModel(8, 15, 0, "中秋节"),
//			new LunarCalendarHolidayModel(9, 9, 0, "重阳节"),
//			new LunarCalendarHolidayModel(12, 8, 0, "腊八节"),
//			new LunarCalendarHolidayModel(12, 23, 0, "北方小年(扫房)"),
//			new LunarCalendarHolidayModel(12, 24, 0, "南方小年(掸尘)"),
//			//new LunarHolidayStruct(12, 30, 0, "除夕")  //注意除夕需要其它方法进行计算
//		};

//		/// <summary>
//		/// 阳历节假日数据列表
//		/// </summary>
//		/// <returns></returns>
//		public static
//#if NET40
//			IList<LunarCalendarHolidayModel>

//#else
//			IReadOnlyList<LunarCalendarHolidayModel> 
//#endif
//			GetLunarCalendarHolidayList()
//		{
//			return _LunarCalendarHolidayList;
//		}



//		/// <summary>
//		/// 阴历节假日数据实体类构造方法
//		/// </summary>
//		/// <param name="month">月份</param>
//		/// <param name="day">当月中的第几天</param>
//		/// <param name="recessDays">休息几天</param>
//		/// <param name="holidayName">节日名称</param>
//		public LunarCalendarHolidayModel(int month, int day, int recessDays, string holidayName)
//		{
//			Month = month;
//			Day = day;
//			RecessDays = recessDays;
//			HolidayName = holidayName;
//		}
//	}

//}
