using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.Interfaces;

namespace Lanymy.Common.Instruments.CryptoModels
{



    public class EncryptStringImageFileDigestInfoModel : EncryptStringBitmapDigestInfoModel, ICryptoFileProperty
    {

        /// <summary>
        /// 原文件全名称
        /// </summary>
        public string SourceFileFullPath { get; set; }

        /// <summary>
        /// 加密后文件全名称
        /// </summary>
        public string EncryptedFileFullPath { get; set; }

    }


}
