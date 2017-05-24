/********************************************************************

时间: 2016年08月07日, PM 01:56:47

作者: lanyanmiyu@qq.com

描述: 通信数据实体类

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension;

namespace Lanymy.General.Extension.Models
{

    /// <summary>
    /// 通信数据实体类
    /// </summary>
    public class CommunicationDataModel
    {

        /// <summary>
        /// 数据类型
        /// </summary>
        public string JsonDataType { get; set; }

        /// <summary>
        /// 传输的数据
        /// </summary>
        public string JsonData { get; set; }

        /// <summary>
        /// 请求是否成功
        /// </summary>
        public bool IfIsSucces { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        ///// <summary>
        ///// 其它 预留扩展属性
        ///// <para>CodeState 状态 为  CodeStateEnum.ServiceError 服务执行失败时</para>
        ///// <para>Other 存的是 此次服务 执行失败 的报错信息</para>
        ///// </summary>
        //public object Other { get; set; }


        /// <summary>
        /// 获取默认失败通信实体类
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static CommunicationDataModel GetCommunicationDataModel(string errorMessage = "未知错误")
        {
            return new CommunicationDataModel { IfIsSucces = false, ErrorMessage = errorMessage };
        }


        /// <summary>
        /// 获取默认失败通信实体类
        /// </summary>
        /// <typeparam name="TJsonDataType"></typeparam>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static CommunicationDataModel GetCommunicationDataModel<TJsonDataType>(string errorMessage = "未知错误")
        {
            return new CommunicationDataModel { IfIsSucces = false, ErrorMessage = errorMessage, JsonDataType = typeof(TJsonDataType).FullName };
        }


    }
}
