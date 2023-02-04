namespace Lanymy.Common.Instruments
{


    public interface ITcpServer : ITcp
    {

        bool IsAccept { get; }

        int Port { get; }

        void Start();



    }

}
