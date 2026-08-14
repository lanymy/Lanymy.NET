using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Interfaces
{
    /// <summary>
    /// 描述加密结果里与模型元数据相关的公共属性。
    /// </summary>
    public interface ICryptoModelProperty<T>
        where T : class
    {
        /// <summary>
        /// 模型类型短名称。
        /// </summary>
        string ModelTypeName { get; set; }

        /// <summary>
        /// 模型类型完整名称。
        /// </summary>
        string ModelTypeFullName { get; set; }

        /// <summary>
        /// 解密或加密前后的源模型对象。
        /// </summary>
        T SourceModel { get; set; }
    }
}
