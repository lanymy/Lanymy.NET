namespace Lanymy.Common.Instruments
{


    public class SessionToken : BaseSessionToken
    {

        public override byte[] CacheHeartBytes => null;

        public SessionToken(string ip, int port) : base(ip, port)
        {

        }

    }
}
