using System.Diagnostics;
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
            var othersDirectoryFullPath = Path.Combine(PathHelper.GetCallDomainPath(), @"Resources\Others\");

            using (var ffmpeg = new LanymyFfmpeg(ffmpegFileFullPath, OnFfmpegOutputCommandAction))
            {
                var videoTotalDuration = ffmpeg.GetVideoTotalDuration(videoFileFullPath);
            }

            string m3u8HttpFullPath = "https://node.imgio.in/demo/birds.m3u8";

            string m3u8SaveFileFullPath = Path.Combine(othersDirectoryFullPath, "birds.mp4");

            using (var ffmpeg = new LanymyFfmpeg(ffmpegFileFullPath, OnFfmpegOutputCommandAction))
            {

                var ffmpegCmdOutputString = ffmpeg.SaveM3u8ToMp4File(m3u8HttpFullPath, m3u8SaveFileFullPath);

                string str = string.Empty;

            }

            using (var ffmpeg = new LanymyFfmpeg(ffmpegFileFullPath, OnFfmpegOutputCommandAction))
            {

                var ffmpegCmdOutputString = ffmpeg.SaveM3u8ToMp4FileAsync(m3u8HttpFullPath, m3u8SaveFileFullPath).Result;

                string str = string.Empty;

            }

        }

        private void OnFfmpegOutputCommandAction(string data)
        {
            Debug.WriteLine(data);
        }

    }
}