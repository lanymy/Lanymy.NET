using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Enums;
using Lanymy.Common.Instruments.CryptoModels;

namespace Lanymy.Common.Instruments.Interfaces
{


    /// <summary>
    /// 数据流加密/解密
    /// </summary>
    public interface ICryptoStream
    {


        EncryptDigestInfoModel EncryptStreamToStream(Stream sourceStream, Stream encryptStream, string secretKey = null, bool ifRandom = true, Encoding encoding = null);



        EncryptDigestInfoModel GetEncryptDigestInfoModelFromEncryptedStream(Stream encryptedStream, Encoding encoding = null);





        EncryptDigestInfoModel DencryptStreamFromStream(Stream encryptedStream, Stream sourceStream, string secretKey = null, Encoding encoding = null);





    }


}
