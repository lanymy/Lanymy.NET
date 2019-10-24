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

                var resultModel = cmd.ExecuteCommand("ipconfig");

                if (resultModel.IsSuccess)
                {

                    string outputDataString = resultModel.OutputDataString;
                    string errorDataString = resultModel.ErrorDataString;

                }

            }


            using (var cmd = new LanymyCmd())
            {

                var resultModel = cmd.ExecuteCommandAsync("ipconfig").Result;

                if (resultModel.IsSuccess)
                {

                    string outputDataString = resultModel.OutputDataString;
                    string errorDataString = resultModel.ErrorDataString;

                }

            }


        }



    }
}