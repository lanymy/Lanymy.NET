namespace Lanymy.Common.Instruments
{


    public class LanymySessionToken : BaseSessionToken, ILoginSessionToken<byte>
    {

        public byte SessionTokenID { get; set; }
        public bool IsLogin { get; set; }
        public byte[] CacheConnectBytes { get; } = null;
        public override byte[] CacheHeartBytes { get; } = null;


        public LanymySessionToken(string ip, int port) : base(ip, port)
        {

        }

    }

}
