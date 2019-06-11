namespace Lanymy.Common.Models
{
    /// <summary>
    /// IO Path 路径信息 实体类
    /// </summary>
    public class PathInfoModel
    {
        /// <summary>
        /// 是否 是 路径
        /// </summary>
        public bool IsPath { get; set; } = false;
        /// <summary>
        /// 路径中文件名称(不包括后缀名) 如 "c:\a\a.txt" 返回 "a"
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 路径中文件后缀名(不包括文件名称) 如 "c:\a\a.txt" 返回 ".txt"
        /// </summary>
        public string FileSuffix { get; set; }
        /// <summary>
        /// 路径中文件全名称(文件名+后缀名) 如 "c:\a\a.txt" 返回 "a.txt"
        /// </summary>
        public string FileFullName => FileName + FileSuffix;
        /// <summary>
        /// 路径中文件的文件夹 全路径 如 "c:\a\a.txt" 返回 "c:\a"
        /// </summary>
        public string PathWithoutFileFullName { get; set; }
    }
}
