using Lanymy.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class ProcessHelperTests
    {
        [TestMethod]
        public void ProcessHelper_StartProcessWithResult_WhenProcessStarts_ShouldCaptureProcessId()
        {
            var result = ProcessHelper.StartProcessWithResult("cmd", true, false, "/c", "echo lanymy-process");

            Assert.IsTrue(result.IsStarted);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.ProcessId.HasValue);
            Assert.IsNull(result.Exception);
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorMessage));
            Assert.IsTrue(result.CreateNoWindow);
            Assert.IsFalse(result.UseShellExecute);
            Assert.IsFalse(result.WaitedForExit);
        }

        [TestMethod]
        public void ProcessHelper_StartProcessWithResult_WhenProcessCannotStart_ShouldCaptureException()
        {
            var result = ProcessHelper.StartProcessWithResult("lanymy_missing_process_123456.exe", true);

            Assert.IsFalse(result.IsStarted);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Exception);
            Assert.AreEqual(result.Exception.Message, result.ErrorMessage);
            Assert.IsFalse(result.ProcessId.HasValue);
            Assert.IsTrue(result.CreateNoWindow);
            Assert.IsFalse(result.WaitedForExit);
        }

        [TestMethod]
        public void ProcessHelper_RunProcessWithResult_WhenExitCodeIsZero_ShouldReportSuccess()
        {
            var result = ProcessHelper.RunProcessWithResult("cmd", true, false, "/c", "exit /b 0");

            Assert.IsTrue(result.IsStarted);
            Assert.IsTrue(result.HasExited);
            Assert.AreEqual(0, result.ExitCode);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.Exception);
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorMessage));
            Assert.IsTrue(result.WaitedForExit);
        }

        [TestMethod]
        public void ProcessHelper_RunProcessWithResult_WhenExitCodeIsNonZero_ShouldReportFailure()
        {
            var result = ProcessHelper.RunProcessWithResult("cmd", true, false, "/c", "exit /b 7");

            Assert.IsTrue(result.IsStarted);
            Assert.IsTrue(result.HasExited);
            Assert.AreEqual(7, result.ExitCode);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Exception);
            Assert.AreEqual("Process exited with code 7.", result.ErrorMessage);
            Assert.IsTrue(result.WaitedForExit);
        }
    }
}
