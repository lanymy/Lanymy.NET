namespace Lanymy.Common.Instruments
{


    public interface ILogoutSession
    {
        /// <summary>
        /// 登出帧缓存数据
        /// </summary>
        byte[] CacheConnectLogoutBytes { get; }
    }

}
