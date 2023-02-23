using System;

namespace Lanymy.Common.Instruments.Common
{

    public interface ISessionToken
    {

        Guid SessionID { get; }

        string IP { get; }

        int Port { get; }

        byte SendNum { get; }

        byte[] CacheHeartBytes { get; }

        int IntervalHeartTotalMilliseconds { get; set; }


        DateTime ConnectionDateTime { get; }

        DateTime LastReceiveDateTime { get; set; }

        DateTime LastSendDateTime { get; set; }


    }


}
