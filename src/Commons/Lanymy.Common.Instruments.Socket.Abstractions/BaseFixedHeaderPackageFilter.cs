using System;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{

    public abstract class BaseFixedHeaderPackageFilter<TPackage, TSendPackage, TSessionToken> : IFixedHeaderPackageFilter<TPackage, TSendPackage, TSessionToken>
        where TPackage : class
        where TSendPackage : class
        where TSessionToken : ISessionToken
    {

        public byte HeaderSize { get; }

        protected BaseFixedHeaderPackageFilter(byte headerSize)
        {
            HeaderSize = headerSize;
        }


        public abstract int GetBodyLengthFromHeader(int cursorIndex, byte[] bufferBytes);

        public abstract bool CheckPackage(byte[] packageBytes);

        public abstract byte[] EncodePackage(TSendPackage sendPackage);

        public abstract TPackage DecodePackage(byte[] packageBytes);


        public virtual byte[] GetPackageBytes(BufferModel buffer, CacheModel cache)
        {

            byte[] dataBytes = null;
            var packageLength = 0;


            if (cache.Position > 0)
            {

                var position = cache.Position + buffer.Position;

                var dataBytesTemp = new byte[position];

                Array.Copy(cache.Data, 0, dataBytesTemp, 0, cache.Position);
                Array.Copy(buffer.BufferData, 0, dataBytesTemp, cache.Position, buffer.Position);


                packageLength = HeaderSize + GetBodyLengthFromHeader(0, dataBytesTemp);

                dataBytes = new byte[packageLength];

                Array.Copy(dataBytesTemp, dataBytes, packageLength);

                buffer.CursorIndex = packageLength - cache.Position;

                cache.Clear();


            }
            else
            {

                if (buffer.CursorIndex >= buffer.Position)
                {
                    buffer.Clear();
                }
                else
                {

                    if (HeaderSize > buffer.Position - buffer.CursorIndex)
                    {
                        packageLength = buffer.Position + 1;
                    }
                    else
                    {
                        packageLength = HeaderSize + GetBodyLengthFromHeader(buffer.CursorIndex, buffer.BufferData);
                    }


                    if (buffer.CursorIndex + packageLength > buffer.Position)//Õ³°ü
                    {
                        cache.Position = buffer.Position - buffer.CursorIndex;
                        Array.Copy(buffer.BufferData, buffer.CursorIndex, cache.Data, 0, cache.Position);
                        buffer.Clear();
                    }
                    else
                    {
                        dataBytes = new byte[packageLength];
                        Array.Copy(buffer.BufferData, buffer.CursorIndex, dataBytes, 0, packageLength);
                        buffer.CursorIndex += packageLength;

                    }
                }

            }

            return dataBytes;

        }

        public abstract byte[] GetHeartBytes(ISessionToken sessionToken);


    }

}
