using System;
using System.Diagnostics;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// cmd 命令行操作器
    /// </summary>
    public class LanymyCmd : BaseCmd
    {

        protected Action<string> OutputDataReceivedAction { get; }
        protected Action<string> ErrorDataReceivedAction { get; }

        public LanymyCmd(Action<string> outputDataReceivedAction = null, Action<string> errorDataReceivedAction = null)
        {

            OutputDataReceivedAction = outputDataReceivedAction;
            ErrorDataReceivedAction = errorDataReceivedAction;

        }


        protected override void OnOutputDataReceivedEvent(object sender, DataReceivedEventArgs e)
        {

            //var data = e.Data;

            //Debug.WriteLine(data);

            OutputDataReceivedAction?.Invoke(e.Data);

        }

        protected override void OnErrorDataReceivedEvent(object sender, DataReceivedEventArgs e)
        {


            //var data = e.Data;

            //Debug.WriteLine(data);

            ErrorDataReceivedAction?.Invoke(e.Data);


        }


    }
}
