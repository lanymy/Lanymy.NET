using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Cmd;

namespace Lanymy.Common.Instruments.Ffmpeg
{


    /// <summary>
    /// ffmpeg 操作器 基类
    /// </summary>
    public abstract class BaseFfmpeg : IDisposable
    {

        /// <summary>
        /// ffmpeg exe 文件全路径
        /// </summary>
        public string FfmpegFileFullPath { get; }

        /// <summary>
        /// 用于执行 ffmpeg 相关命令行的 CMD 控制台 进程
        /// </summary>
        protected LanymyCmd _CmdFfmpeg { get; }

        private Task _Task;

        protected Action<string> OnFfmpegOutputCommand { get; }

        protected BaseFfmpeg(string ffmpegFileFullPath, Action<string> onFfmpegOutputCommand)
        {

            if (!File.Exists(ffmpegFileFullPath))
            {
                throw new FileNotFoundException(nameof(ffmpegFileFullPath), ffmpegFileFullPath);
            }

            FfmpegFileFullPath = ffmpegFileFullPath;
            OnFfmpegOutputCommand = onFfmpegOutputCommand;
            _CmdFfmpeg = new LanymyCmd(OnOutputDataReceivedAction, OnErrorDataReceivedAction);

        }

        private void OnOutputDataReceivedAction(string data)
        {
            OnFfmpegOutputCommandAction(data);
        }

        private void OnErrorDataReceivedAction(string data)
        {
            OnFfmpegOutputCommandAction(data);
        }

        private void OnFfmpegOutputCommandAction(string data)
        {
            OnFfmpegOutputCommand?.Invoke(data);
        }


        public async Task<string> RunFfmpegCmdAsync(params string[] args)
        {
            return await Task.Run(() => RunFfmpegCmd(args));
        }

        public string RunFfmpegCmd(params string[] args)
        {

            if (args.IfIsNullOrEmpty())
            {
                return null;
            }

            return RunFfmpegCmd(string.Join(" ", args));

        }


        public async Task<string> RunFfmpegCmdAsync(string ffmpegCmdString)
        {
            return await Task.Run(() => RunFfmpegCmd(ffmpegCmdString));
        }

        /// <summary>
        /// 命令字符串中 不需要包含 ffmpeg 关键字 直接传入 后面的相关参数命令即可
        /// </summary>
        /// <param name="ffmpegCmdString"></param>
        /// <returns></returns>
        public string RunFfmpegCmd(string ffmpegCmdString)
        {

            var cmdResultModel = _CmdFfmpeg.ExecuteCommand(GetFfmpegCmdFormatString(ffmpegCmdString));

            return cmdResultModel.IsSuccess ? cmdResultModel.GetFullDataString() : cmdResultModel.Exception.ToString();

        }

        /// <summary>
        /// 获取 ffmpeg 完整执行 命令行字符串
        /// </summary>
        /// <param name="ffmpegCmdString"></param>
        /// <returns></returns>
        public string GetFfmpegCmdFormatString(string ffmpegCmdString)
        {
            return string.Format("\"{0}\" {1}", FfmpegFileFullPath, ffmpegCmdString);
        }


        //TimeSpan totalMediaDuration = new TimeSpan();

        public TimeSpan GetVideoTotalDuration(string videoFileFullPath)
        {

            var ts = new TimeSpan();

            var args = new[]{
                "-i",
                string.Format("\"{0}\"",videoFileFullPath),
            };

            var outputString = RunFfmpegCmd(args);

            if (!outputString.IfIsNullOrEmpty())
            {

                const string DURATION_START_TAG = "Duration:";
                const string DURATION_END_TAG = ",";

                string durationStr = outputString;
                durationStr = durationStr.LeftRemoveString(DURATION_START_TAG);
                durationStr = durationStr.LeftSubString(DURATION_END_TAG);
                durationStr = durationStr.Trim();

                if (!durationStr.IfIsNullOrEmpty())
                {
                    TimeSpan.TryParse(durationStr, out ts);
                }

            }

            return ts;

        }


        public virtual void Dispose()
        {

            _CmdFfmpeg?.Dispose();

        }

    }

}
