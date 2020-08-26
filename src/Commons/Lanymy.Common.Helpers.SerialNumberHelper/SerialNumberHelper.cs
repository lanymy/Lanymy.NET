using System;

namespace Lanymy.Common.Helpers
{
    /// <summary>
    /// 编号辅助类
    /// </summary>
    public class SerialNumberHelper
    {

        /// <summary>
        /// UTF8 编码 字符串长度为22 存储位数为176位 形式 流水号 (yyyyMMddHHmmssfff + UInt16)
        /// </summary>
        /// <returns></returns>
        public static string GetSerialNumber22StringLengthByGuidBytes()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff") + BitConverter.ToUInt16(Guid.NewGuid().ToByteArray(), 0).ToString("D5");
        }



        /// <summary>
        /// UTF8 编码 字符串长度为27 存储位数为216位 形式 流水号 (yyyyMMddHHmmssfff + UInt32)
        /// </summary>
        /// <returns></returns>
        public static string GetSerialNumber27StringLengthByGuidBytes()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff") + BitConverter.ToUInt32(Guid.NewGuid().ToByteArray(), 0).ToString("D10");
        }


        /// <summary>
        /// UTF8 编码 字符串长度为37 存储位数为296位 形式 流水号 (yyyyMMddHHmmssfff + UInt64)
        /// </summary>
        /// <returns></returns>
        public static string GetSerialNumber37StringLengthByGuidBytes()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff") + BitConverter.ToUInt64(Guid.NewGuid().ToByteArray(), 0).ToString("D20");
        }


        /// <summary>
        /// UTF8 编码 字符串长度为27 存储位数为216位 形式 流水号 (yyyyMMddHHmmssfff + UInt32)
        /// </summary>
        /// <returns></returns>
        public static string GetSerialNumber27StringLengthByGuidHashCode()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff") + BitConverter.ToUInt32(BitConverter.GetBytes(Guid.NewGuid().GetHashCode()), 0).ToString("D10");
        }


        /// <summary>
        /// 获取 长整型 数据格式的 流水号 (yyMMddHHmmss + 7位随机数)
        /// </summary>
        /// <returns></returns>
        public static long GetLongSerialNumber()
        {
            return long.Parse(DateTime.Now.ToString("yyMMddHHmmss") + new Random(Guid.NewGuid().GetHashCode()).Next(0, 10000000).ToString("D7"));
        }

    }
}
