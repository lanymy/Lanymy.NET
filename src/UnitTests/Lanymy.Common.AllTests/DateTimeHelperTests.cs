using System;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.Enums;
using Lanymy.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class DateTimeHelperTests
    {
        [TestMethod]
        public void UnixAndY2KRoundTripApis_ShouldRestoreOriginalDateTime()
        {
            var dateTimeWithSecondPrecision = new DateTime(2024, 5, 6, 7, 8, 9);
            var dateTimeWithMillisecondPrecision = new DateTime(2024, 5, 6, 7, 8, 9, 321);

            var unixSeconds = DateTimeHelper.GetTotalSecondsFrom1970(dateTimeWithSecondPrecision);
            var y2kMilliseconds = DateTimeHelper.GetTotalMillisecondsFrom2000(dateTimeWithMillisecondPrecision);

            Assert.AreEqual(dateTimeWithSecondPrecision, DateTimeHelper.GetDateTimeFrom1970TotalSeconds(unixSeconds));
            Assert.AreEqual(dateTimeWithMillisecondPrecision, DateTimeHelper.GetDateTimeFrom2000TotalMilliseconds(y2kMilliseconds));
        }

        [TestMethod]
        public void InstantiationApis_ShouldUseSharedInstantiationBaseLine()
        {
            var baseLine = DateTimeFormatKeys.START_DATE_TIME_INSTANTIATION;
            var secondsTarget = baseLine.AddSeconds(12);
            var millisecondsTarget = baseLine.AddMilliseconds(3456);

            Assert.AreEqual((uint)12, DateTimeHelper.GetTotalSecondsFromInstantiation(secondsTarget));
            Assert.AreEqual((uint)3456, DateTimeHelper.GetTotalMillisecondsFromInstantiation(millisecondsTarget));
            Assert.AreEqual(secondsTarget, DateTimeHelper.GetDateTimeFromInstantiationTotalSeconds(12));
            Assert.AreEqual(millisecondsTarget, DateTimeHelper.GetDateTimeFromInstantiationTotalMilliseconds(3456));
        }

        [TestMethod]
        public void QuarterApis_ShouldReturnQuarterBoundariesAndEnum()
        {
            var input = new DateTime(2024, 5, 15);

            Assert.AreEqual(new DateTime(2024, 4, 1), DateTimeHelper.GetQuarterStartDate(input));
            Assert.AreEqual(new DateTime(2024, 6, 30), DateTimeHelper.GetQuarterLastDate(input));
            Assert.AreEqual(DateQuarterEnum.SecondQuarter, DateTimeHelper.GetCurrentQuarter(input));
        }

        [TestMethod]
        public void GetIntervalMilliseconds_ShouldReturnSignedMillisecondsDifference()
        {
            var start = new DateTime(2024, 1, 1, 0, 0, 0, 0);
            var end = start.AddMilliseconds(1500);

            Assert.AreEqual(1500, DateTimeHelper.GetIntervalMilliseconds(start, end));
            Assert.AreEqual(-1500, DateTimeHelper.GetIntervalMilliseconds(end, start));
        }

        [TestMethod]
        public void GetMondayDateByDate_ShouldReturnWeekMonday_ForWeekdayAndSunday()
        {
            Assert.AreEqual(new DateTime(2024, 8, 12), DateTimeHelper.GetMondayDateByDate(new DateTime(2024, 8, 14)));
            Assert.AreEqual(new DateTime(2024, 8, 12), DateTimeHelper.GetMondayDateByDate(new DateTime(2024, 8, 18)));
        }

        [TestMethod]
        public void GetDaysInMonthAndJavaTimeApis_ShouldMatchFrameworkSemantics()
        {
            Assert.AreEqual(29, DateTimeHelper.GetDaysInMonth(2024, 2));
            Assert.AreEqual(new DateTime(1970, 1, 1, 8, 0, 0), DateTimeHelper.GetDateTimeFromJavaLongDateTime(0));
            Assert.AreEqual(new DateTime(1970, 1, 1, 0, 0, 0), DateTimeHelper.GetDateTimeFromJavaLongDateTime(0, 0));
        }
    }
}
