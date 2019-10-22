using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.Core.UnitTests
{
    [TestClass()]
    public class PathHelperTests
    {


        [TestMethod()]
        public void CombineRelativePathTest()
        {

            string path = @"E:\MediaToolkit\新建文件夹";

            var path1 = PathHelper.CombineSystemRelativePath(path, @"../");
            Assert.AreEqual(path1, @"E:\MediaToolkit\");

            var path2 = @"E:\MediaToolkit";
            var path3 = PathHelper.MakeRelativePath(path1, path2);
            Assert.AreEqual(path3, @"../MediaToolkit");

            var path4 = @"http://www.abc.com/a/b/c.html";
            var path5 = PathHelper.CombineHttpRelativePath(path4, "../123.html");
            Assert.AreEqual(path5, @"http://www.abc.com/a/123.html");

            var path6 = PathHelper.MakeRelativePath(path4, path5);
            Assert.AreEqual(path6, @"../123.html");

            var path7 = PathHelper.CombineHttpRelativePath(path4, "/");
            Assert.AreEqual(path7, @"http://www.abc.com/");

            var path8 = PathHelper.CombineSystemRelativePath(path, "/");
            Assert.AreEqual(path8, @"E:\");


            var path9 = PathHelper.CombineRelativePath(path, @"../");
            Assert.AreEqual(path9, @"E:\MediaToolkit\");

            var path10 = PathHelper.CombineRelativePath(path4, "../123.html");
            Assert.AreEqual(path10, @"http://www.abc.com/a/123.html");

            var path11 = PathHelper.CombineRelativePath(path4, "/");
            Assert.AreEqual(path11, @"http://www.abc.com/");

            var path12 = PathHelper.CombineRelativePath(path, "/");
            Assert.AreEqual(path12, @"E:\");

        }

    }
}