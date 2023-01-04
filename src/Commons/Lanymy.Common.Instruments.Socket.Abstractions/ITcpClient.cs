namespace Lanymy.Common.Instruments
{


    public interface ITcpClient : ITcp
    {

        string ServerIP { get; }

        int Port { get; }

        //bool Send(byte[] data);
        void Send(byte[] data);

        void Start();

        void Close();


    }


}
