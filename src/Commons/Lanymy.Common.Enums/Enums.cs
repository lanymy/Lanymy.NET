

namespace Lanymy.Common.Enums
{



    /// <summary>
    /// 路径类别枚举
    /// </summary>
    public enum PathTypeEnum
    {

        /// <summary>
        /// 未定义
        /// </summary>
        UnDefine,

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
    /// 数据库类型
    /// </summary>
    public enum DbTypeEnum
    {


        /// <summary>
        /// 未定义
        /// </summary>
        UnDefine,


        /// <summary>
        /// 未知路径
        /// </summary>
        UnKnow,

        /// <summary>
        /// Microsoft SQL Server
        /// </summary>
        SqlServer,

        /// <summary>
        /// MySQL
        /// </summary>
        MySQL,


        /// <summary>
        /// Oracle
        /// </summary>
        Oracle,


        /// <summary>
        /// SQLite
        /// </summary>
        Sqlite,

        /// <summary>
        /// PostgreSQL
        /// </summary>
        PostgreSQL,

        /// <summary>
        /// MongoDB
        /// </summary>
        MongoDB,


    }



    /// <summary>
    /// 多少位计算模式 x86 / x64
    /// </summary>
    public enum BitOperatingTypeEnum
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
        /// 32位
        /// </summary>
        x86,
        /// <summary>
        /// 64位
        /// </summary>
        x64,

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
    /// 哈希散列算法类型
    /// </summary>
    public enum HashAlgorithmTypeEnum
    {

        /// <summary>
        /// 未定义
        /// </summary>
        UnDefine,

        /// <summary>
        /// 未知类型
        /// </summary>
        UnKnown,

        MD5,

        SHA1,

        SHA256,

        SHA384,

        SHA512,


    }



    #region 安全加密方法中 加密方向类型 枚举

    ///// <summary>
    ///// 安全加密方法中 加密类型 枚举
    ///// </summary>
    //public enum SecurityEncryptDirectionTypeEnum
    //{

    //    UnDefine,

    //    BytesToBytes,
    //    BytesToFile,

    //    StringToBytes,
    //    StringToFile,
    //    StringToBase64String,

    //    ModelToBytes,
    //    ModelToBase64String,
    //    ModelToFile,

    //    FileToFile,

    //    BytesToBitmap,
    //    StringToBitmap,
    //    ModelToBitmap,

    //    BytesToImageFile,
    //    StringToImageFile,
    //    ModelToImageFile,


    //}


    #endregion


    /// <summary>
    /// 密钥长度(加密级别) 1024/2048/3072/7680/15360
    /// </summary>
    public enum RsaKeySizeTypeEnum
    {
        V1024 = 1024,
        V2048 = 2048,
        V3072 = 3072,
        V7680 = 7680,
        V15360 = 15360,
    }


}
