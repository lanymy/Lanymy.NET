using Lanymy.Common.ConstKeys;

namespace Lanymy.Common.Instruments
{

    public class CacheModel
    {

        public readonly byte[] Data;

        public int Position { get; set; }


        public CacheModel(int bufferSize = BufferSizeKeys.BUFFER_SIZE_8K)
        {
            Data = new byte[bufferSize];
            Clear();
        }

        public void Clear()
        {

            Position = 0;

        }

    }
}
