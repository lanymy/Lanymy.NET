namespace AutoCopyReleaseDll.Models
{

    public class ConsoleArgumentModel
    {


        /// <summary>
        /// 工程编译生成的文件全名称
        /// </summary>
        public string ProjectFileFullName { get; set; }
        /// <summary>
        /// 工程编译生成目标目录全路径
        /// </summary>
        public string TargetDirFullPath { get; set; }
        /// <summary>
        /// 要自动发布到指定目录的全路径
        /// </summary>
        public string AutoCopyDirFullPath { get; set; }

        /// <summary>
        /// 忽略处理文件夹的名称 多个用 ';' 符号分割
        /// </summary>
        public string IgnoreFolderNames { get; set; }

        /// <summary>
        /// 0关闭调试模式;1开启调试模式
        /// </summary>
        public bool IsDebugModel { get; set; }


    }

}
