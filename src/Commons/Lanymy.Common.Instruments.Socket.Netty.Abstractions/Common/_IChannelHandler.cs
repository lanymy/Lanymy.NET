namespace Lanymy.Common.Instruments.Common
{
    /// <summary>
    /// 暴露通道会话和字节发送能力的最小 handler 视图。
    /// </summary>
    public interface IChannelClientHandler<out TChannelSession>
        where TChannelSession : BaseChannelSession
    {
        /// <summary>
        /// 当前 handler 绑定的会话对象。
        /// </summary>
        TChannelSession CurrentChannelSession { get; }

        /// <summary>
        /// 向当前通道发送原始字节。
        /// </summary>
        /// <param name="bytes">待发送字节。</param>
        void SendBytes(byte[] bytes);
    }
}
