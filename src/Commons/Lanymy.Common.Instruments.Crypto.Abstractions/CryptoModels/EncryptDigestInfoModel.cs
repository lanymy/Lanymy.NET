using System;

namespace Lanymy.Common.Instruments.CryptoModels
{
    /// <summary>
    /// 加密 摘要信息 基类 实体类
    /// </summary>
    public class EncryptDigestInfoModel
    {

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        //public SecurityEncryptDirectionTypeEnum SecurityEncryptDirectionType { get; set; }

        /// <summary>
        /// 是否 随机种子 加密
        /// </summary>
        public bool IsRandomEncrypt { get; set; }

        /// <summary>
        /// 原始大小
        /// </summary>
        public long SourceBytesSize { get; set; }

        /// <summary>
        /// 加密后大小
        /// </summary>
        public long EncryptBytesSize { get; set; }
        /// <summary>
        /// 加密后 正文大小
        /// </summary>
        public long EncryptContentBytesSize { get; set; }

        /// <summary>
        /// 原始 二进制数据 哈希值
        /// </summary>
        public string SourceBytesHashCode { get; set; }

        /// <summary>
        /// 加密后 二进制数据 哈希值
        /// </summary>
        public string EncryptBytesHashCode { get; set; }
        /// <summary>
        /// 加密后 正文 哈希值
        /// </summary>
        public string EncryptContentBytesHashCode { get; set; }

        public string DencryptHeaderInfoModelJsonString { get; set; }

        //public object ResultData { get; set; }

        public object OtherData { get; set; }

        ///// <summary>
        ///// 头摘要信息二进制数据长度
        ///// </summary>
        //internal int HeaderInfoLength { get; set; }
        ///// <summary>
        ///// 在数据流中 头摘要信息二进制数据 结束位置 索引值
        ///// </summary>
        //internal long HeaderInfoEndPosition { get; set; }

        /// <summary>
        /// 时间戳 只有在随机加密情况下 时间戳才有值
        /// </summary>
        public DateTime? CreateDateTime { get; set; }

    }
}
