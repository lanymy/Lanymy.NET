using System.Linq;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{

    public class LanymyFixedHeaderPackageFilter : BaseFixedHeaderPackageFilter<LanymyUpPackageDataModel, LanymyDownPackageDataModel, LanymySessionToken>
    {

        public LanymyFixedHeaderPackageFilter() : base(4)
        {

        }

        public override int GetBodyLengthFromHeader(int cursorIndex, byte[] bufferBytes)
        {
            return bufferBytes[cursorIndex + 3] + 1;
        }

        public override bool CheckPackage(byte[] packageBytes)
        {
            var len = packageBytes.Length;
            return packageBytes[0] == 0xDD && (byte)packageBytes.Take(len - 1).Sum(o => o) == packageBytes[len - 1];
        }

        public override byte[] EncodePackage(LanymyDownPackageDataModel sendPackage)
        {
            return sendPackage.SourceBytes;
        }

        public override LanymyUpPackageDataModel DecodePackage(byte[] packageBytes)
        {
            return new LanymyUpPackageDataModel(packageBytes);
        }

        private const string BATTERY_CAPACITY_HEX = "DD 01 01 06 00 00 00 00 00 00 B2";
        private byte[] _BatteryCapacityBytes;
        public byte[] BatteryCapacityBytes
        {
            get
            {
                if (_BatteryCapacityBytes == null)
                {
                    _BatteryCapacityBytes = BytesHelper.HexStringToBytes(BATTERY_CAPACITY_HEX);
                }
                return _BatteryCapacityBytes;
            }
        }
        public override byte[] GetHeartBytes(ISessionToken sessionToken)
        {
            return BatteryCapacityBytes;
        }
    }

}
