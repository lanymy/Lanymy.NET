using System;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 定义 socket 会话的基础身份和收发时间戳信息。
    /// </summary>
    public abstract class BaseSessionToken : ISessionToken
    {
        /// <summary>
        /// 当前连接会话的唯一标识。
        /// </summary>
        public Guid SessionID { get; }

        /// <summary>
        /// 连接建立时记录的对端 IP。
        /// </summary>
        public string IP { get; }

        /// <summary>
        /// 连接建立时记录的对端端口。
        /// </summary>
        public int Port { get; }

        private volatile byte _SendNum = 0;

        /// <summary>
        /// 返回递增的发送序号，供协议层构造包序号使用。
        /// </summary>
        public byte SendNum => _SendNum++;

        /// <summary>
        /// 当前会话缓存的心跳包。
        /// </summary>
        public abstract byte[] CacheHeartBytes { get; }

        private volatile uint _LastReceiveDateTimeTotalMillisecondsFromInstantiation;

        /// <summary>
        /// 最近一次接收数据距离实例化基线的毫秒数。
        /// </summary>
        public uint LastReceiveDateTimeTotalMillisecondsFromInstantiation
        {
            get
            {
                return _LastReceiveDateTimeTotalMillisecondsFromInstantiation;
            }
            set
            {
                _LastReceiveDateTimeTotalMillisecondsFromInstantiation = value;
            }
        }

        /// <summary>
        /// 连接建立时间。
        /// </summary>
        public DateTime ConnectionDateTime { get; }

        /// <summary>
        /// 最近一次接收数据时间。
        /// </summary>
        public DateTime LastReceiveDateTime { get; set; }

        /// <summary>
        /// 最近一次发送数据时间。
        /// </summary>
        public DateTime LastSendDateTime { get; set; }

        /// <summary>
        /// 初始化会话令牌。
        /// </summary>
        /// <param name="ip">对端 IP。</param>
        /// <param name="port">对端端口。</param>
        protected BaseSessionToken(string ip, int port)
        {
            SessionID = Guid.NewGuid();
            IP = ip;
            Port = port;
            ConnectionDateTime = DateTime.Now;
            LastReceiveDateTime = ConnectionDateTime;
            LastReceiveDateTimeTotalMillisecondsFromInstantiation = DateTimeHelper.GetTotalMillisecondsFromInstantiation(ConnectionDateTime);
            LastSendDateTime = ConnectionDateTime;
        }
    }
}
