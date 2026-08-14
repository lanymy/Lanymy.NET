using System;
using System.IO;
using System.Net.Sockets;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 集中封装 TCP 接收循环的运行态校验和关闭噪音过滤逻辑。
    /// </summary>
    internal static class TcpReceiveGuardHelper
    {
        /// <summary>
        /// 判断当前接收循环是否还能继续使用指定流。
        /// </summary>
        /// <param name="isRunning">当前对象是否仍处于运行态。</param>
        /// <param name="currentNetworkStream">当前回调所持有的流快照。</param>
        /// <param name="activeNetworkStream">对象上最新的活动流。</param>
        /// <returns>仅当仍在运行且流快照仍然是当前活动流时返回 <see langword="true"/>。</returns>
        public static bool CanContinueReceive(bool isRunning, NetworkStream currentNetworkStream, NetworkStream activeNetworkStream)
        {
            return isRunning
                   && !currentNetworkStream.IfIsNull()
                   && ReferenceEquals(activeNetworkStream, currentNetworkStream);
        }

        /// <summary>
        /// 判断接收回调中的异常是否属于关闭交界期可忽略噪音。
        /// </summary>
        /// <param name="exception">捕获的异常。</param>
        /// <param name="canContinueReceive">当前是否仍允许继续接收。</param>
        /// <returns>返回 <see langword="true"/> 表示可以吞掉该异常。</returns>
        public static bool CanIgnoreReceiveException(Exception exception, bool canContinueReceive)
        {
            if (canContinueReceive)
            {
                return false;
            }

            // 流已经被关闭或替换后，ObjectDisposed / IO 失败通常只是关闭链路上的正常噪音。
            if (exception is ObjectDisposedException || exception is IOException)
            {
                return true;
            }

            // Socket 层面的一部分中断错误也属于关闭期预期行为，不应重新上报为运行态故障。
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
