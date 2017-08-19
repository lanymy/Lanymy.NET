using System;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using Lanymy.General.Extension.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Asn1.Pkcs;

namespace Lanymy.General.Extension._40Tests
{

    [TestClass]
    public class PathFunctionsTests
    {



        [TestMethod]
        public void CombineRelativePathTest()
        {

            string path1 = "http://aaaa/bbb";
            string path2 = "../ccc/";

            string result = PathFunctions.CombineRelativePath(path1, path2);

        }







    }


}
