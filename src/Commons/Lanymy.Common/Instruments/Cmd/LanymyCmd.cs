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

        //private const string COMMAND_START_TAG = "CommandStartTag";
        //private const string COMMAND_END_TAG = "CommandEndTag";

        ///// <summary>
        ///// echo
        ///// </summary>
        //private const string COMMAND_ECHO = "echo";

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

            //if (!data.IfIsNull())
            //{

            //    if (!data.Contains(COMMAND_START_TAG) && !data.Contains(COMMAND_END_TAG))
            //    {
            //        OutputDataReceivedAction?.Invoke(data);
            //    }

            //    //if (data.StartsWith(COMMAND_END_TAG))
            //    if (data.Contains(COMMAND_END_TAG))
            //    {
            //        _CurrentAutoResetEventLocker.Set();
            //    }

            //}

        }

        protected override void OnErrorDataReceivedEvent(object sender, DataReceivedEventArgs e)
        {


            //var data = e.Data;

            //Debug.WriteLine(data);

            ErrorDataReceivedAction?.Invoke(e.Data);

            //if (!data.IfIsNull())
            //{

            //    if (!data.Contains(COMMAND_START_TAG) && !data.Contains(COMMAND_END_TAG))
            //    {
            //        ErrorDataReceivedAction?.Invoke(data);
            //    }

            //    //if (data.StartsWith(COMMAND_END_TAG))
            //    if (data.Contains(COMMAND_END_TAG))
            //    {
            //        _CurrentAutoResetEventLocker.Set();
            //    }

            //}

        }


        //public CmdResultModel ExecuteCommandWithResultModel(params string[] args)
        //{
        //    return ExecuteCommandWithResultModel(string.Join(" ", args));
        //}

        //public CmdResultModel ExecuteCommandWithResultModel(string cmdString)
        //{


        //    var resultModel = new CmdResultModel
        //    {
        //        CmdID = Guid.NewGuid(),
        //        ExecuteCommandString = cmdString,
        //        CmdStartDateTime = DateTime.Now,
        //    };

        //    var str = ExecuteCommand(cmdString);

        //    resultModel.OutputDataString = str;
        //    resultModel.CmdEndDateTime = DateTime.Now;

        //    return resultModel;

        //}

        //public LanymyCmdResultModel ExecuteCommandWithResultModel(string cmdString)
        //{


        //    var resultModel = new LanymyCmdResultModel
        //    {
        //        CmdID = Guid.NewGuid(),
        //        ExecuteCommandString = cmdString,
        //        CmdStartDateTime = DateTime.Now,
        //    };

        //    var startTagString = GetTagString(COMMAND_START_TAG, resultModel.CmdID);
        //    var endTagString = GetTagString(COMMAND_END_TAG, resultModel.CmdID);

        //    ExecuteCommand(startTagString);
        //    ExecuteCommand(cmdString);
        //    ExecuteCommand(endTagString);

        //    //ExecuteCommand(GetCmdString(COMMAND_ECHO, startTagString));
        //    //ExecuteCommand(cmdString);
        //    //ExecuteCommand(GetCmdString(COMMAND_ECHO, endTagString));

        //    _CurrentAutoResetEventLocker.WaitOne();
        //    _CurrentAutoResetEventLocker.WaitOne();

        //    var outputDataString = _OutputDataReceivedMessage.ToString();
        //    outputDataString = outputDataString.LeftRemoveString(startTagString, true, 1);
        //    outputDataString = outputDataString.LeftSubString(endTagString);
        //    outputDataString = outputDataString.RightRemoveString(Environment.NewLine);
        //    outputDataString = outputDataString.Trim() + Environment.NewLine;

        //    resultModel.OutputDataString = outputDataString;
        //    resultModel.ErrorDataString = _ErrorDataReceivedMessage.ToString();
        //    resultModel.CmdEndDateTime = DateTime.Now;

        //    return resultModel;

        //}

        //private string GetTagString(string tag, Guid cmdID)
        //{
        //    return string.Format("{0} - [ {1} ]", tag, cmdID);
        //}

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
