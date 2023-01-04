using System;
using Lanymy.Common.ConstKeys;

namespace Lanymy.Common.Instruments
{


    public class BufferModel
    {


        private const int MAX_BUFFER_SIZE = BufferSizeKeys.BUFFER_SIZE_8K;

        /// <summary>
        /// 缓存
        /// </summary>
        public byte[] BufferData { get; set; }

        /// <summary>
        /// 成功读取位置
        /// </summary>
        public int Position { get; set; }
        public int CursorIndex { get; set; }

        /// <summary>
        /// 缓存大小
        /// </summary>
        public int BufferSize { get; }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="bufferSize">缓存大小</param>
        public BufferModel(int bufferSize = MAX_BUFFER_SIZE)
        {

            if (bufferSize <= 0)
            {
                throw new ArgumentException(nameof(bufferSize) + " is error!");
            }

            if (bufferSize < 16)
            {
                bufferSize = 16;
            }
            else
            {
                bufferSize = bufferSize > MAX_BUFFER_SIZE ? MAX_BUFFER_SIZE : bufferSize;
            }

            BufferSize = bufferSize;
            BufferData = new byte[BufferSize];

        }

        /// <summary>
        /// 清除读取数据
        /// </summary>
        public void Clear()
        {
            Position = 0;
            CursorIndex = 0;
        }



    }

}
