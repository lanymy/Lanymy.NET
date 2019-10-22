using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments.Cmd
{



    /// <summary>
    /// cmd 命令行操作器 基类
    /// </summary>
    public abstract class BaseCmd : IDisposable
    {

        //protected abstract Action<object, DataReceivedEventArgs> OutputDataReceivedAction { get; }
        //protected abstract Action<object, DataReceivedEventArgs> ErrorDataReceivedAction { get; }


        protected AutoResetEvent _CurrentAutoResetEvent = new AutoResetEvent(false);
        protected Process _CmdProcess { get; private set; }

        private Task _Task;

        protected readonly StringBuilder _OutputDataReceivedMessage = new StringBuilder();
        protected readonly StringBuilder _ErrorDataReceivedMessage = new StringBuilder();

        //protected BaseCmd(Action<object, DataReceivedEventArgs> outputDataReceivedAction, Action<object, DataReceivedEventArgs> errorDataReceivedAction)
        protected BaseCmd()
        {

            //OutputDataReceivedAction = outputDataReceivedAction;
            //ErrorDataReceivedAction = errorDataReceivedAction;

            //_CurrentMutexLockName = typeof(BaseCmd).FullName + "LockName";

            _Task = new Task(StartCmdProcess);
            _Task.Start();

            //确保 构造方法执行完 CMD 进程 已初始化完 , 处于可用状态 
            _CurrentAutoResetEvent.WaitOne();

        }

        private void StartCmdProcess()
        {

            var cmdProcessStartInfo = ProcessHelper.GetProcessStartInfo("cmd", true);

            using (_CmdProcess = new Process())
            {

                var state = false;

                try
                {

                    _CmdProcess.StartInfo = cmdProcessStartInfo;
                    state = _CmdProcess.Start();

                    _CmdProcess.OutputDataReceived += OnOutputDataReceived;
                    _CmdProcess.ErrorDataReceived += OnErrorDataReceived;
                    _CmdProcess.BeginOutputReadLine();
                    _CmdProcess.BeginErrorReadLine();

                }
                catch
                {
                }
                finally
                {
                    _CurrentAutoResetEvent.Set();
                }

                if (state)
                {

                    _CmdProcess.WaitForExit();

                    //_CmdProcess.Close();

                    //if ((_CmdProcess.ExitCode != 0 && _CmdProcess.ExitCode != 1) || _CmdProcess != null)
                    //{
                    //    throw new Exception(_CmdProcess.ExitCode.ToString());
                    //}

                }

            }

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

        protected abstract void OnOutputDataReceivedEvent(object sender, DataReceivedEventArgs e);

        protected abstract void OnErrorDataReceivedEvent(object sender, DataReceivedEventArgs e);


        public string GetCmdString(string cmd, params string[] args)
        {

            var argsString = string.Empty;

            if (!args.IfIsNullOrEmpty())
            {
                argsString = string.Join(" ", args);
            }

            return string.Format("{0} {1}", cmd, argsString);

        }

        protected virtual void ExecuteCommand(string cmd, params string[] args)
        {

            ExecuteCommand(GetCmdString(cmd, args));

        }

        protected virtual void ExecuteCommand(string cmdString)
        {

            _ErrorDataReceivedMessage.Clear();
            _OutputDataReceivedMessage.Clear();

            _CmdProcess.StandardInput.WriteLine(cmdString);
            _CmdProcess.StandardInput.Flush();

        }


        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public virtual async void Dispose()
        {

            _OutputDataReceivedMessage.Clear();
            _ErrorDataReceivedMessage.Clear();

            if (!_CmdProcess.IfIsNull())
            {

                _CmdProcess.OutputDataReceived -= OnOutputDataReceived;
                _CmdProcess.ErrorDataReceived -= OnErrorDataReceived;

                _CmdProcess.Dispose();
                _CmdProcess = null;

            }

            if (!_Task.IfIsNull())
            {

                if (_Task.Status == TaskStatus.Running)
                {
                    await _Task;
                }

                _Task.Dispose();
                _Task = null;

            }

            _CurrentAutoResetEvent?.Dispose();

        }

    }


}
