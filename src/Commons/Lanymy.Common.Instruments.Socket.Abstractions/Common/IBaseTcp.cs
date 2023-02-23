using System;

namespace Lanymy.Common.Instruments.Common
{



    public interface IBaseTcp
    {

        System.Net.Sockets.Socket CurrentSocket { get; }

        /// <summary>
        /// �Ƿ�����
        /// </summary>
        bool IsConnected { get; }

        ///// <summary>
        ///// �Ƿ��ͷ�
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


    }



}
