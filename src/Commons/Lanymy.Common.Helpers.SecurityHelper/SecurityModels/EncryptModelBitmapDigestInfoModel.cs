using System.Drawing;

namespace Xb.Common.Helpers.SecurityModels
{
    public class EncryptModelBitmapDigestInfoModel<T> : EncryptModelDigestInfoModel<T> where T : class
    {

        public Bitmap EncryptedBitmap { get; set; }


    }
}
