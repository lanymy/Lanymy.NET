using System.Drawing;

namespace Xb.Common.Helpers.SecurityModels
{
    public class EncryptBitmapDigestInfoModel : EncryptDigestInfoModel
    {
        public byte[] SourceBytes { get; set; }
        public string SourceString { get; set; }

        public Bitmap EncryptedBitmap { get; set; }
    }
}
