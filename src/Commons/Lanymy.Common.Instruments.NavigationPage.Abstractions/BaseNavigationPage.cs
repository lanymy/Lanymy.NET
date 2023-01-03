using System;
using System.Collections.Generic;
using System.Text;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{
    public abstract class BaseNavigationPage : INavigationPage
    {

        /// <summary>
        /// 页码索引值 0为第一页
        /// </summary>
        public uint PageIndex { get; private set; }

        /// <summary>
        /// 每页多少个
        /// </summary>
        public uint PageSize { get; set; }

        /// <summary>
        /// 一共多少页
        /// </summary>
        public uint TotalPageCount { get; private set; }

        /// <summary>
        /// 总数量 是多少个
        /// </summary>
        public uint TotalCount { get; set; }


        /// <summary>
        /// 能否首页
        /// </summary>
        public bool CanFirstPage => CanPrevPage;


        /// <summary>
        /// 能否上一页
        /// </summary>
        public bool CanPrevPage => PageIndex > 0;

        /// <summary>
        /// 能否下一页
        /// </summary>
        public bool CanNextPage => PageIndex + 1 < TotalPageCount;


        /// <summary>
        /// 能否最后一页
        /// </summary>
        public bool CanLastPage => CanNextPage;


        protected readonly Action<INavigationPage> _CurrentPageingAction;


        protected BaseNavigationPage(Action<INavigationPage> pageingAction)
        {

            if (pageingAction.IfIsNull())
            {
                throw new ArgumentNullException(nameof(pageingAction));
            }

            _CurrentPageingAction = pageingAction;

        }


        protected virtual void OnRefreshPageInfo(uint pageSize, uint totalCount)
        {
            if (totalCount == 0)
            {
                pageSize = 1;
                totalCount = 1;
            }

            PageSize = pageSize;
            TotalCount = totalCount;

            TotalPageCount = (uint)Math.Ceiling(TotalCount / (double)PageSize);

            GoToFirstPage();
        }

        /// <summary>
        /// 刷新翻页信息,并跳转到首页
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        public void RefreshPageInfo(uint pageSize, uint totalCount)
        {

            OnRefreshPageInfo(pageSize, totalCount);

        }


        protected virtual void OnGoToFirstPage()
        {
            GoToPage(0);
        }

        public void GoToFirstPage()
        {
            OnGoToFirstPage();
        }


        protected virtual void OnGoToLastPage()
        {
            GoToPage(TotalPageCount - 1);
        }

        public void GoToLastPage()
        {
            OnGoToLastPage();
        }


        protected virtual void OnGoToPage(uint pageIndex)
        {
            if (pageIndex < TotalPageCount)
            {
                PageIndex = pageIndex;
                _CurrentPageingAction?.Invoke(this);
            }
        }

        public void GoToPage(uint pageIndex)
        {

            OnGoToPage(pageIndex);

        }



        protected virtual void OnGoToPrevPage()
        {
            if (CanPrevPage)
            {
                GoToPage(PageIndex - 1);
            }
        }

        public void GoToPrevPage()
        {
            OnGoToPrevPage();
        }


        protected virtual void OnGoToNextPage()
        {
            if (CanNextPage)
            {
                GoToPage(PageIndex + 1);
            }
        }

        public void GoToNextPage()
        {

            OnGoToNextPage();

        }



    }

}
