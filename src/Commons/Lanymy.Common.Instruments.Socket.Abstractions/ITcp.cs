using System;

namespace Lanymy.Common.Instruments
{
    public interface ITcp : IDisposable
    {

        System.Net.Sockets.Socket CurrentSocket { get; }
        /// <summary>
        /// 是否连接
        /// </summary>
        bool IsConnected { get; }

        ///// <summary>
        ///// 是否释放
        ///// </summary>
        //bool IsDisposed { get; }
        /// <summary>
        /// ReceiveBufferSize
        /// </summary>
        int ReceiveBufferSize { get; }

        /// <summary>
        /// SendBufferSize
        /// </summary>
        int SendBufferSize { get; }


        bool IsRunning { get; }

        void Close();

    }

}
