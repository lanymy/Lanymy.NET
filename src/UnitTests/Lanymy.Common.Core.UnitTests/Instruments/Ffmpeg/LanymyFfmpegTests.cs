using System.IO;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.Ffmpeg;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.Core.UnitTests.Instruments.Ffmpeg
{
    [TestClass()]
    public class LanymyFfmpegTests
    {

        [TestMethod()]
        public void LanymyFfmpegTest()
        {

            var ffmpegFileFullPath = Path.Combine(PathHelper.GetCallDomainPath(), @"Resources\Tools\ffmpeg.exe");
            var videoFileFullPath = Path.Combine(PathHelper.GetCallDomainPath(), @"Resources\Others\BigBunny.m4v");

            using (var ffmpeg = new LanymyFfmpeg(ffmpegFileFullPath))
            {
                var videoTotalDuration = ffmpeg.GetVideoTotalDuration(videoFileFullPath);
            }




        }

    }
}