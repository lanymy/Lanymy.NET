namespace Lanymy.Common.Instruments.Common
{


    public interface IChannelClientHandler<out TChannelSession>
        where TChannelSession : BaseChannelSession
    {

        TChannelSession CurrentChannelSession { get; }

        void SendBytes(byte[] bytes);

    }

}
