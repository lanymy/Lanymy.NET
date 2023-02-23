using System;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{
    public abstract class BaseSessionToken : ISessionToken
    {
        public Guid SessionID { get; }

        public string IP { get; }

        public int Port { get; }

        private volatile byte _SendNum = 0;

        public byte SendNum => _SendNum++;

        public abstract byte[] CacheHeartBytes { get; }

        private volatile int _IntervalHeartTotalMilliseconds;
        public int IntervalHeartTotalMilliseconds
        {
            get
            {
                return _IntervalHeartTotalMilliseconds;
            }
            set
            {
                _IntervalHeartTotalMilliseconds = value;
            }
        }


        public DateTime ConnectionDateTime { get; }
        public DateTime LastReceiveDateTime { get; set; }
        public DateTime LastSendDateTime { get; set; }


        protected BaseSessionToken(string ip, int port)
        {

            SessionID = Guid.NewGuid();
            IP = ip;
            Port = port;
            ConnectionDateTime = DateTime.Now;
            LastReceiveDateTime = ConnectionDateTime;
            LastSendDateTime = ConnectionDateTime;

        }




    }

}
