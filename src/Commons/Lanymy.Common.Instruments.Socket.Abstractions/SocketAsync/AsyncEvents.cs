//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using Lanymy.Common.Instruments.Common;

//namespace Lanymy.Common.Instruments.SocketAsync
//{

//    /// <summary>
//    /// tcp socket 异常回调
//    /// </summary>
//    /// <param name="client">TcpSocket</param>
//    /// <param name="ex">Exception</param>
//    public delegate Task TcpServerClientErrorEventAsync(IAsyncTcpServerClient client, Exception ex);

//    public delegate Task TcpReceiveDataEventAsync(IAsyncTcpServerClient tcpServerClient, BufferModel buffer, CacheModel cache);

//    public delegate Task TcpStartReceiveEventAsync(IAsyncTcpServerClient tcpServerClient);

//    public delegate Task TcpCloseEventAsync(IAsyncTcpServerClient tcpServerClient);

//    public delegate byte[] GetHeartBytesEvent(ISessionToken sessionToken);


//}
