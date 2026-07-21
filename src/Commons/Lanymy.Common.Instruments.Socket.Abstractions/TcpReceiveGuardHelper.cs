using System;
using System.IO;
using System.Net.Sockets;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{


    internal static class TcpReceiveGuardHelper
    {

        public static bool CanContinueReceive(bool isRunning, NetworkStream currentNetworkStream, NetworkStream activeNetworkStream)
        {
            return isRunning
                   && !currentNetworkStream.IfIsNull()
                   && ReferenceEquals(activeNetworkStream, currentNetworkStream);
        }

        public static bool CanIgnoreReceiveException(Exception exception, bool canContinueReceive)
        {
            if (canContinueReceive)
            {
                return false;
            }

            if (exception is ObjectDisposedException || exception is IOException)
            {
                return true;
            }

            if (exception is SocketException socketException)
            {
                return socketException.SocketErrorCode == SocketError.Interrupted
                       || socketException.SocketErrorCode == SocketError.OperationAborted
                       || socketException.SocketErrorCode == SocketError.NotSocket;
            }

            return false;
        }

    }


}
