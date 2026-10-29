namespace TodoManager
{
    internal interface ITaskService
    {
        string? LastError { get; }

        TaskStatas GetStatas();
        IReadOnlyList<TaskItem> GetAll();
        List<TaskItem> GetPending();
        List<TaskItem> GetOverdue();
        TaskItem Add(string title, TaskPriority priority, DateTime? dueDate = null);
        bool Delete(int id);
        bool MarkDone(int id);
    }
}