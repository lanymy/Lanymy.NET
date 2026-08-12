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

    }

}
