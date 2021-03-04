using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Interfaces
{
    public interface ICryptoFileProperty
    {
        /// <summary>
        /// 原文件全名称
        /// </summary>
        string SourceFileFullPath { get; set; }

        /// <summary>
        /// 加密后文件全名称
        /// </summary>
        string EncryptedFileFullPath { get; set; }

    }
}
