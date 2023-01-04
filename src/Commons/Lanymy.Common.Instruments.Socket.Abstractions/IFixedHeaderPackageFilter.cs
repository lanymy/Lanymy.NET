namespace Lanymy.Common.Instruments
{


    public interface IFixedHeaderPackageFilter<TPackage, TSendPackage, TSessionToken>
        where TPackage : class
        where TSendPackage : class
        where TSessionToken : ISessionToken
    {

        byte HeaderSize { get; }

        int GetBodyLengthFromHeader(int cursorIndex, byte[] bufferBytes);
        bool CheckPackage(byte[] packageBytes);
        byte[] EncodePackage(TSendPackage sendPackage);
        TPackage DecodePackage(byte[] packageBytes);

        byte[] GetPackageBytes(BufferModel buffer, CacheModel cache);

        byte[] GetHeartBytes(ISessionToken sessionToken);


    }


}
