/********************************************************************

时间: 2017年06月02日, AM 07:58:21

作者: lanyanmiyu@qq.com

描述: 单元测试基类

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lanymy.General.Extension.Models
{


    /// <summary>
    /// 单元测试基类
    /// </summary>
    public abstract class BaseTestModel
    {

        /// <summary>
        /// 当前测试用例根目录名称
        /// </summary>
        protected readonly string _TestRootDirectoryName;

        /// <summary>
        /// 当前测试用例文件名称
        /// </summary>
        protected readonly string _TestFileFullName;

        /// <summary>
        /// 当前测试用例文件全路径
        /// </summary>
        protected readonly string _TestFileFullPath = string.Empty;

        /// <summary>
        /// 当前测试用例根目录全路径
        /// </summary>
        protected readonly string _TestFileRootDirectoryFullPath = string.Empty;

        protected BaseTestModel(string testFileFullName)
        {
            _TestRootDirectoryName = this.GetType().Name;
            _TestFileFullName = testFileFullName;
            _TestFileRootDirectoryFullPath = Path.Combine(PathFunctions.GetCallDomainPath(), _TestRootDirectoryName);
            _TestFileFullPath = Path.Combine(_TestFileRootDirectoryFullPath, _TestFileFullName);
            PathFunctions.InitDirectoryPath(_TestFileRootDirectoryFullPath);
        }



    }


}
