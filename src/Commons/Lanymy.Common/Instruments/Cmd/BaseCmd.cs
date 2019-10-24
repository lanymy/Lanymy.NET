using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Models.ResultModels;

namespace Lanymy.Common.Instruments.Cmd
{



    /// <summary>
    /// cmd 命令行操作器 基类
    /// </summary>
    public abstract class BaseCmd : IDisposable
    {

        protected readonly StringBuilder _OutputDataReceivedMessage = new StringBuilder();
        protected readonly StringBuilder _ErrorDataReceivedMessage = new StringBuilder();

        private readonly ProcessStartInfo _CmdProcessStartInfo = ProcessHelper.GetProcessStartInfo("cmd", true);

        protected abstract void OnOutputDataReceivedEvent(object sender, DataReceivedEventArgs e);

        protected abstract void OnErrorDataReceivedEvent(object sender, DataReceivedEventArgs e);


        /// <summary>
        /// 组装CMD 命令 成完整字符串
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string GetCmdString(string cmd, params string[] args)
        {

            var argsString = string.Empty;

            if (!args.IfIsNullOrEmpty())
            {
                argsString = string.Join(" ", args);
            }

            return string.Format("{0} {1}", cmd, argsString);

        }


        /// <summary>
        /// 异步执行CMD命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual async Task<CmdResultModel> ExecuteCommandAsync(string cmd, params string[] args)
        {
            return await Task.Run(() => ExecuteCommand(cmd, args));
        }

        /// <summary>
        /// 执行CMD命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual CmdResultModel ExecuteCommand(string cmd, params string[] args)
        {
            return ExecuteCommand(GetCmdString(cmd, args));
        }


        /// <summary>
        /// 异步执行CMD字符串
        /// </summary>
        /// <param name="cmdString"></param>
        /// <returns></returns>
        public virtual async Task<CmdResultModel> ExecuteCommandAsync(string cmdString)
        {
            return await Task.Run(() => ExecuteCommand(cmdString));
        }


        /// <summary>
        /// 执行CMD字符串
        /// </summary>
        /// <param name="cmdString"></param>
        /// <returns></returns>
        public virtual CmdResultModel ExecuteCommand(string cmdString)
        {

            _OutputDataReceivedMessage.Clear();
            _ErrorDataReceivedMessage.Clear();

            var isSuccess = false;

            var resultModel = new CmdResultModel
            {
                CmdID = Guid.NewGuid(),
                ExecuteCommandString = cmdString,
                CmdStartDateTime = DateTime.Now,
            };


            using (var process = new Process())
            {

                try
                {

                    process.StartInfo = _CmdProcessStartInfo;
                    process.Start();

                    process.OutputDataReceived += OnOutputDataReceived;
                    process.ErrorDataReceived += OnErrorDataReceived;
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.StandardInput.WriteLine(cmdString);
                    process.StandardInput.Flush();

                    process.StandardInput.Close();



                    process.WaitForExit();

                    process.OutputDataReceived -= OnOutputDataReceived;
                    process.ErrorDataReceived -= OnErrorDataReceived;

                    isSuccess = true;

                }
                catch (Exception ex)
                {

                    isSuccess = false;
                    resultModel.Exception = ex;

                }
                finally
                {

                    process.Close();

                }


            }

            resultModel.OutputDataString = _OutputDataReceivedMessage.ToString();
            resultModel.ErrorDataString = _ErrorDataReceivedMessage.ToString();
            resultModel.CmdEndDateTime = DateTime.Now;
            resultModel.IsSuccess = isSuccess;

            return resultModel;

        }


        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {

            _OutputDataReceivedMessage.AppendLine(e.Data);
            OnOutputDataReceivedEvent(sender, e);

        }

        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {

            _ErrorDataReceivedMessage.AppendLine(e.Data);
            OnErrorDataReceivedEvent(sender, e);

        }

        public virtual void Dispose()
        {

            _OutputDataReceivedMessage.Clear();
            _ErrorDataReceivedMessage.Clear();

        }

    }


}
