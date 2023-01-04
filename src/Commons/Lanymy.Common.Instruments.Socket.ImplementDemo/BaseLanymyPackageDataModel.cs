namespace Lanymy.Common.Instruments
{
    public abstract class BaseLanymyPackageDataModel
    {
        /// <summary>
        /// 主机号
        /// </summary>
        public byte CMO { get; protected set; }
        public byte[] SourceBytes { get; protected set; }
        public string SourceHex { get; protected set; }
        public FrameTypeEnum FrameType { get; protected set; }
        public CommandTypeEnum CommandType { get; protected set; }





    }
}
