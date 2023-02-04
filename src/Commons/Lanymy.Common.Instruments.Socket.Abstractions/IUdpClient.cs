using System;

namespace Lanymy.Common.Instruments
{
    public interface IUdpClient : IDisposable
    {

        int Port { get; }

        bool IsAccept { get; }

        bool IsDisposed { get; }

        void Start();

        bool Send(byte[] data, string remoteIP, int remotePort);

        bool SendBroadcast(byte[] data, int broadcastPort);

        bool Send(SendUdpDataModel sendUdpDataModel);


        void Close();


    }

}
