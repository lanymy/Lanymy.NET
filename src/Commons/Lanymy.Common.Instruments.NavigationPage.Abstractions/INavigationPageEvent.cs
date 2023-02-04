using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.Common.Instruments
{

    public interface INavigationPageEvent
    {


        /// <summary>
        /// 刷新翻页信息,并跳转到首页
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        void RefreshPageInfo(uint pageSize, uint totalCount);


        void GoToFirstPage();



        void GoToLastPage();




        void GoToPage(uint pageIndex);



        void GoToPrevPage();


        void GoToNextPage();


    }

}
