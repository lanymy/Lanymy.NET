using System.Drawing;

namespace Lanymy.Common.Helpers.SecurityModels
{
    public class EncryptModelBitmapDigestInfoModel<T> : EncryptModelDigestInfoModel<T> where T : class
    {

        public Bitmap EncryptedBitmap { get; set; }


    }
}
