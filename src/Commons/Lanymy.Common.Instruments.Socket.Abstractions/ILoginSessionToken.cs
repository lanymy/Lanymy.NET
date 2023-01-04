namespace Lanymy.Common.Instruments
{


    public interface ILoginSessionToken<TSessionTokenID> : ILoginSession
    {

        TSessionTokenID SessionTokenID { get; set; }

    }

}
