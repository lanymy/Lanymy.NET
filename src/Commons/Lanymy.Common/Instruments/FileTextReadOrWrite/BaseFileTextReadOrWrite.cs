using System;
using System.IO;
using System.Text;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments.FileTextReadOrWrite
{
    /// <summary>
    /// 文件 文本 读写 基类
    /// </summary>
    public abstract class BaseFileTextReadOrWrite : IDisposable
    {


        /// <summary>
        /// 文本文件全路径
        /// </summary>
        public string TextFileFullPath { get; }

        /// <summary>
        /// 编码
        /// </summary>
        public Encoding CurrentEncoding { get; } = DefaultSettingKeys.DEFAULT_ENCODING;

        /// <summary>
        /// 文件 文本 读写 基类 构造方法
        /// </summary>
        /// <param name="textFileFullPath">文件全路径</param>
        /// <param name="ifOverWriteFile">True 覆盖旧文件 创建新文件 ; False 在旧文件上 追加 内容; 默认值 False 旧文件追加内容</param>
        /// <param name="encoding">编码 null 则使用默认编码</param>
        protected BaseFileTextReadOrWrite(string textFileFullPath, bool ifOverWriteFile = false, Encoding encoding = null)
        {

            TextFileFullPath = textFileFullPath;

            PathHelper.InitDirectoryPath(TextFileFullPath);

            if (!encoding.IfIsNullOrEmpty())
            {
                CurrentEncoding = encoding;
            }

            if (ifOverWriteFile)
            {
                if (File.Exists(TextFileFullPath))
                {
                    File.Delete(TextFileFullPath);
                }
            }

            if (!File.Exists(TextFileFullPath))
            {
                File.Create(TextFileFullPath).Dispose();
            }

        }


        /// <summary>
        /// 资源释放
        /// </summary>
        public abstract void Dispose();

    }
}
