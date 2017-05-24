///********************************************************************

//时间: 2016年12月08日, PM 10:08:37

//作者: lanyanmiyu@qq.com

//描述: 阳历节假日数据实体类

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
//	/// 阳历节假日数据实体类
//	/// </summary>
//	public class SolarCalendarHolidayModel : BaseCalendarHolidayModel
//	{

//		private readonly static List<SolarCalendarHolidayModel> _SolarCalendarHolidayList = new List<SolarCalendarHolidayModel>()
//		{ 
//			new SolarCalendarHolidayModel(1, 1, 1, "元旦"),
//			new SolarCalendarHolidayModel(2, 2, 0, "世界湿地日"),
//			new SolarCalendarHolidayModel(2, 10, 0, "国际气象节"),
//			new SolarCalendarHolidayModel(2, 14, 0, "情人节"),
//			new SolarCalendarHolidayModel(3, 1, 0, "国际海豹日"),
//			new SolarCalendarHolidayModel(3, 5, 0, "学雷锋纪念日"),
//			new SolarCalendarHolidayModel(3, 8, 0, "妇女节"), 
//			new SolarCalendarHolidayModel(3, 12, 0, "植树节 孙中山逝世纪念日"), 
//			new SolarCalendarHolidayModel(3, 14, 0, "国际警察日"),
//			new SolarCalendarHolidayModel(3, 15, 0, "消费者权益日"),
//			new SolarCalendarHolidayModel(3, 17, 0, "中国国医节 国际航海日"),
//			new SolarCalendarHolidayModel(3, 21, 0, "世界森林日 消除种族歧视国际日 世界儿歌日"),
//			new SolarCalendarHolidayModel(3, 22, 0, "世界水日"),
//			new SolarCalendarHolidayModel(3, 24, 0, "世界防治结核病日"),
//			new SolarCalendarHolidayModel(4, 1, 0, "愚人节"),
//			new SolarCalendarHolidayModel(4, 7, 0, "世界卫生日"),
//			new SolarCalendarHolidayModel(4, 22, 0, "世界地球日"),
//			new SolarCalendarHolidayModel(5, 1, 1, "劳动节"), 
//			new SolarCalendarHolidayModel(5, 2, 1, "劳动节假日"),
//			new SolarCalendarHolidayModel(5, 3, 1, "劳动节假日"),
//			new SolarCalendarHolidayModel(5, 4, 0, "青年节"), 
//			new SolarCalendarHolidayModel(5, 8, 0, "世界红十字日"),
//			new SolarCalendarHolidayModel(5, 12, 0, "国际护士节"), 
//			new SolarCalendarHolidayModel(5, 31, 0, "世界无烟日"), 
//			new SolarCalendarHolidayModel(6, 1, 0, "国际儿童节"), 
//			new SolarCalendarHolidayModel(6, 5, 0, "世界环境保护日"),
//			new SolarCalendarHolidayModel(6, 26, 0, "国际禁毒日"),
//			new SolarCalendarHolidayModel(7, 1, 0, "建党节 香港回归纪念 世界建筑日"),
//			new SolarCalendarHolidayModel(7, 11, 0, "世界人口日"),
//			new SolarCalendarHolidayModel(8, 1, 0, "建军节"), 
//			new SolarCalendarHolidayModel(8, 8, 0, "中国男子节 父亲节"),
//			new SolarCalendarHolidayModel(8, 15, 0, "抗日战争胜利纪念"),
//			new SolarCalendarHolidayModel(9, 9, 0, "毛主席逝世纪念"), 
//			new SolarCalendarHolidayModel(9, 10, 0, "教师节"), 
//			new SolarCalendarHolidayModel(9, 18, 0, "九·一八事变纪念日"),
//			new SolarCalendarHolidayModel(9, 20, 0, "国际爱牙日"),
//			new SolarCalendarHolidayModel(9, 27, 0, "世界旅游日"),
//			new SolarCalendarHolidayModel(9, 28, 0, "孔子诞辰"),
//			new SolarCalendarHolidayModel(10, 1, 1, "国庆节 国际音乐日"),
//			new SolarCalendarHolidayModel(10, 2, 1, "国庆节假日"),
//			new SolarCalendarHolidayModel(10, 3, 1, "国庆节假日"),
//			new SolarCalendarHolidayModel(10, 6, 0, "老人节"), 
//			new SolarCalendarHolidayModel(10, 24, 0, "联合国日"),
//			new SolarCalendarHolidayModel(11, 10, 0, "世界青年节"),
//			new SolarCalendarHolidayModel(11, 12, 0, "孙中山诞辰纪念"), 
//			new SolarCalendarHolidayModel(12, 1, 0, "世界艾滋病日"), 
//			new SolarCalendarHolidayModel(12, 3, 0, "世界残疾人日"), 
//			new SolarCalendarHolidayModel(12, 20, 0, "澳门回归纪念"), 
//			new SolarCalendarHolidayModel(12, 24, 0, "平安夜"), 
//			new SolarCalendarHolidayModel(12, 25, 0, "圣诞节"), 
//			new SolarCalendarHolidayModel(12, 26, 0, "毛主席诞辰纪念"),
//		};


//		/// <summary>
//		/// 阳历节假日数据列表
//		/// </summary>
//		/// <returns></returns>
//		public static IReadOnlyList<SolarCalendarHolidayModel> GetSolarCalendarHolidayList()
//		{
//			return _SolarCalendarHolidayList;
//		}


//		/// <summary>
//		/// 阳历节假日数据实体类构造方法
//		/// </summary>
//		/// <param name="month">月份</param>
//		/// <param name="day">当月中的第几天</param>
//		/// <param name="recessDays">休息几天</param>
//		/// <param name="holidayName">节日名称</param>
//		public SolarCalendarHolidayModel(int month, int day, int recessDays, string holidayName)
//		{
//			Month = month;
//			Day = day;
//			RecessDays = recessDays;
//			HolidayName = holidayName;
//		}
//	}


//}
