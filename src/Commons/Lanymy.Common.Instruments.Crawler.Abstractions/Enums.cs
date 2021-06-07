using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{

    /// <summary>
    /// 资源类型
    /// </summary>
    public enum ResourceTypeEnum
    {

        /// <summary>
        /// 未定义
        /// </summary>
        UnDefine,

        /// <summary>
        /// 未知
        /// </summary>
        UnKnow,

        /// <summary>
        /// 图片资源
        /// </summary>
        Image,

        /// <summary>
        /// 视频资源
        /// </summary>
        Video,

    }

    public enum ResourceDownloadTypeEnum
    {

        /// <summary>
        /// 未定义
        /// </summary>
        UnDefine,

        /// <summary>
        /// 未知
        /// </summary>
        UnKnow,

        Mp3,

        Video,

        Jpg,

        M3u8,
    }

}
