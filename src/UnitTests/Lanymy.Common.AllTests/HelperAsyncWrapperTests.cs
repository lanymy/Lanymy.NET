using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class HelperAsyncWrapperTests
    {
        private sealed class JsonPayload
        {
            public string Name { get; set; }
        }

        [TestMethod]
        public async Task JsonSerializeHelper_AsyncWrappers_ShouldMatchSyncBehavior()
        {
            var payload = new JsonPayload
            {
                Name = "lanymy",
            };

            var syncJson = JsonSerializeHelper.SerializeToJson(payload);
            var asyncJson = await JsonSerializeHelper.SerializeToJsonAsync(payload);
            var asyncModel = await JsonSerializeHelper.DeserializeFromJsonAsync<JsonPayload>(syncJson);

            Assert.AreEqual(syncJson, asyncJson);
            Assert.IsNotNull(asyncModel);
            Assert.AreEqual(payload.Name, asyncModel.Name);
        }

        [TestMethod]
        public async Task CompressionHelper_AsyncWrappers_ShouldMatchSyncBehavior()
        {
            var sourceBytes = Encoding.UTF8.GetBytes("lanymy async wrapper");

            var syncCompressed = CompressionHelper.CompressBytesToBytes(sourceBytes);
            var asyncCompressed = await CompressionHelper.CompressBytesToBytesAsync(sourceBytes);
            var asyncDecompressed = await CompressionHelper.DecompressBytesFromBytesAsync(syncCompressed);

            CollectionAssert.AreEqual(syncCompressed, asyncCompressed);
            CollectionAssert.AreEqual(sourceBytes, asyncDecompressed);
        }

        [TestMethod]
        public async Task GenericityHelper_DoTaskWorkAsync_ForSyncFunc_ShouldReturnResult()
        {
            var result = await GenericityHelper.DoTaskWorkAsync(static () => 42);

            Assert.AreEqual(42, result);
        }
    }
}
