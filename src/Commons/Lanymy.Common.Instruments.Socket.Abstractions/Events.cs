using System;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{



    ///// <summary>
    ///// tcp socket 连接成功回调
    ///// </summary>
    ///// <param name="client">TcpSocket</param>
    ///// <param name="isSuccess">isSuccess</param>
    //public delegate void TcpConnectEvent(ITcpServerClient client, bool isSuccess);

    /// <summary>
    /// tcp socket 异常回调
    /// </summary>
    /// <param name="client">TcpSocket</param>
    /// <param name="ex">Exception</param>
    public delegate void TcpServerClientErrorEvent(ITcpServerClient client, Exception ex);


    ///// <summary>
    ///// tcp soket 正在接收数据回调
    ///// </summary>
    ///// <param name="client">TcpSocketAsync</param>
    ///// <param name="length">数据总长度</param>
    //public delegate void TcpReadingEvent(ITcpServerClient client, int length);


    ///// <summary>
    ///// tcp socket 接收数据回调
    ///// </summary>
    ///// <param name="client">TcpSocket</param>
    //public delegate void TcpReceiveEvent(ITcpServerClient client, List<byte[]> receiveDataBytesList);
    ////public delegate void TcpReceiveEvent(ITcpServerClient client, byte[] receiveDataBytes);


    public delegate void TcpReceiveDataEvent(ITcpServerClient tcpServerClient, BufferModel buffer, CacheModel cache);

    public delegate void TcpStartReceiveEvent(ITcpServerClient tcpServerClient);
    public delegate void TcpCloseEvent(ITcpServerClient tcpServerClient);

    public delegate void TcpHeartEvent(ITcpServerClient tcpServerClient);



    //public delegate void TcpAcceptEvent(ITcpServerClient client);

    //public delegate void TcpServerErrorEvent(ITcpServer server, Exception ex);




    ///// <summary>
    ///// udp socket 接收数据回调
    ///// </summary>
    ///// <param name="remoteEP">发送端终结点</param>
    ///// <param name="data">接收的数据</param>
    ///// <param name="length">接收数据长度</param>
    //public delegate void UdpReceiveEvent(EndPoint remoteEP, byte[] data, int length);


    ///// <summary>
    ///// udp socket 异常回调
    ///// </summary>
    ///// <param name="remoteEP">发送端终结点</param>
    ///// <param name="ex">异常</param>
    //public delegate void UdpServerErrorEvent(EndPoint remoteEP, Exception ex);

    ///// <summary>
    ///// 心跳回调
    ///// </summary>
    ///// <param name="client"></param>
    //public delegate void HeartEvent(ITcpServerClient client);



}
