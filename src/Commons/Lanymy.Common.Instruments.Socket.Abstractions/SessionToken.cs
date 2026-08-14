namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 默认的会话令牌实现，不额外缓存心跳包。
    /// </summary>
    public class SessionToken : BaseSessionToken
    {
        /// <summary>
        /// 默认实现不持有心跳缓存。
        /// </summary>
        public override byte[] CacheHeartBytes => null;

        /// <summary>
        /// 初始化默认会话令牌。
        /// </summary>
        /// <param name="ip">对端 IP。</param>
        /// <param name="port">对端端口。</param>
        public SessionToken(string ip, int port) : base(ip, port)
        {
        }
    }
}
