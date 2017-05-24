/********************************************************************

时间: 2016年11月21日, PM 02:00:23

作者: lanyanmiyu@qq.com

描述: 压缩包内 文件信息 实体类

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.General.Extension.Models
{


    /// <summary>
    /// 压缩包内 文件信息 实体类
    /// </summary>
    public class ZipFileDigestInfoModel
    {

        /// <summary>
        /// 名称
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// 是否是文件夹
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// 子对象
        /// </summary>
        public List<ZipFileDigestInfoModel> Children { get; set; }

        /// <summary>
        /// 压缩包内 文件信息 实体类 构造方法
        /// </summary>
        public ZipFileDigestInfoModel()
        {
            Children = new List<ZipFileDigestInfoModel>();
        }

    }


}
