using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{


    public abstract class BaseSimpleWorkTaskQueue<TData> : BaseWorkTask
    //where TAddToQueueData : class
    //where TQueueData : class
    {

        protected Task _CurrentTask;
        protected CancellationTokenSource _CurrentCancellationTokenSource;
        protected readonly ConcurrentQueue<TData> _CurrentCacheConcurrentQueue = new ConcurrentQueue<TData>();
        private readonly Action<TData> _WorkAction;
        private readonly int _SleepIntervalMilliseconds;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workAction"></param>
        /// <param name="sleepIntervalMilliseconds">执行完当前消息队列所有信息后,到下一次循环的空闲等待时间; 小于等于0 为不等待</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected BaseSimpleWorkTaskQueue(Action<TData> workAction, int sleepIntervalMilliseconds = 10)
        {

            if (workAction.IfIsNull())
            {
                throw new ArgumentNullException(nameof(workAction));
            }


            if (sleepIntervalMilliseconds < 0)
            {
                sleepIntervalMilliseconds = 0;
            }

            _SleepIntervalMilliseconds = sleepIntervalMilliseconds;
            _WorkAction = workAction;

        }



        protected virtual void OnWorkAction(TData data)
        {
            _WorkAction(data);
        }

        protected virtual void OnWorkError(TData data, Exception ex)
        {
        }

        protected virtual Task DelayAsync(CancellationToken token)
        {
            if (_SleepIntervalMilliseconds <= 0)
            {
                return Task.CompletedTask;
            }

            return Task.Delay(_SleepIntervalMilliseconds, token);
        }

        private void TryOnWorkError(TData data, Exception ex)
        {
            try
            {
                OnWorkError(data, ex);
            }
            catch
            {
            }
        }

        protected virtual void ClearCurrentCacheConcurrentQueue()
        {
#if NET48
            TData data;

            while (_CurrentCacheConcurrentQueue.TryDequeue(out data))
            {
            }
#else
            _CurrentCacheConcurrentQueue.Clear();
#endif
        }


        private async Task OnTaskAsync(CancellationToken token)
        {

            TData data;

            while (!token.IsCancellationRequested && IsRunning)
            {

                while (_CurrentCacheConcurrentQueue.TryDequeue(out data))
                {

                    try
                    {
                        OnWorkAction(data);
                    }
                    catch (Exception ex)
                    {
                        TryOnWorkError(data, ex);
                    }

                }

                if (_SleepIntervalMilliseconds > 0)
                {
                    await DelayAsync(token);
                }

            }

        }

        public virtual void AddToQueue(TData data)
        {

            if (IsRunning)
            {
                _CurrentCacheConcurrentQueue.Enqueue(data);
            }

        }

        protected override async Task OnStartAsync()
        {
            if (_CurrentCancellationTokenSource.IfIsNull())
            {
                _CurrentCancellationTokenSource = new CancellationTokenSource();
            }

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
            Exception stopException = null;
            var currentTask = _CurrentTask;
            var currentCancellationTokenSource = _CurrentCancellationTokenSource;

            try
            {
                currentCancellationTokenSource?.Cancel();
            }
            catch (Exception ex)
            {
                stopException = ex;
            }

            try
            {
                if (!currentTask.IfIsNullOrEmpty())
                {
                    await currentTask;
                }
            }
            catch (OperationCanceledException) when (currentCancellationTokenSource != null && currentCancellationTokenSource.IsCancellationRequested)
            {
                // ignored
            }
            catch (Exception ex)
            {
                stopException = ex;
            }
            finally
            {
                if (ReferenceEquals(_CurrentTask, currentTask))
                {
                    _CurrentTask = null;
                }

                currentTask?.Dispose();

                if (ReferenceEquals(_CurrentCancellationTokenSource, currentCancellationTokenSource))
                {
                    _CurrentCancellationTokenSource = null;
                }

                currentCancellationTokenSource?.Dispose();
                ClearCurrentCacheConcurrentQueue();
            }

            if (stopException != null)
            {
                throw stopException;
            }

        }

    }

}
