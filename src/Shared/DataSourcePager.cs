///********************************************************************

//时间: 2015年01月14日, PM 05:49:13

//作者: lanyanmiyu@qq.com

//描述: 实体集数据源分页控件

//其它: 为了不使用反射提高执行效率  实体集的排序 请先在外部 自行排序 再传入排好序的实体集

//********************************************************************/








//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Lanymy.General.Extension.ExtensionFunctions;


//namespace Lanymy.General.Extension
//{


//    /// <summary>
//    /// 实体集数据源分页控件
//    /// <para>实体集的排序 请先在外部 自行排序 再传入排好序的实体集</para>
//    /// </summary>
//    /// <typeparam name="T">实体集合的实体类</typeparam>
//    public class DataSourcePager<T>
//    {


//        #region 成员变量及属性


//        /// <summary>
//        /// 能否上一页
//        /// </summary>
//        public bool CanPrev
//        {
//            get
//            {
//                return _PageCount > 0 && _PageIndex > 0;
//            }
//        }


//        /// <summary>
//        /// 能否下一页
//        /// </summary>
//        public bool CanNext
//        {
//            get
//            {
//                return _PageCount > 0 && _PageIndex < PageCount - 1;
//            }
//        }

//        /// <summary>
//        /// 总数量
//        /// </summary>
//        private int _TotalCount;


//        /// <summary>
//        /// 获得总数量
//        /// </summary>
//        //[Browsable(false)]
//        public int TotalCount
//        {
//            get
//            {
//                return _TotalCount;
//            }
//        }



//        /// <summary>
//        /// 页数量
//        /// </summary>
//        private int _PageCount;

//        /// <summary>
//        /// 获得总页数
//        /// </summary>
//        //[Browsable(false)]
//        public int PageCount
//        {
//            get
//            {
//                return _PageCount;
//            }
//        }

//        /// <summary>
//        /// 页号索引值
//        /// </summary>
//        private int _PageIndex;

//        /// <summary>
//        /// 页号索引值
//        /// </summary>
//        //[Category("分页"), Description("页号索引值")]
//        public int PageIndex
//        {
//            get
//            {
//                return _PageIndex;
//            }
//        }//end method


//        /// <summary>
//        /// 全部刷新
//        /// </summary>
//        private void CalcPaging(bool resetPageIndex)
//        {

//            if (_PageSize <= 0)
//                throw new ArgumentOutOfRangeException("PageSize");

//            if (_TotalCount <= 0)
//            {
//                _PageCount = 1;
//                _PageIndex = 0;

//                return;
//            }


//            //计算页数
//            _PageCount = _TotalCount % _PageSize == 0 ? _TotalCount / _PageSize : _TotalCount / _PageSize + 1;
//            if (resetPageIndex)
//            {
//                _PageIndex = 0;
//            }


//        }//end method


//        /// <summary>
//        /// 每页显示数
//        /// </summary>
//        private int _PageSize;

//        /// <summary>
//        /// 每页显示数
//        /// </summary>
//        //[Category("分页"), Description("每页显示的项数。")]
//        public int PageSize
//        {
//            get
//            {
//                return _PageSize;
//            }
//            set
//            {
//                _PageSize = value;
//                this.CalcPaging(true);
//            }
//        }



//        ///// <summary>
//        ///// 数据源
//        ///// </summary>
//        //private IEnumerable<T> _DataSource = null;

//        ///// <summary>
//        ///// 设置或获得数据源
//        ///// </summary>
//        ////[Browsable(false)]
//        //public IEnumerable<T> DataSource
//        //{
//        //    get
//        //    {
//        //        return _DataSource;
//        //    }
//        //    set
//        //    {
//        //        _DataSource = value;
//        //        CalcPaging(true);
//        //    }
//        //}

//        Func<List<T>> _FuncSearch;

//        #endregion //end成员变量及属性

//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        /// <param name="dataSource">实体集数据源</param>
//        /// <param name="pageSize">一页多少数据</param>
//        public DataSourcePager(IEnumerable<T> dataSource, int pageSize)
//        {

//            SetDataSource(dataSource, pageSize);

//        }

//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        /// <param name="dataSource">实体集数据源</param>
//        /// <param name="pageSize">一页多少数据</param>
//        public DataSourcePager(IQueryable<T> dataSource, int pageSize)
//        {
//            SetDataSource(dataSource, pageSize);
//        }



//        /// <summary>
//        /// 绑定数据源
//        /// </summary>
//        /// <param name="dataSource"></param>
//        /// <param name="pageSize"></param>
//        public void SetDataSource(IEnumerable<T> dataSource, int pageSize)
//        {
//            _PageSize = pageSize;

//            if (dataSource.IfIsNullOrEmpty())
//            {
//                _TotalCount = 0;
//                _FuncSearch = null;
//            }
//            else
//            {
//                _TotalCount = dataSource.Count();
//                _FuncSearch = () =>
//                {
//                    if (dataSource != null && _TotalCount > 0)
//                    {
//                        return dataSource.Skip(_PageIndex * _PageSize).Take(_PageSize).ToList();
//                    }

//                    return null;
//                };
//            }

//            CalcPaging(true);
//        }


//        /// <summary>
//        /// 绑定数据源
//        /// </summary>
//        /// <param name="dataSource"></param>
//        /// <param name="pageSize"></param>
//        public void SetDataSource(IQueryable<T> dataSource, int pageSize)
//        {
//            _PageSize = pageSize;

//            if (dataSource.IfIsNullOrEmpty())
//            {
//                _TotalCount = 0;
//                _FuncSearch = null;
//            }
//            else
//            {
//                _TotalCount = dataSource.Count();
//                _FuncSearch = () =>
//                {
//                    if (dataSource != null && _TotalCount > 0)
//                    {
//                        return dataSource.Skip(_PageIndex * _PageSize).Take(_PageSize).ToList();
//                    }

//                    return null;
//                };
//            }

//            CalcPaging(true);
//        }

//        /// <summary>
//        /// 上一页
//        /// </summary>
//        /// <returns></returns>
//        public List<T> PrevPage()
//        {
//            return GoToPage(PageIndex - 1);
//        }

//        /// <summary>
//        /// 下一页
//        /// </summary>
//        /// <returns></returns>
//        public List<T> NextPage()
//        {
//            return GoToPage(PageIndex + 1);
//        }

//        /// <summary>
//        /// 转到页
//        /// </summary>
//        /// <param name="pageIndex">页码索引值</param>
//        public List<T> GoToPage(int pageIndex)
//        {

//            _PageIndex = pageIndex;

//            if (!_FuncSearch.IfIsNullOrEmpty())
//            {
//                return _FuncSearch();
//            }


//            return null;

//        }


//    }


//}
