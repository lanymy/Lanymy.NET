using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Interfaces
{


    public interface ICrypto :
        ICryptoStream, ICryptoStreamCore,
        ICryptoBytes, ICryptoBytesCore,
        ICryptoString, ICryptoStringCore,
        ICryptoFile, ICryptoFileCore,
        ICryptoBitmap, ICryptoBitmapCore,
        ICryptoModel
    {



    }


}
