using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Ffmpeg
{


    /// <summary>
    /// ffmpeg 操作器
    /// </summary>
    public class LanymyFfmpeg : BaseFfmpeg
    {

        public LanymyFfmpeg(string ffmpegFileFullPath, Action<string> onFfmpegOutputCommand = null) : base(ffmpegFileFullPath, onFfmpegOutputCommand)
        {

        }

        /// <summary>
        /// m3u8 转码成 mp4 文件
        /// </summary>
        /// <param name="m3u8FileFullPath">支持 http 全路径 和 带盘符系统 全路径 如: http://www.abc.com/1.m3u8 或 c:\1.m3u8</param>
        /// <param name="saveFileFullPath">保存到本地的 mp4 文件系统全路径 如: c:\1.mp4</param>
        /// <returns></returns>
        public string SaveM3u8ToMp4File(string m3u8FileFullPath, string saveFileFullPath)
        {

            const string FORMAT_STRING = "-threads 0 -i \"{0}\" -c copy -y -bsf:a aac_adtstoasc -movflags +faststart \"{1}\"";

            return RunFfmpegCmd(string.Format(FORMAT_STRING, m3u8FileFullPath, saveFileFullPath));

        }


        public async Task<string> SaveM3u8ToMp4FileAsync(string m3u8FileFullPath, string saveFileFullPath)
        {
            return await Task.Run(() => SaveM3u8ToMp4File(m3u8FileFullPath, saveFileFullPath));
        }

    }

}
