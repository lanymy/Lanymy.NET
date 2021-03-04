using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{
    public interface ICryptoStreamCore
    {


        //public TEncryptDigestInfoModel EncryptStreamToStream<TEncryptDigestInfoModel>(Stream sourceStream, Stream encryptStream, SecurityEncryptDirectionTypeEnum securityEncryptDirectionType, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
        TEncryptDigestInfoModel EncryptStreamToStream<TEncryptDigestInfoModel>(Stream sourceStream, Stream encryptStream, string secretKey = null, bool ifRandom = true, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptDigestInfoModel, new();

        TEncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedStream<TEncryptDigestInfoModel>(Stream encryptedStream, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptDigestInfoModel, new();
        TEncryptDigestInfoModel DencryptStreamFromStream<TEncryptDigestInfoModel>(Stream encryptedStream, Stream sourceStream, string secretKey = null, Encoding encoding = null)
            where TEncryptDigestInfoModel : EncryptDigestInfoModel, new();

    }
}
