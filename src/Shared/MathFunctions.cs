// *******************************************************************
// 创建时间：2015年01月14日, AM 10:32:05
// 作者：lanyanmiyu@qq.com
// 说明：数学计算公式和算法
// 其它:
// *******************************************************************



using System;


namespace Lanymy.General.Extension
{

    /// <summary>
    /// 数学计算公式和算法
    /// </summary>
    public class MathFunctions
    {


        /// <summary>
        /// 经纬度之间的距离单位Km
        /// </summary>
        /// <param name="fromX">起点经度</param>
        /// <param name="fromY">起点纬度</param>
        /// <param name="toX">终点经度</param>
        /// <param name="toY">终点纬度</param>
        /// <returns></returns>
        public static double CalcDistance(double fromX, double fromY, double toX, double toY)
        {
            double rad = 6371; //Earth radius in Km
            double p1X = fromX / 180 * Math.PI;
            double p1Y = fromY / 180 * Math.PI;
            double p2X = toX / 180 * Math.PI;
            double p2Y = toY / 180 * Math.PI;
            return Math.Acos(Math.Sin(p1Y) * Math.Sin(p2Y) +
                Math.Cos(p1Y) * Math.Cos(p2Y) * Math.Cos(p2X - p1X)) * rad;
        }
    }
}
