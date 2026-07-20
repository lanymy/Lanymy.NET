using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class SessionTokenTests
    {
        private sealed class TestSessionToken : BaseSessionToken
        {
            public override byte[] CacheHeartBytes => null;

            public TestSessionToken(string ip, int port)
                : base(ip, port)
            {
            }
        }

        [TestMethod]
        public void SessionToken_ShouldInitializeLastReceiveMilliseconds()
        {
            var sessionToken = new TestSessionToken("127.0.0.1", 9527);

            Assert.AreEqual(sessionToken.ConnectionDateTime, sessionToken.LastReceiveDateTime);
            Assert.AreEqual(DateTimeHelper.GetTotalMillisecondsFromInstantiation(sessionToken.ConnectionDateTime), sessionToken.LastReceiveDateTimeTotalMillisecondsFromInstantiation);
        }
    }
}
