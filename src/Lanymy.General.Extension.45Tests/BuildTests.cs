using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.General.Extension._45Tests
{
    [TestClass()]
    public class BuildTests
    {
        [TestMethod]
        public void BuildTest()
        {
            var callDomainPath = PathFunctions.GetCallDomainPath();
        }
    }
}
