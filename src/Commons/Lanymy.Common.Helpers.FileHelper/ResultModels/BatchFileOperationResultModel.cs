using System;
using System.Collections.Generic;
using System.Linq;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// 批量文件操作结果。
    /// </summary>
    public class BatchFileOperationResultModel
    {
        /// <summary>
        /// 本次请求的操作项数量。
        /// </summary>
        public int RequestedItemCount { get; set; }

        /// <summary>
        /// 批次级异常。
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 单项操作结果集合。
        /// </summary>
        public IReadOnlyList<FileOperationResultModel> Results { get; set; } = new List<FileOperationResultModel>();

        /// <summary>
        /// 是否全部成功。
        /// </summary>
        public bool IsSuccess => Exception == null
                                 && RequestedItemCount > 0
                                 && Results != null
                                 && RequestedItemCount == Results.Count
                                 && Results.All(item => item != null && item.IsSuccess);

        /// <summary>
        /// 成功数量。
        /// </summary>
        public int SuccessCount => Results?.Count(item => item != null && item.IsSuccess) ?? 0;

        /// <summary>
        /// 失败数量。
        /// </summary>
        public int FailureCount => Results?.Count(item => item == null || !item.IsSuccess) ?? 0;

        /// <summary>
        /// 首个可用异常。
        /// </summary>
        public Exception FirstException => Exception ?? Results?.Select(item => item?.Exception).FirstOrDefault(item => item != null);
    }
}
