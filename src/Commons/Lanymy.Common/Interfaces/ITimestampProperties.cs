using System;

namespace Lanymy.Common.Interfaces
{
    /// <summary>
    /// 时间戳属性接口
    /// </summary>
    public interface ITimestampProperties
    {


        /// <summary>
        /// 创建时间属性
        /// </summary>
        /// <value>The create date time.</value>
        DateTime CreateDateTime { get; set; }


        /// <summary>
        /// 最后修改时间属性
        /// </summary>
        /// <value>The last update date time.</value>
        DateTime LastUpdateDateTime { get; set; }


    }
}
