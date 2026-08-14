using Lanymy.Common.ConstKeys;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 保存 socket 半包缓存数据及当前位置。
    /// </summary>
    public class CacheModel
    {
        /// <summary>
        /// 半包缓存区。
        /// </summary>
        public readonly byte[] Data;

        /// <summary>
        /// 当前缓存有效数据长度。
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// 初始化缓存模型。
        /// </summary>
        /// <param name="bufferSize">缓存区大小。</param>
        public CacheModel(int bufferSize = BufferSizeKeys.BUFFER_SIZE_8K)
        {
            Data = new byte[bufferSize];
            Clear();
        }

        /// <summary>
        /// 清空当前缓存位置。
        /// </summary>
        public void Clear()
        {
            Position = 0;
        }
    }
}
