namespace Lanymy.Common.Instruments
{


    public interface ILoginSession
    {


        bool IsLogin { get; set; }

        /// <summary>
        /// 连接帧缓存数据
        /// </summary>
        byte[] CacheConnectBytes { get; }






    }

}
