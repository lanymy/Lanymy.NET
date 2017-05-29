/********************************************************************

时间: 2016年05月19日, PM 02:47:10

作者: lanyanmiyu@qq.com

描述: CSV 序列化 / 反序列化 接口

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.General.Extension.Interfaces
{

    /// <summary>
    /// CSV 序列化 / 反序列化 功能 接口
    /// </summary>
    public interface ICsvSerializer<TModel> : ICsvModelSerializer<TModel>, ICsvFileSerializer<TModel> where TModel : class, new()
    {

       



    }


}
