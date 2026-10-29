
using System.Globalization;
using System.Reflection.Metadata;

namespace TodoManager
{
    internal class Program
    {
        static ITaskService service = new TaskService();
        static void Main(string[] args)
        {
          
            if (!string.IsNullOrEmpty(service.LastError))
            {
                ShowServiceError();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

            bool running = true;
                        
            while (running)
            {
                ShowMenu();

                var choice = Console.ReadLine();

                switch (choice)
                {

                    case "1":
                        AddTask();
                        break;
                    case "2":
                        ShowTask();
                        break;
                    case "3":
                        DeleteTask();
                        break;
                    case "4":
                        MarkTaskDone();
                        break;
                    case "5":
                        ShowPendingTasks();
                        break;
                    case "6":
                        ViewOverdueTasks();
                        break;
                    case"7":
                        ShowStats();
                        break;
                    case "8":
                        Console.WriteLine("GoodBye!");
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid Choice");
                        break;

                }
                if (running)
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }
        static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("====Welcome to TodoManager====");
            Console.WriteLine("1=Add task");
            Console.WriteLine("2=View task");
            Console.WriteLine("3=Delete task");
            Console.WriteLine("4=Mark task as done");
            Console.WriteLine("5=View pending tasks (by priority)");
            Console.WriteLine("6=View overdue tasks");
            Console.WriteLine("7=View task statistics");
            Console.WriteLine("8=Exit");
        }
        static void AddTask()
        {

            Console.Write("Enter task title :");
            string? title = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(title))
            {

                Console.WriteLine("Title can't be empty.");
                return;

            }
            Console.Write("Priority (1=Low , 2=Medium , 3=High):");
            string? p = Console.ReadLine();

            TaskPriority priority = TaskPriority.Low;

            switch (p)
            {
                case "2":
                     priority = TaskPriority.Medium;
                    break;
                case "3":
                     priority = TaskPriority.High;
                    break;
                    

            }
            Console.Write("Due date and time (yyyy-MM-dd HH:mm, or Enter to skip): ");
            string? d = Console.ReadLine();

            DateTime? dueDate = null;

            if (!string.IsNullOrWhiteSpace(d))
            {
                if (DateTime.TryParseExact(d, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime parsed))
                {
                    if (parsed < DateTime.Now)
                    {
                        Console.WriteLine("Due time can't be in the past. Skipping due date.");
                    }
                    else
                    {
                        dueDate = parsed;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid format. Example: 2026-11-01 14:30. Skipping due date.");
                }
            }
            service.Add(title, priority, dueDate);
            Console.WriteLine("Task added");
            ShowServiceError();
        }
        static void ShowTask()
        {
           
            var all = service.GetAll();

            if (all.Count == 0)
            {
                Console.WriteLine("No tasks yet.");
                return;
            }
            foreach (TaskItem t in all)
            {
                string status = t.IsDone ? "[*]" : "[ ]";
                string due = t.DueDate?.ToString("yyyy-MM-dd HH:mm") ?? "-";
                Console.WriteLine($"{t.Id}-{status}-{t.Title}-{t.Priority}-{due}");
            }

        }
        static void DeleteTask()
        {
            Console.Write("Enter task id to delete: ");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out int id))
            {
                Console.WriteLine("Please enter a valid number.");
                return;
            }
            if (!service.Delete(id))
            {
                Console.WriteLine("Task not found!");
                return;
            }

            Console.WriteLine("Task deleted.");
            ShowServiceError();

        }
        static void MarkTaskDone()
        {
            Console.Write("Enter task id to mark as done: ");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out int id))
            {
                Console.WriteLine("Please enter a valid number");
                return;
            }

            if (!service.MarkDone(id))
            {
                Console.WriteLine("Task not found!");
                return;
            }

            Console.WriteLine("Task marked as done.");
            ShowServiceError();

        }
        static void ShowPendingTasks()
        {
            var pending = service.GetPending();
            
            if (pending.Count == 0)
            {

                Console.WriteLine("No pending tasks.");
                return;
            
            }
            Console.WriteLine("--- Pending tasks ---");
            foreach (TaskItem t in pending)
            {
                string due = t.DueDate?.ToString("yyyy-MM-dd HH:mm") ?? "-";
                Console.WriteLine($"{t.Id}-{t.Title}-{t.Priority}-{due}");
            }
        }
        static void ViewOverdueTasks()
        {
            var overdue = service.GetOverdue();
            if (overdue.Count == 0)
            {
                Console.WriteLine("No overdue tasks.");
                return;
            }
            Console.WriteLine("--- Overdue tasks ---");
            foreach (TaskItem t in overdue)
            {
                string due = t.DueDate?.ToString("yyyy-MM-dd HH:mm") ?? "-";
                Console.WriteLine($"{t.Id}-{t.Title}-{t.Priority}-{due}");
            }
        }
        static void ShowServiceError()
        {
            if (!string.IsNullOrEmpty(service.LastError))
            {
                Console.WriteLine($"Error: {service.LastError}");
            }
        }
        static void ShowStats()
        {
            var stats = service.GetStatas();
            Console.WriteLine("--- Task Statistics ---");
            Console.WriteLine($"Total tasks: {stats.Total}");
            Console.WriteLine($"Pending tasks: {stats.Pending}");
            Console.WriteLine($"Overdue tasks: {stats.Overdue}");
            Console.WriteLine($"Tasks Done:{stats.Done}/{stats.Total}");
        }
    }
}
