using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{


    public abstract class BaseTimerWorkTask : BaseWorkTask
    {

        protected readonly Func<TimerWorkTaskDataResult> _CurrentWorkFunc;
        public int TaskSleepMilliseconds { get; }
        protected CancellationTokenSource _CurrentCancellationTokenSource;
        protected Task _CurrentTask;


        protected BaseTimerWorkTask(Func<TimerWorkTaskDataResult> workFunc, int taskSleepMilliseconds = 3 * 1000)
        {

            _CurrentWorkFunc = workFunc;
            TaskSleepMilliseconds = taskSleepMilliseconds;

        }


        protected virtual TimerWorkTaskDataResult OnWorkFunc()
        {
            return _CurrentWorkFunc();
        }

        protected virtual void OnWorkError(Exception ex)
        {
        }

        private void TryOnWorkError(Exception ex)
        {
            try
            {
                OnWorkError(ex);
            }
            catch
            {
            }
        }


        protected virtual async Task OnTaskAsync(CancellationToken token)
        {

            try
            {
                while (!token.IsCancellationRequested)
                {

                    await Task.Delay(TaskSleepMilliseconds, token);

                    TimerWorkTaskDataResult timerWorkTaskDataResult;

                    try
                    {
                        timerWorkTaskDataResult = OnWorkFunc();
                    }
                    catch (OperationCanceledException) when (token.IsCancellationRequested)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        TryOnWorkError(ex);
                        continue;
                    }

                    if (!timerWorkTaskDataResult.IfIsNullOrEmpty() && timerWorkTaskDataResult.IsBreak)
                    {
                        break;
                    }

                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
            }

        }



        protected override async Task OnStartAsync()
        {

            _CurrentCancellationTokenSource = new CancellationTokenSource();
            var token = _CurrentCancellationTokenSource.Token;

            _CurrentTask = Task.Factory.StartNew(
                () => OnTaskAsync(token),
                token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default).Unwrap();

            await Task.CompletedTask;

        }



        protected override async Task OnStopAsync()
        {


            if (_CurrentCancellationTokenSource.IfIsNullOrEmpty())
            {
                return;
            }


            _CurrentCancellationTokenSource.Cancel();


            try
            {
                await _CurrentTask;
            }
            catch (OperationCanceledException)
            {
                // ignored
            }

            _CurrentTask.Dispose();
            _CurrentTask = null;

            if (!_CurrentCancellationTokenSource.IfIsNullOrEmpty())
            {
                _CurrentCancellationTokenSource.Dispose();
                _CurrentCancellationTokenSource = null;
            }


        }



    }

}
