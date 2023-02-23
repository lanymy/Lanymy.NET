using System.Net;

namespace Lanymy.Common.Instruments
{


    public class UdpSourceDataModel //: IWorkTaskQueueDataModel
    {

        public IPEndPoint RemoteIPEndPoint { get; set; }
        public byte[] SourceDataBytes { get; set; }
    }

}
