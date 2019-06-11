using System.Drawing;

namespace Lanymy.Common.Models.SecurityModels
{
    public class EncryptBitmapDigestInfoModel : EncryptDigestInfoModel
    {
        public byte[] SourceBytes { get; set; }
        public string SourceString { get; set; }

        public Bitmap EncryptedBitmap { get; set; }
    }
}
