namespace Lanymy.Common.Abstractions.Interfaces
{

    /// <summary>
    /// 删除属性接口
    /// </summary>
    public interface IIsDeleteProperties
    {
        /// <summary>
        /// 软删除标记属性
        /// </summary>
        bool IsDelete { get; set; }
    }
}
