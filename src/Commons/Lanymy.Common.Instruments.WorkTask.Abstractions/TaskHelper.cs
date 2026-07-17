using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{

    public static class TaskHelper
    {

        public static void SyncWait(Task task)
        {
            task.GetAwaiter().GetResult();
        }

    }

}
