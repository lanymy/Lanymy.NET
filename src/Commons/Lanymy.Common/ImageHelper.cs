using System.Drawing;
using System.Drawing.Imaging;

namespace Lanymy.Common
{
    /// <summary>
    /// 图片 相关 辅助类
    /// </summary>
    public class ImageHelper
    {

        /// <summary>
        /// Saves the bitmap to image file.
        /// </summary>
        /// <param name="encryptImage">The encrypt image.</param>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveBitmapToImageFile(Bitmap encryptImage, string imageFileFullPath)
        {
            bool result;
            try
            {
                using (var image = encryptImage)
                {
                    image.Save(imageFileFullPath, ImageFormat.Png);
                }
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;

        }

        /// <summary>
        /// Gets the bitmap from image file.
        /// </summary>
        /// <param name="imageFileFullPath">The image file full path.</param>
        /// <returns>Bitmap.</returns>
        public static Bitmap GetBitmapFromImageFile(string imageFileFullPath)
        {
            return (Bitmap)Bitmap.FromFile(imageFileFullPath);
        }
    }
}
