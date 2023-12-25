using SkiaSharp.QrCode.Image;
using SkiaSharp;
using System;
using System.Drawing;
using System.IO;
using SkiaSharp.QrCode;

namespace Lanymy.Common.Helpers
{


    /// <summary>
    /// 二维码辅助类
    /// </summary>
    public class SkiaSharpQrCodeHelper
    {



        //        /// <summary>
        //        /// 创建二维码图片
        //        /// </summary>
        //        /// <param name="message">消息内容</param>
        //        /// <param name="eccLevel">容错级别</param>
        //        /// <param name="pixelsPerModule">平均像素点</param>
        //        /// <param name="darkColorHtmlHex">暗色值 默认值 #000000</param>
        //        /// <param name="lightColorHtmlHex">亮色值 默认值 #FFFFFF</param>
        //        /// <param name="iconFileFullPath">二维码中间logo文件全路径</param>
        //        /// <param name="iconSizePercent">二维码中间logo显示尺寸</param>
        //        /// <param name="iconBorderWidth">二维码中间logo边框</param>
        //        /// <param name="drawQuietZones"></param>
        //        /// <returns></returns>
        //        public static Bitmap CreateQrCodeBitmap(string message, QRCodeGenerator.ECCLevel eccLevel = QRCodeGenerator.ECCLevel.L, int pixelsPerModule = 20, string darkColorHtmlHex = "#000000", string lightColorHtmlHex = "#FFFFFF", string iconFileFullPath = null, int iconSizePercent = 15, int iconBorderWidth = 6, bool drawQuietZones = true)
        //        {


        //#if NET7_0_OR_GREATER

        //            throw new NotSupportedException("QRCoder-1.4.3 不支持.net7.");
        //#else

        //            Bitmap qrCodeBitmap;

        //            var darkColor = ColorTranslator.FromHtml(darkColorHtmlHex);
        //            var lightColor = ColorTranslator.FromHtml(lightColorHtmlHex);
        //            var iconBitmap = new Bitmap(iconFileFullPath);

        //            using (var qrGenerator = new QRCodeGenerator())
        //            {
        //                using (var qrCodeData = qrGenerator.CreateQrCode(message, eccLevel))
        //                {
        //                    using (var qrCode = new QRCode(qrCodeData))
        //                    {
        //                        qrCodeBitmap = qrCode.GetGraphic(pixelsPerModule, darkColor, lightColor, iconBitmap, iconSizePercent, iconBorderWidth, drawQuietZones);
        //                    }
        //                }
        //            }

        //            return qrCodeBitmap;

        //#endif

        //        }




        public static void CreateQrCode(string qrCodeImageFileFullPath, string content, int sizeX = 512, int sizeY = 512, SKEncodedImageFormat outputFormat = SKEncodedImageFormat.Png, int quality = 100, ECCLevel eccLevel = ECCLevel.L)
        {


            var qrCode = new QrCode(content, new Vector2Slim(sizeX, sizeY), outputFormat, quality);
            using (var output = new FileStream(qrCodeImageFileFullPath, FileMode.OpenOrCreate))
            {
                qrCode.GenerateImage(output, true, eccLevel);
            }


        }


    }
}
