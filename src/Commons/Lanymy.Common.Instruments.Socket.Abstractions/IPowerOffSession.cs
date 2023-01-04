namespace Lanymy.Common.Instruments
{
    public interface IPowerOffSession
    {
        /// <summary>
        /// 关机帧缓存数据
        /// </summary>
        byte[] CachePowerOffBytes { get; }
    }

}
