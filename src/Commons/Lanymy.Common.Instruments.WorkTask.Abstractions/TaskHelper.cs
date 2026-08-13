using System;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{

    public static class TaskHelper
    {

        public static void SyncWait(Task task)
        {
            task.GetAwaiter().GetResult();
        }

        public static void SyncWait(Func<Task> taskFactory)
        {
            if (taskFactory == null)
            {
                return;
            }

            SyncWait(taskFactory());
        }

        public static TResult SyncWait<TResult>(Task<TResult> task)
        {
            return task.GetAwaiter().GetResult();
        }

        public static TResult SyncWait<TResult>(Func<Task<TResult>> taskFactory)
        {
            if (taskFactory == null)
            {
                return default;
            }

            return SyncWait(taskFactory());
        }

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
