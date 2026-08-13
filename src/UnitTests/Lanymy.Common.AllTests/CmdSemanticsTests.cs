using System;
using System.IO;
using Lanymy.Common.Instruments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class CmdSemanticsTests
    {
        [TestMethod]
        public void LanymyCmd_ExecuteCommand_WhenExitCodeIsZero_ShouldReportSuccessAndCaptureExitCode()
        {
            using var cmd = new LanymyCmd();

            var result = cmd.ExecuteCommand("echo lanymy-success");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, result.ExitCode);
            StringAssert.Contains(result.OutputDataString ?? string.Empty, "lanymy-success");
        }

        [TestMethod]
        public void LanymyCmd_ExecuteCommand_WhenExitCodeIsNonZero_ShouldReportFailureAndCaptureExitCode()
        {
            using var cmd = new LanymyCmd();

            var result = cmd.ExecuteCommand("echo lanymy-failed 1>&2 & exit /b 7");

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(7, result.ExitCode);
            Assert.IsNull(result.Exception);
            StringAssert.Contains(result.ErrorDataString ?? string.Empty, "lanymy-failed");
        }

        [TestMethod]
        public void LanymyFfmpeg_RunFfmpegCmd_WhenCommandExitsNonZeroWithoutException_ShouldReturnFullOutput()
        {
            var tempScriptPath = Path.Combine(Path.GetTempPath(), $"lanymy_ffmpeg_{Guid.NewGuid():N}.cmd");

            try
            {
                File.WriteAllText(tempScriptPath, "@echo off\r\necho ffmpeg-stdout\r\necho ffmpeg-stderr 1>&2\r\nexit /b 7\r\n");

                using var ffmpeg = new LanymyFfmpeg(tempScriptPath);

                var result = ffmpeg.RunFfmpegCmd("-version");

                StringAssert.Contains(result ?? string.Empty, "ffmpeg-stdout");
                StringAssert.Contains(result ?? string.Empty, "ffmpeg-stderr");
            }
            finally
            {
                if (File.Exists(tempScriptPath))
                {
                    File.Delete(tempScriptPath);
                }
            }
        }
    }
}
