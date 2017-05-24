/********************************************************************

时间: 2017年05月24日, PM 06:08:57

作者: lanyanmiyu@qq.com

描述: 压缩器接口

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.General.Extension.Interfaces
{


    /// <summary>
    /// 压缩器接口
    /// </summary>
    public interface ICompresser: ICompresserBytesAndBytes, ICompresserBytesAndBase64String, ICompresserStringAndBytes, ICompresserStringAndBase64String
    {




    }


}
