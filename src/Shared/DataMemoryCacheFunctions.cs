/********************************************************************

时间: 2017年08月16日, PM 03:44:18

作者: lanyanmiyu@qq.com

描述: 内存模式 数据缓存 辅助类 

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using Lanymy.General.Extension.Instruments;
using Lanymy.General.Extension.Interfaces;

namespace Lanymy.General.Extension
{


    /// <summary>
    /// 内存模式 数据缓存 辅助类
    /// </summary>
    public class DataMemoryCacheFunctions
    {
        #region 单例

        private static IDataMemoryCache _DataCache = null;

        private static readonly object SynObject = new object();

        /// <summary>
        /// 获取服务器通信数据中心对象
        /// </summary>
        public static IDataMemoryCache Instance()
        {

            if (null == _DataCache)
            {
                lock (SynObject)
                {
                    if (null == _DataCache)
                    {
                        _DataCache = new DataMemoryCache();
                    }
                }
            }

            return _DataCache;
        }


        #endregion
    }



}
