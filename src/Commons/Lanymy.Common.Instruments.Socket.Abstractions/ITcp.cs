using System;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{


    public interface ITcp : IBaseTcp, IDisposable
    {

        void Close();

        Task CloseAsync();

    }

}
