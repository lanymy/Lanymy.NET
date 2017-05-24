///********************************************************************

//时间: 2015年01月21日, PM 02:31:13

//作者: lanyanmiyu@qq.com

//描述: 图集浏览器导航器

//其它:     

//********************************************************************/





//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Lanymy.General.Extension.Models;


//namespace Lanymy.General.Extension
//{


//    /// <summary>
//    /// 图集浏览器导航器
//    /// </summary>
//    public class AtlasBrowserPager
//    {
//        private List<int> _IndexListDataSource;

//        /// <summary>
//        /// 图集索引值数据源
//        /// </summary>
//        public List<int> IndexListDataSource
//        {

//            get
//            {
//                return _IndexListDataSource;
//            }
//            set
//            {
//                _IndexListDataSource = value;
//                _EntitySourcePager.SetDataSource(_IndexListDataSource, _EntitySourcePager.PageSize);
//                Refresh();
//            }
//        }

//        private int _CurrentImageIndex;
//        /// <summary>
//        /// 当前图片索引值
//        /// </summary>
//        public int CurrentImageIndex
//        {
//            get
//            {
//                return _CurrentImageIndex;
//            }
//            set
//            {
//                _CurrentImageIndex = value;
//                OnCurrentImageChanded(new CommonEventHandler.CommonEventArgs() { Sender = this });
//            }
//        }

//        private List<int> _CurrentImageIndexList;
//        /// <summary>
//        /// 当前页  图片索引值 集合
//        /// </summary>
//        public List<int> CurrentImageIndexList
//        {
//            get { return _CurrentImageIndexList; }
//        }

//        public int PageSize
//        {
//            get { return _EntitySourcePager.PageSize; }
//            set
//            {
//                _EntitySourcePager.PageSize = value;
//                Refresh();
//            }
//        }

//        private DataSourcePager<int> _EntitySourcePager;

//        /// <summary>
//        /// 分页导航控件
//        /// </summary>
//        public DataSourcePager<int> EntitySourcePager
//        {
//            get { return _EntitySourcePager; }
//        }


//        /// <summary>
//        /// 能否上一个
//        /// </summary>
//        public bool CanPrevSingle
//        {
//            get { return CurrentImageIndex > 0; }
//        }

//        /// <summary>
//        /// 能否下一个
//        /// </summary>
//        public bool CanNextSingle
//        {
//            get { return CurrentImageIndex < _EntitySourcePager.TotalCount - 1; }
//        }

//        /// <summary>
//        /// 能否上一页
//        /// </summary>
//        public bool CanPrevPage
//        {
//            get { return _EntitySourcePager.CanPrev; }
//        }

//        /// <summary>
//        /// 能否下一页
//        /// </summary>
//        public bool CanNextPage
//        {
//            get { return _EntitySourcePager.CanNext; }
//        }


//        public event CommonEventHandler.CommonEvent CurrentImageChanded;

//        protected void OnCurrentImageChanded(CommonEventHandler.CommonEventArgs e)
//        {
//            if (CurrentImageChanded != null)
//            {
//                CurrentImageChanded(e);
//            }
//        }


//        public AtlasBrowserPager()
//        {
//            _EntitySourcePager = new DataSourcePager<int>(_IndexListDataSource, 4);
//        }

//        /// <summary>
//        /// 导航到上一个单项
//        /// </summary>
//        public void GoPrevSingle()
//        {
//            if (_CurrentImageIndex > _CurrentImageIndexList[0])
//            {
//                CurrentImageIndex = _CurrentImageIndex - 1;
//            }
//            else
//            {
//                _CurrentImageIndexList = _EntitySourcePager.PrevPage().ToList();
//                CurrentImageIndex = _CurrentImageIndexList.Last();
//            }
//        }

//        /// <summary>
//        /// 导航到下一个单项
//        /// </summary>
//        public void GoNextSingle()
//        {
//            if (_CurrentImageIndex < _CurrentImageIndexList.Last())
//            {
//                CurrentImageIndex = _CurrentImageIndex + 1;
//            }
//            else
//            {
//                _CurrentImageIndexList = _EntitySourcePager.NextPage().ToList();
//                CurrentImageIndex = _CurrentImageIndexList.First();
//            }
//        }

//        /// <summary>
//        /// 导航到上一页
//        /// </summary>
//        public void GoPrevPage()
//        {
//            _CurrentImageIndexList = _EntitySourcePager.PrevPage().ToList();
//            CurrentImageIndex = _CurrentImageIndexList.First();
//        }


//        /// <summary>
//        /// 导航到下一页
//        /// </summary>
//        public void GoNextPage()
//        {
//            _CurrentImageIndexList = _EntitySourcePager.NextPage().ToList();
//            CurrentImageIndex = _CurrentImageIndexList.First();
//        }



//        /// <summary>
//        /// 刷新
//        /// </summary>

//        private void Refresh()
//        {
//            IEnumerable<int> list = _EntitySourcePager.GoToPage(0);


//            if (list != null)
//            {

//                _CurrentImageIndexList = list.ToList();
//                _CurrentImageIndex = CurrentImageIndexList[0];

//            }
//        }
//    }


//}
