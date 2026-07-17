using Lanymy.Common.Instruments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class CustomMemoryCacheTests
    {
        [TestMethod]
        public void SetValue_ShouldOverwriteExistingValue()
        {
            var cache = new CustomMemoryCache();

            cache.SetValue("test_key", "old");
            cache.SetValue("test_key", "new");

            var result = cache.GetValue("test_key");

            Assert.AreEqual("new", result);
        }
    }
}
