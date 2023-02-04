using System.Net;

namespace Lanymy.Common.Instruments
{

    public class SendUdpDataModel : IUdpPackage
    {

        public IPEndPoint RemoteIpEndPoint { get; set; }

        public byte[] PackageBytes { get; }

        public SendUdpDataModel(IPEndPoint remoteIpEndPoint, byte[] packageBytes)
        {
            RemoteIpEndPoint = remoteIpEndPoint;
            PackageBytes = packageBytes;
        }

    }

}
