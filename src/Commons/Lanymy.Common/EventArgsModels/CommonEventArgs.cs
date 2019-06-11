using System;

namespace Lanymy.Common.EventArgsModels
{
    /// <summary>
    /// 通用事件参数实体类
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class CommonEventArgs : EventArgs
    {


        public bool Handled { get; set; } = false;

        public object Source { get; set; }

        public object Data { get; set; }
        public object Other { get; set; }

    }
}
