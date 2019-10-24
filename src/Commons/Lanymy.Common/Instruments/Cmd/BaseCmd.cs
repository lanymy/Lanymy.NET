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

        //protected abstract Action<object, DataReceivedEventArgs> OutputDataReceivedAction { get; }
        //protected abstract Action<object, DataReceivedEventArgs> ErrorDataReceivedAction { get; }


        //protected AutoResetEvent _CurrentAutoResetEventLocker = new AutoResetEvent(false);
        //protected Process _CmdProcess { get; private set; }

        //private Task _Task;

        protected readonly StringBuilder _OutputDataReceivedMessage = new StringBuilder();
        protected readonly StringBuilder _ErrorDataReceivedMessage = new StringBuilder();

        private readonly ProcessStartInfo _CmdProcessStartInfo = ProcessHelper.GetProcessStartInfo("cmd", true);


        //protected BaseCmd(Action<object, DataReceivedEventArgs> outputDataReceivedAction, Action<object, DataReceivedEventArgs> errorDataReceivedAction)
        protected BaseCmd()
        {

            ////OutputDataReceivedAction = outputDataReceivedAction;
            ////ErrorDataReceivedAction = errorDataReceivedAction;

            ////_CurrentMutexLockName = typeof(BaseCmd).FullName + "LockName";

            //_Task = new Task(StartCmdProcess);
            //_Task.Start();

            ////确保 构造方法执行完 CMD 进程 已初始化完 , 处于可用状态 
            //_CurrentAutoResetEventLocker.WaitOne();

        }

        //private void StartCmdProcess()
        //{

        //    var cmdProcessStartInfo = ProcessHelper.GetProcessStartInfo("cmd", true);

        //    using (_CmdProcess = new Process())
        //    {

        //        var state = false;

        //        try
        //        {

        //            _CmdProcess.StartInfo = cmdProcessStartInfo;
        //            state = _CmdProcess.Start();

        //            _CmdProcess.OutputDataReceived += OnOutputDataReceived;
        //            _CmdProcess.ErrorDataReceived += OnErrorDataReceived;
        //            _CmdProcess.BeginOutputReadLine();
        //            _CmdProcess.BeginErrorReadLine();

        //        }
        //        catch
        //        {
        //        }
        //        finally
        //        {
        //            _CurrentAutoResetEventLocker.Set();
        //        }

        //        if (state)
        //        {

        //            _CmdProcess.WaitForExit();

        //            //_CmdProcess.Close();

        //            //if ((_CmdProcess.ExitCode != 0 && _CmdProcess.ExitCode != 1) || _CmdProcess != null)
        //            //{
        //            //    throw new Exception(_CmdProcess.ExitCode.ToString());
        //            //}

        //        }

        //    }

        //}

        //private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        //{

        //    _OutputDataReceivedMessage.AppendLine(e.Data);
        //    OnOutputDataReceivedEvent(sender, e);

        //}

        //private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        //{

        //    _ErrorDataReceivedMessage.AppendLine(e.Data);
        //    OnErrorDataReceivedEvent(sender, e);

        //}

        protected abstract void OnOutputDataReceivedEvent(object sender, DataReceivedEventArgs e);

        protected abstract void OnErrorDataReceivedEvent(object sender, DataReceivedEventArgs e);


        //public string GetCmdString(string cmd, params string[] args)
        //{

        //    var argsString = string.Empty;

        //    if (!args.IfIsNullOrEmpty())
        //    {
        //        argsString = string.Join(" ", args);
        //    }

        //    return string.Format("{0} {1}", cmd, argsString);

        //}

        //protected virtual void ExecuteCommand(string cmd, params string[] args)
        //{

        //    ExecuteCommand(GetCmdString(cmd, args));

        //}

        //protected virtual void ExecuteCommand(string cmdString)
        //{

        //    _ErrorDataReceivedMessage.Clear();
        //    _OutputDataReceivedMessage.Clear();

        //    _CmdProcess.StandardInput.WriteLine(cmdString);
        //    _CmdProcess.StandardInput.Flush();

        //}


        public string GetCmdString(string cmd, params string[] args)
        {

            var argsString = string.Empty;

            if (!args.IfIsNullOrEmpty())
            {
                argsString = string.Join(" ", args);
            }

            return string.Format("{0} {1}", cmd, argsString);

        }

        public virtual async Task<CmdResultModel> ExecuteCommandAsync(string cmd, params string[] args)
        {
            return await Task.Run(() => ExecuteCommand(cmd, args));
        }

        public virtual CmdResultModel ExecuteCommand(string cmd, params string[] args)
        {

            return ExecuteCommand(GetCmdString(cmd, args));

        }

        public virtual async Task<CmdResultModel> ExecuteCommandAsync(string cmdString)
        {
            return await Task.Run(() => ExecuteCommand(cmdString));
        }

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

                    //process.StandardInput.AutoFlush = true;
                    process.StandardInput.WriteLine(cmdString);
                    process.StandardInput.Flush();

                    process.StandardInput.Close();

                    //sb.Append(process.StandardOutput.ReadToEnd());
                    //sb.Append(process.StandardError.ReadToEnd());

                    //process.StandardInput.WriteLine("exit");

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


        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        //public virtual async void Dispose()
        public virtual void Dispose()
        {

            _OutputDataReceivedMessage.Clear();
            _ErrorDataReceivedMessage.Clear();

            //if (!_CmdProcess.IfIsNull())
            //{

            //    _CmdProcess.OutputDataReceived -= OnOutputDataReceived;
            //    _CmdProcess.ErrorDataReceived -= OnErrorDataReceived;
            //    _CmdProcess.StandardInput.WriteLine("exit");
            //    try
            //    {
            //        _CmdProcess.StandardInput.Flush();
            //    }
            //    catch
            //    {

            //    }
            //    _CmdProcess.Close();
            //    _CmdProcess.Dispose();
            //    _CmdProcess = null;

            //}

            //if (!_Task.IfIsNull())
            //{

            //    if (_Task.Status == TaskStatus.Running)
            //    {
            //        await _Task;
            //    }

            //    _Task.Dispose();
            //    _Task = null;

            //}

            //_CurrentAutoResetEventLocker?.Dispose();

        }

    }


}
