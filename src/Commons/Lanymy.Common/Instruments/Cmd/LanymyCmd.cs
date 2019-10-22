using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Models.ResultModels;

namespace Lanymy.Common.Instruments.Cmd
{


    /// <summary>
    /// cmd 命令行操作器
    /// </summary>
    public class LanymyCmd : BaseCmd
    {

        private const string COMMAND_START_TAG = "CommandStartTag";
        private const string COMMAND_END_TAG = "CommandEndTag";
        /// <summary>
        /// echo
        /// 
        /// </summary>
        private const string COMMAND_ECHO = "echo";


        protected override void OnOutputDataReceivedEvent(object sender, DataReceivedEventArgs e)
        {

            if (!e.Data.IfIsNullOrEmpty() && e.Data.StartsWith(COMMAND_END_TAG))
            {
                _CurrentAutoResetEvent.Set();
            }

        }

        protected override void OnErrorDataReceivedEvent(object sender, DataReceivedEventArgs e)
        {


        }


        public LanymyCmdResultModel ExecuteCommandWithResultModel(params string[] args)
        {
            return ExecuteCommandWithResultModel(string.Join(" ", args));
        }

        public LanymyCmdResultModel ExecuteCommandWithResultModel(string cmdString)
        {


            var resultModel = new LanymyCmdResultModel
            {
                CmdID = Guid.NewGuid(),
                ExecuteCommandString = cmdString,
                CmdStartDateTime = DateTime.Now,
            };

            var startTagString = GetTagString(COMMAND_START_TAG, resultModel.CmdID);
            var endTagString = GetTagString(COMMAND_END_TAG, resultModel.CmdID);

            ExecuteCommand(GetCmdString(COMMAND_ECHO, startTagString));
            ExecuteCommand(cmdString);
            ExecuteCommand(GetCmdString(COMMAND_ECHO, endTagString));

            _CurrentAutoResetEvent.WaitOne();

            var outputDataString = _OutputDataReceivedMessage.ToString();
            outputDataString = outputDataString.LeftRemoveString(startTagString, true, 1);
            outputDataString = outputDataString.LeftSubString(endTagString);
            outputDataString = outputDataString.RightRemoveString(Environment.NewLine);
            outputDataString = outputDataString.Trim() + Environment.NewLine;

            resultModel.OutputDataString = outputDataString;
            resultModel.ErrorDataString = _ErrorDataReceivedMessage.ToString();
            resultModel.CmdEndDateTime = DateTime.Now;

            return resultModel;

        }

        private string GetTagString(string tag, Guid cmdID)
        {
            return string.Format("{0} - [ {1} ]", tag, cmdID);
        }

        //protected override void RunCmd(params string[] args)
        //{
        //    //if (!args.IfIsNullOrEmpty())
        //    //{
        //    //    RunCmd(string.Join(" ", args));
        //    //}
        //}

        //protected override void ExecuteCommand(string cmdString)
        //{



        //    //_CmdProcess.StandardInput.WriteLine(cmdString);
        //    //_CmdProcess.StandardInput.Flush();

        //}

        //public override void Dispose()
        //{



        //    base.Dispose();

        //}

    }

}
