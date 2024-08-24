namespace DataStorageWebApi.TaskManager
{
    public interface ITaskManager
    {
        public void AddTask(Task task);

        public IEnumerable<Task> GetTasks();

        public void RemoveCompletedTasks();

    }
}
