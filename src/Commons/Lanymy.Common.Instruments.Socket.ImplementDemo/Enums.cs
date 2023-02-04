namespace Lanymy.Common.Instruments
{

    public enum FrameTypeEnum : byte
    {
        /// <summary>
        /// 应答
        /// </summary>
        Answer = 0,
        /// <summary>
        /// 请求
        /// </summary>
        Ask = 1,
    }

    public enum CommandTypeEnum : byte
    {

        /// <summary>
        /// 未定义
        /// </summary>
        UnDefine = 0,

        /// <summary>
        /// 心跳
        /// </summary>
        Heart = 1,

        /// <summary>
        /// 功能控制
        /// </summary>
        Control = 2,

    }

    public enum BatteryLevelTypeEnum : byte
    {
        /// <summary>
        /// 无
        /// </summary>
        UnDefine = 0,
        /// <summary>
        /// 低电量
        /// </summary>
        Low = 1,
        /// <summary>
        /// 中电量
        /// </summary>
        Middle = 2,
        /// <summary>
        /// 高电量
        /// </summary>
        High = 3,
    }


    /// <summary>
    /// 选择模式
    /// </summary>
    public enum SelectModeTypeEnum : byte
    {
        /// <summary>
        /// 未定义
        /// </summary>
        UnDefine = 0,
        /// <summary>
        /// 启动模式
        /// </summary>
        Start = 1,
        /// <summary>
        /// 停止模式
        /// </summary>
        Stop = 2,
    }



}
