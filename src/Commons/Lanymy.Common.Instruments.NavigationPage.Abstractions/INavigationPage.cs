using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.Common.Instruments
{

    public interface INavigationPage
    {

        /// <summary>
        /// 页码索引值 0为第一页
        /// </summary>
        uint PageIndex { get; }

        /// <summary>
        /// 每页多少个
        /// </summary>
        uint PageSize { get; set; }

        /// <summary>
        /// 一共多少页
        /// </summary>
        uint TotalPageCount { get; }

        /// <summary>
        /// 总数量 是多少个
        /// </summary>
        uint TotalCount { get; set; }


        /// <summary>
        /// 能否首页
        /// </summary>
        bool CanFirstPage { get; }


        /// <summary>
        /// 能否上一页
        /// </summary>
        bool CanPrevPage { get; }

        /// <summary>
        /// 能否下一页
        /// </summary>
        bool CanNextPage { get; }


        /// <summary>
        /// 能否最后一页
        /// </summary>
        bool CanLastPage { get; }



    }

}
