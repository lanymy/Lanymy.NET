using Lanymy.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class RegexHelperTests
    {
        [TestMethod]
        public void BasicValidationApis_ShouldMatchCurrentPatterns()
        {
            Assert.IsTrue(RegexHelper.IsPhoneNumber("13800138000"));
            Assert.IsFalse(RegexHelper.IsPhoneNumber("23800138000"));

            Assert.IsTrue(RegexHelper.IsAllNumeric(string.Empty));
            Assert.IsTrue(RegexHelper.IsAllNumeric("012345"));
            Assert.IsFalse(RegexHelper.IsAllNumeric("12A"));

            Assert.IsTrue(RegexHelper.IsPositiveNum("+12"));
            Assert.IsFalse(RegexHelper.IsPositiveNum("0"));
            Assert.IsFalse(RegexHelper.IsPositiveNum("-12"));

            Assert.IsTrue(RegexHelper.IsIP("192.168.1.10"));
            Assert.IsFalse(RegexHelper.IsIP("256.168.1.10"));
        }

        [TestMethod]
        public void NameAndTextValidationApis_ShouldRespectHistoricalBoundaries()
        {
            Assert.IsTrue(RegexHelper.IsChineseName("张三"));
            Assert.IsFalse(RegexHelper.IsChineseName("张"));

            Assert.IsTrue(RegexHelper.IsChineseOrEnglishName("Lanymy Net"));
            Assert.IsTrue(RegexHelper.IsChineseOrEnglishName("张三"));
            Assert.IsFalse(RegexHelper.IsChineseOrEnglishName("张三A"));

            Assert.IsTrue(RegexHelper.IsChineseChar("测试"));
            Assert.IsFalse(RegexHelper.IsChineseChar("测试A"));

            Assert.IsTrue(RegexHelper.IsHexString("1AF0"));
            Assert.IsFalse(RegexHelper.IsHexString("1af0"));
        }

        [TestMethod]
        public void UrlEmailAndTimeApis_ShouldMatchCurrentPatterns()
        {
            Assert.IsTrue(RegexHelper.IsEmail("demo@example.com"));
            Assert.IsFalse(RegexHelper.IsEmail("demo@example"));

            Assert.IsTrue(RegexHelper.IsUrl("https://example.com/demo?id=1"));
            Assert.IsFalse(RegexHelper.IsUrl("ftp://example.com"));

            Assert.IsTrue(RegexHelper.IsTime("23:59:59"));
            Assert.IsTrue(RegexHelper.IsTime("8:05"));
            Assert.IsFalse(RegexHelper.IsTime("24:00:00"));
        }

        [TestMethod]
        public void WildcardAndPathApis_ShouldMatchCurrentMatchingRules()
        {
            Assert.IsTrue(RegexHelper.CheckWithWildcard("lanymy.log", "*.log"));
            Assert.IsTrue(RegexHelper.CheckWithWildcard("ab", "a?"));
            Assert.IsFalse(RegexHelper.CheckWithWildcard("lanymy.txt", "*.log"));

            Assert.IsTrue(RegexHelper.IsAbsolutePath(@"C:\Temp\demo.txt"));
            Assert.IsFalse(RegexHelper.IsAbsolutePath(@"demo.txt"));

            Assert.IsTrue(RegexHelper.IsRelativePath("."));
            Assert.IsTrue(RegexHelper.IsRelativePath("./logs/demo.txt"));
            Assert.IsTrue(RegexHelper.IsRelativePath("../logs/demo.txt"));
            Assert.IsTrue(RegexHelper.IsRelativePath("logs/demo.txt"));
            Assert.IsFalse(RegexHelper.IsRelativePath(@"C:\logs\demo.txt"));
            Assert.IsFalse(RegexHelper.IsRelativePath("//server/share"));
        }
    }
}
