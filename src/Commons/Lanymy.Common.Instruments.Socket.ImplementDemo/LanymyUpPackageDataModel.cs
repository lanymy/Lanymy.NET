using System.Net;
using Lanymy.Common.Helpers;

namespace Lanymy.Common.Instruments
{
    public class LanymyUpPackageDataModel : BaseLanymyPackageDataModel, IUdpPackage
    {

        public IPEndPoint RemoteIpEndPoint { get; set; }


        public BatteryLevelTypeEnum BatteryLevelType { get; }



        public LanymyUpPackageDataModel(byte[] dataBytes)
        {

            SourceBytes = dataBytes;
            SourceHex = BytesHelper.HexStringFromBytes(SourceBytes);
            CommandType = (CommandTypeEnum)SourceBytes[1];
            FrameType = (FrameTypeEnum)SourceBytes[2];
            CMO = SourceBytes[4];

            if (CommandType == CommandTypeEnum.Heart && FrameType == FrameTypeEnum.Answer)
            {
                BatteryLevelType = (BatteryLevelTypeEnum)SourceBytes[5];
            }

        }

        public LanymyUpPackageDataModel(byte cmo)
        {
            CMO = cmo;
        }


    }

}
