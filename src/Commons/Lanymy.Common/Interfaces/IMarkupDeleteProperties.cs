namespace Lanymy.Common.Interfaces
{
    /// <summary>
    /// 标记软删除属性接口
    /// </summary>
    public interface IMarkupDeleteProperties : ITimestampProperties
    {
        /// <summary>
        /// 软删除标记属性
        /// </summary>
        bool IsDelete { get; set; }
    }
}
