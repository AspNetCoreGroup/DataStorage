using System.Collections.Concurrent;

namespace DataStorageWebApi.TaskManager
{
    public class TaskManager: ITaskManager
    {
        private readonly ConcurrentBag<Task> _tasks = new ConcurrentBag<Task>();

        public void AddTask(Task task)
        {
            _tasks.Add(task);
        }

        public IEnumerable<Task> GetTasks()
        {
            return _tasks;
        }

        public void RemoveCompletedTasks()
        {
            var completedTasks = _tasks.Where(t => t.IsCompleted).ToList();
            foreach (var task in completedTasks)
            {
                _tasks.TryTake(out _);
            }
        }
    }
}
