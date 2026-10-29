using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoManager
{
    internal class TaskItem
    {
        public DateTime? DueDate { get; set; }
        public string Title { get; set; }
        public int Id { get; set; }
        public bool IsDone { get; set; }
        public TaskPriority Priority { get; set; }


        public TaskItem(int id, string title, TaskPriority priority, DateTime? dueDate)
        {
            Id = id;
            Title = title;
            Priority = priority;
            DueDate = dueDate;
            IsDone = false;
        }
    }
}
