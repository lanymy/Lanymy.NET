using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Interfaces
{
    /// <summary>
    /// 聚合所有加密能力的总接口，覆盖流、字节、字符串、文件、位图和模型对象场景。
    /// </summary>
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
