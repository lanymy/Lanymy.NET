using System.Drawing;

namespace Lanymy.Common.Models.SecurityModels
{
    public class EncryptModelBitmapDigestInfoModel<T> : EncryptModelDigestInfoModel<T> where T : class
    {

        public Bitmap EncryptedBitmap { get; set; }


    }
}
