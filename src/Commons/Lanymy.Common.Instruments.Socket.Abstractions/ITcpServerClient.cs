using System.Net;

namespace Lanymy.Common.Instruments
{

    public interface ITcpServerClient : ITcp
    {

        /// <summary>
        /// LocalEndPoint
        /// </summary>
        EndPoint LocalEndPoint { get; }

        /// <summary>
        /// RemoteEndPoint
        /// </summary>
        EndPoint RemoteEndPoint { get; }

        /// <summary>
        /// 用户定义的对象
        /// </summary>
        ISessionToken CurrentSessionToken { get; set; }

        //bool Send(byte[] data);
        void Send(byte[] data);

    }

}
