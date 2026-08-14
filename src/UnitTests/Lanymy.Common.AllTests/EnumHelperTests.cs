using System;
using System.Linq;
using Lanymy.Common.Enums;
using Lanymy.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class EnumHelperTests
    {
        [Flags]
        private enum TestFlagsEnum
        {
            None = 0,
            Read = 1,
            Write = 2,
            Execute = 4,
        }

        [TestMethod]
        public void EnumMapApis_ShouldReturnCachedMappingAndItems()
        {
            var dictionary1 = EnumHelper.GetEnumItemDictionary<DateQuarterEnum>();
            var dictionary2 = EnumHelper.GetEnumItemDictionary(typeof(DateQuarterEnum));
            var enumItem = EnumHelper.GetEnumItem(DateQuarterEnum.FirstQuarter);

            Assert.AreSame(dictionary1, dictionary2);
            Assert.AreEqual(4, dictionary1.Count);
            Assert.AreEqual(DateQuarterEnum.FirstQuarter, enumItem.CurrentEnum);
        }

        [TestMethod]
        public void GetEnumItemListRemoveEnumItems_ShouldExcludeSpecifiedItemsOnly()
        {
            var items = EnumHelper.GetEnumItemListRemoveEnumItems(DateQuarterEnum.SecondQuarter, DateQuarterEnum.FourthQuarter);
            var values = items.Select(o => (DateQuarterEnum)o.CurrentEnum).ToList();

            CollectionAssert.AreEquivalent(
                new[]
                {
                    DateQuarterEnum.FirstQuarter,
                    DateQuarterEnum.ThirdQuarter,
                },
                values);
        }

        [TestMethod]
        public void FlagsApis_ShouldSplitCompositeFlagsAndEvaluateHasFlag()
        {
            var composite = TestFlagsEnum.Read | TestFlagsEnum.Execute;
            var dictionary = EnumHelper.GetEnumFlagsItemDictionary(composite);
            var list = EnumHelper.GetEnumFlagsItemList(composite);

            Assert.AreEqual(2, dictionary.Count);
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(dictionary.Keys.Cast<TestFlagsEnum>().Contains(TestFlagsEnum.Read));
            Assert.IsTrue(dictionary.Keys.Cast<TestFlagsEnum>().Contains(TestFlagsEnum.Execute));
            Assert.IsTrue(EnumHelper.HasFlag(composite, TestFlagsEnum.Read));
            Assert.IsFalse(EnumHelper.HasFlag(composite, TestFlagsEnum.Write));
        }
    }
}
