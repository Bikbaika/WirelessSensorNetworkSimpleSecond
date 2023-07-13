using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirelessSensorNetworkSimpleSecond
{
    public class TaskScheduler
    {
        private List<Task> tasks = new List<Task>();

        public void ScheduleTask(Task task)
        {
            tasks.Add(task);
        }

        public void RunTasks()
        {
            foreach (var task in tasks)
            {
                task.RunSynchronously();
            }

            tasks.Clear();
        }
    }
}
