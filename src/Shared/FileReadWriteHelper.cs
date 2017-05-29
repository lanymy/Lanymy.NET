/********************************************************************

时间: 2015年10月22日, PM 01:47:58

作者: lanyanmiyu@qq.com

描述: 文件读写辅助类

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.ExtensionFunctions;

namespace Lanymy.General.Extension
{

    /// <summary>
    /// 文件读写辅助类
    /// </summary>
    public class FileReadWriteHelper : IDisposable
    {

        /// <summary>
        /// 文件全路径
        /// </summary>
        public string _FileFullPath;

        /// <summary>
        /// 文件全路径
        /// </summary>
        public string FileFullPath
        {
            get { return _FileFullPath; }
        }


        ///// <summary>
        ///// 基础数据流
        ///// </summary>
        //private FileStream _FileStream;


        /// <summary>
        /// 写数据流对象
        /// </summary>
        private StreamWriter _Writer;


        /// <summary>
        /// 读数据流对象
        /// </summary>
        private StreamReader _Reader;

        /// <summary>
        /// 编码
        /// </summary>
        private Encoding _DefaultEncoding = GlobalSettings.DEFAULT_ENCODING;


        /// <summary>
        /// 当前数据流 读写 状态值 True 读状态 False 写状态
        /// </summary>
        public bool IfIsRead
        {
            get { return !_Reader.IfIsNullOrEmpty(); }
        }


        public void Dispose()
        {

            if (!_Writer.IfIsNullOrEmpty())
            {
                _Writer.Dispose();
                _Writer = null;
            }

            if (!_Reader.IfIsNullOrEmpty())
            {
                _Reader.Dispose();
                _Reader = null;
            }

        }

        /// <summary>
        /// 日志文件全路径
        /// </summary>
        /// <param name="fileFullPath">文件全路径</param>
        /// <param name="ifOverWriteFileInWriteFileModel">True 覆盖旧文件 创建新文件 ; False 在旧文件上 追加 内容; 默认值 False 旧文件追加内容</param>
        public FileReadWriteHelper(string fileFullPath, bool ifOverWriteFileInWriteFileModel = false)
        {

            _FileFullPath = fileFullPath;

            PathFunctions.InitDirectoryPath(_FileFullPath);

            if (ifOverWriteFileInWriteFileModel)
            {
                if (File.Exists(_FileFullPath))
                {
                    File.Delete(_FileFullPath);
                }
            }

            if (!File.Exists(_FileFullPath))
            {
                File.Create(_FileFullPath).Dispose();
            }

        }



        private void CheckWriter()
        {
            if (!_Reader.IfIsNullOrEmpty())
            {
                _Reader.Dispose();
                _Reader = null;
            }

            if (_Writer.IfIsNullOrEmpty())
            {
                _Writer = new StreamWriter(_FileFullPath, true, _DefaultEncoding);
            }
        }


        private void CheckReader()
        {
            if (!_Writer.IfIsNullOrEmpty())
            {
                _Writer.Dispose();
                _Writer = null;
            }

            if (_Reader.IfIsNullOrEmpty())
            {
                _Reader = new StreamReader(_FileFullPath, _DefaultEncoding);
            }
        }


        /// <summary>
        /// 写消息
        /// </summary>
        /// <param name="message"></param>
        public void Write(string message)
        {

            CheckWriter();

            _Writer.Write(message);
            _Writer.Flush();

        }

        /// <summary>
        /// 写一行消息
        /// </summary>
        /// <param name="message"></param>
        public void WriteLine(string message)
        {
            Write(message + Environment.NewLine);
        }


        /// <summary>
        /// 读取全部消息
        /// </summary>
        /// <returns></returns>
        public string ReadAll()
        {
            CheckReader();

            return _Reader.ReadToEnd();
        }



        /// <summary>
        /// 一行 一行 读取数据
        /// </summary>
        public string ReadLine()
        {

            CheckReader();

            return _Reader.ReadLine();

        }


        /// <summary>
        /// 判断是否还有字符串
        /// </summary>
        /// <returns></returns>
        public bool IfHaveString()
        {

            CheckReader();

            return _Reader.Peek() != -1;

        }


    }


}
