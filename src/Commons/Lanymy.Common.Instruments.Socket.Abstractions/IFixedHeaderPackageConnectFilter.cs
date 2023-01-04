namespace Lanymy.Common.Instruments
{

    public interface IFixedHeaderPackageConnectFilter<TSessionToken>
        where TSessionToken : ILoginSession
    {
        byte[] GetConnectBytes(ILoginSession sessionToken);
    }


}
