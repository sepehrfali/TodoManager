using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Xml.Linq;

namespace TodoManager
{
    internal class TaskService : ITaskService
    {

        private readonly string _filePath;

        private List<TaskItem> _tasks = new List<TaskItem>();

        private int _nextId = 1;

        public string? LastError { get; private set; }

        public TaskService(string filePath = "tasks.json") 
        {
            _filePath = filePath;
            Load();
        }
        public IReadOnlyList<TaskItem> GetAll()
        {
            return _tasks;
        }
        public List<TaskItem> GetPending()
        {
            return _tasks
                .Where(t => !t.IsDone)
                .OrderByDescending(t => t.Priority)
                .ToList();
        }
        public List<TaskItem> GetOverdue()
        {
            return _tasks
                .Where(t => !t.IsDone && t.DueDate != null && t.DueDate < DateTime.Now )
                .OrderBy(t => t.DueDate)
                .ToList();
        }
        public TaskStatas GetStatas()
        {
            int total = _tasks.Count;
            int done = _tasks.Count(t => t.IsDone);
            int overdue = _tasks.Count(t => !t.IsDone && t.DueDate != null && t.DueDate < DateTime.Now);
            return new TaskStatas(total, done, total-done, overdue);
        }
        public TaskItem Add(string title, TaskPriority priority, DateTime? dueDate = null)
        {
            var task = new TaskItem(_nextId, title, priority, dueDate);
            _tasks.Add(task);
            _nextId++;
            Save();
            return task;
        }
        public bool Delete(int id)
        {
            TaskItem? task = _tasks.Find(t => t.Id == id);
            if (task == null)
            {
                return false;
            }
            _tasks.Remove(task);
            Save();
            return true;
        }
        public bool MarkDone(int id)
        {
            TaskItem? task = _tasks.Find(t => t.Id == id);
            if (task == null)
            {
                return false;
            }
            task.IsDone = true;
            Save();
            return true;
        }

        private void Save()
        {   
            LastError = null;
            try
            {
                string json = JsonSerializer.Serialize(_tasks, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            catch (IOException)
            {
                LastError = "Could not save tasks.";
            }
            catch (UnauthorizedAccessException)
            {
                LastError = "No permission to write the tasks file.";
            }
        }

        private void Load()
        {
            if (!File.Exists(_filePath))
            {
                return;
            }
            try
            {
                string json = File.ReadAllText(_filePath);
                _tasks = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
                _nextId = _tasks.Count == 0 ? 1 : _tasks.Max(t => t.Id) + 1;
            }
            catch (JsonException)
            {
                LastError = "tasks.json is corrupted. Starting with an empty list.";
                _tasks = new List<TaskItem>();
                _nextId = 1;
            }
        }
    }
}
