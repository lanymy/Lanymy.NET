using System.Collections.Generic;
using System.Linq;
using System.Net;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{

    public class LanymyDownPackageDataModel : BaseLanymyPackageDataModel, IUdpPackage, ISendPackageSendNum
    {

        //private byte[] _SourceBytes;
        //public new byte[] SourceBytes { get { return _SourceBytes; } }

        public IPEndPoint RemoteIpEndPoint { get; set; }

        /// <summary>
        /// 模式
        /// </summary>
        public SelectModeTypeEnum SelectModeType { get; set; }
        /// <summary>
        /// 关机
        /// </summary>
        public bool IsPowerOff { get; set; }


        /// <summary>
        /// 点位ID
        /// </summary>
        public byte SoundPositionIndex { get; set; }

        /// <summary>
        /// 音频索引
        /// </summary>
        public byte SoundIndex { get; set; }

        /// <summary>
        /// 音量级别 0无效;1静音;8最大
        /// </summary>
        public byte SoundPower { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmo">编号</param>
        /// <param name="selectModeType">模式 </param>
        /// <param name="isPowerOff">关机</param>
        /// <param name="soundPositionIndex">听诊点</param>
        /// <param name="soundIndex">音频</param>
        /// <param name="soundPower">音量级别 0无效;1静音;8最大</param>
        public LanymyDownPackageDataModel
        (
            byte cmo,
            SelectModeTypeEnum selectModeType = SelectModeTypeEnum.UnDefine,
            bool isPowerOff = false,
            byte soundPositionIndex = 0,
            byte soundIndex = 0,
            byte soundPower = 0
        )
        {

            FrameType = FrameTypeEnum.Ask;
            CommandType = CommandTypeEnum.Control;
            CMO = cmo;
            SelectModeType = selectModeType;
            IsPowerOff = isPowerOff;
            SoundPositionIndex = soundPositionIndex;
            SoundIndex = soundIndex;
            SoundPower = soundPower;

            ResetSourceBytes();

        }

        private void ResetSourceBytes()
        {


            var listBytes = new List<byte>
            {
                170,
                2,
                1,
                6,
                CMO,
                (byte)SelectModeType,
                IsPowerOff?(byte)1:(byte)0,
                SoundPositionIndex,
                SoundIndex,
                SoundPower,
            };

            //效验和
            listBytes.Add((byte)listBytes.Sum(o => o));

            SourceBytes = listBytes.ToArray();

            listBytes.Clear();
            listBytes = null;

            SourceHex = BytesHelper.HexStringFromBytes(SourceBytes);

        }


        //public void ResetCMO(byte cmo)
        //{
        //    CMO = cmo;
        //    ResetSourceBytes();
        //}


        public byte SendNum { get; set; }
    }

}