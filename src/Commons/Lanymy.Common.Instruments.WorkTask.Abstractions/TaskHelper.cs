using System;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 提供 Task 与同步调用之间的桥接辅助能力。
    /// </summary>
    public static class TaskHelper
    {
        /// <summary>
        /// 同步等待一个异步任务完成，并保留原始异常语义。
        /// </summary>
        /// <param name="task">需要等待的任务。</param>
        public static void SyncWait(Task task)
        {
            task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// 同步执行一个任务工厂；工厂为空时直接返回。
        /// </summary>
        /// <param name="taskFactory">创建任务的委托。</param>
        public static void SyncWait(Func<Task> taskFactory)
        {
            if (taskFactory == null)
            {
                return;
            }

            SyncWait(taskFactory());
        }

        /// <summary>
        /// 同步等待一个返回结果的异步任务完成。
        /// </summary>
        /// <typeparam name="TResult">任务结果类型。</typeparam>
        /// <param name="task">需要等待的任务。</param>
        /// <returns>任务结果。</returns>
        public static TResult SyncWait<TResult>(Task<TResult> task)
        {
            return task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// 同步执行一个返回结果的任务工厂；工厂为空时返回默认值。
        /// </summary>
        /// <typeparam name="TResult">任务结果类型。</typeparam>
        /// <param name="taskFactory">创建任务的委托。</param>
        /// <returns>任务结果。</returns>
        public static TResult SyncWait<TResult>(Func<Task<TResult>> taskFactory)
        {
            if (taskFactory == null)
            {
                return default;
            }

            return SyncWait(taskFactory());
        }

        /// <summary>
        /// 同步执行任务工厂，并把异常折叠为返回值而不是直接抛出。
        /// </summary>
        /// <param name="taskFactory">创建任务的委托。</param>
        /// <returns>执行过程中捕获的异常；成功时返回 <see langword="null"/>。</returns>
        public static Exception TrySyncWait(Func<Task> taskFactory)
        {
            try
            {
                SyncWait(taskFactory);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// 同步执行返回结果的任务工厂，并把异常折叠为返回值。
        /// </summary>
        /// <typeparam name="TResult">任务结果类型。</typeparam>
        /// <param name="taskFactory">创建任务的委托。</param>
        /// <param name="result">成功时返回任务结果，失败时返回默认值。</param>
        /// <returns>执行过程中捕获的异常；成功时返回 <see langword="null"/>。</returns>
        public static Exception TrySyncWait<TResult>(Func<Task<TResult>> taskFactory, out TResult result)
        {
            try
            {
                result = SyncWait(taskFactory);
                return null;
            }
            catch (Exception ex)
            {
                result = default;
                return ex;
            }
        }

    }

}
