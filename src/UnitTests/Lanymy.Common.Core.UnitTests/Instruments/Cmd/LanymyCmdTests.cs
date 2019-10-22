using Lanymy.Common.Instruments.Cmd;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.Core.UnitTests.Instruments.Cmd
{

    [TestClass()]
    public class LanymyCmdTests
    {

        [TestMethod()]
        public void LanymyCmdTest()
        {

            using (var cmd = new LanymyCmd())
            {

                var resultModel = cmd.ExecuteCommandWithResultModel("ipconfig");

                if (resultModel.IsSuccess)
                {

                    string str = resultModel.OutputDataString;

                }

            }

        }

    }
}