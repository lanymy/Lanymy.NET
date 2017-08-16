/********************************************************************

时间: 2015年04月02日, AM 10:16:34

作者: lanyanmiyu@qq.com

描述: 枚举导航

其它:     

********************************************************************/

namespace Lanymy.General.Extension.Models
{


    /// <summary>
    /// 注册表根项枚举
    /// </summary>
    public enum RegeditRootEnum
    {
        HKEY_CLASSES_ROOT,
        HKEY_CURRENT_USER,
        HKEY_LOCAL_MACHINE,
        HKEY_USERS,
        HKEY_CURRENT_CONFIG,
    }


    /// <summary>
    /// 日志消息类别枚举
    /// </summary>
    public enum LogMessageTypeEnum
    {

        /// <summary>
        /// 测试消息
        /// </summary>
        Debug,

        /// <summary>
        /// 内容消息
        /// </summary>
        Info,

        /// <summary>
        /// 警告消息
        /// </summary>
        Warn,

        /// <summary>
        /// 错误消息
        /// </summary>
        Error,

        /// <summary>
        /// 致命消息
        /// </summary>
        Fatal,


    }


    ///// <summary>
    ///// 消息标识枚举
    ///// </summary>

    //public enum MessageTokenEnum
    //{

    //    /// <summary>
    //    /// 关闭窗体
    //    /// </summary>

    //    CloseWindow = 0,


    //    /// <summary>
    //    /// 显示消息提示框
    //    /// </summary>

    //    ShowCustomMessageBox,


    //    /// <summary>
    //    /// 是否显示遮罩层
    //    /// </summary>
    //    ShowLoadingMask,

    //    /// <summary>
    //    /// 初始化窗体
    //    /// </summary>
    //    InitWindow,


    //}


    /// <summary>
    /// 路径类别枚举
    /// </summary>
    public enum PathTypeEnum
    {
        /// <summary>
        /// 未知路径
        /// </summary>
        UnKnow,
        /// <summary>
        /// 文件路径
        /// </summary>
        File,
        /// <summary>
        /// 文件夹路径
        /// </summary>
        Directory,

    }



    /// <summary>
    /// 日期季度枚举
    /// </summary>
    public enum DateQuarterEnum
    {

        /// <summary>
        /// 第一季度
        /// </summary>
        FirstQuarter = 1,

        /// <summary>
        /// 第二季度
        /// </summary>
        SecondQuarter = 4,

        /// <summary>
        /// 第三季度
        /// </summary>
        ThirdQuarter = 7,

        /// <summary>
        /// 第四季度
        /// </summary>
        FourthQuarter = 10,

    }


    /// <summary>
    /// Windows服务状态
    /// </summary>
    public enum ServiceStatusEnum
    {
        /// <summary>
        /// 未定义
        /// </summary>
        UnDefine,
        /// <summary>
        /// 服务未运行。 这对应于 Win32 SERVICE_STOPPED 常数，该常数定义为 0x00000001。
        /// </summary>
        Stopped = 1,
        /// <summary>
        /// 服务正在启动。 这对应于 Win32 SERVICE_START_PENDING 常数，该常数定义为 0x00000002。
        /// </summary>
        StartPending = 2,
        /// <summary>
        /// 服务正在停止。 这对应于 Win32 SERVICE_STOP_PENDING 常数，该常数定义为 0x00000003。
        /// </summary>
        StopPending = 3,
        /// <summary>
        /// 服务正在运行。 这对应于 Win32 SERVICE_RUNNING 常数，该常数定义为 0x00000004。
        /// </summary>
        Running = 4,
        /// <summary>
        /// 服务即将继续。 这对应于 Win32 SERVICE_CONTINUE_PENDING 常数，该常数定义为 0x00000005。
        /// </summary>
        ContinuePending = 5,
        /// <summary>
        /// 服务即将暂停。 这对应于 Win32 SERVICE_PAUSE_PENDING 常数，该常数定义为 0x00000006。
        /// </summary>
        PausePending = 6,
        /// <summary>
        /// 服务已暂停。 这对应于 Win32 SERVICE_PAUSED 常数，该常数定义为 0x00000007。
        /// </summary>
        Paused = 7,

    }


    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DataBaseTypeEnum
    {

        /// <summary>
        /// 未知
        /// </summary>
        UnDefine = 0,
        /// <summary>
        /// Sql Server Compact Edition
        /// </summary>
        SQLServerCE = 1,
        MySQL = 2,
        Npgsql = 3,
        Oracle = 4,
        SQLite = 5,
        SQLServer = 6,
        Access = 7,
        Excel = 8,

    }


    /// <summary>
    /// 多少位计算模式 x86 / x64
    /// </summary>
    public enum BitOperatingSystemTypeEnum
    {
        /// <summary>
        /// 未定义
        /// </summary>
        UnDefine,
        /// <summary>
        /// 32位
        /// </summary>
        x86,
        /// <summary>
        /// 64位
        /// </summary>
        x64,
        /// <summary>
        /// 未知
        /// </summary>
        UnKnow,
    }


}
