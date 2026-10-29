using System.Data;
using System.Threading.Tasks;
using TodoManager;
using Xunit;

namespace TodoManager.Tests
{
    public class TaskServiceTests : IDisposable
    {
        private readonly string _path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");

        [Fact]
        public void Add_AddsTaskToList()
        {
            var service = new TaskService(_path);

            service.Add("Buy milk", TaskPriority.High);

            Assert.Single(service.GetAll());
        }

        [Fact]
        public void Delete_ReturnsFalse_WhenIdDoesNotExist()
        {
            var service = new TaskService(_path);
            var result = service.Delete(99);
            Assert.False(result);
        }

        [Fact]
        public void MarkDone_SetsIsDone()
        {
            var service = new TaskService(_path);
            var task = service.Add("Buy milk", TaskPriority.High);
            service.MarkDone(task.Id);
            Assert.True(task.IsDone);
        }

        [Fact]
        public void Delete_RemovesTask_WhenIdExists()
        {
            var service = new TaskService(_path);
            var task = service.Add("Buy milk", TaskPriority.High);
            var result = service.Delete(task.Id);
            Assert.True(result);
            Assert.Empty(service.GetAll());
        }

        [Fact]
        public void Tasks_ArePersisted_BetweenInstances()
        {
            var first = new TaskService(_path);
            first.Add("Buy milk", TaskPriority.High);

            var second = new TaskService(_path);  

            Assert.Single(second.GetAll());
            Assert.Equal("Buy milk", second.GetAll()[0].Title);
        }

        [Fact]
        public void Load_SetsLastError_WhenFileIsCorrupted()
        {
            File.WriteAllText(_path, "abc");

            var service = new TaskService(_path);

            Assert.NotNull(service.LastError);
            Assert.Empty(service.GetAll());
        }
        [Fact]
        public void Add_StoresDueDate()
        {
            var service = new TaskService(_path);
            var date = new DateTime(2030, 1, 1, 14, 30, 0);

            var task = service.Add("Buy milk", TaskPriority.High, date);

            Assert.Equal(date, task.DueDate);
        }

        [Fact] 
        public void GetOverdue_ReturnsOnlyPastDueUnfinishedTasks()
        {
            var service = new TaskService(_path);
            
            service.Add("Overdue", TaskPriority.High, DateTime.Today.AddDays(-1));
            service.Add("Future", TaskPriority.High, DateTime.Today.AddDays(1));
            service.Add("No date", TaskPriority.High);

            var done = service.Add("Overdue but done", TaskPriority.High, DateTime.Today.AddDays(-5));
            service.MarkDone(done.Id);

            var overdue = service.GetOverdue();

            Assert.Single(overdue);
            Assert.Equal("Overdue", overdue[0].Title);
        }

        [Fact]
        public void GetStats_ReturnsCorrectCounts()
        {
            var service = new TaskService(_path);
            service.Add("A", TaskPriority.High, DateTime.Today.AddDays(-1));
            var b = service.Add("B", TaskPriority.Low);
            service.Add("C", TaskPriority.Medium);
            service.MarkDone(b.Id);

            var stats = service.GetStatas();

            Assert.Equal(new TaskStatas(3, 1, 2, 1), stats);
        }

        [Fact]
        public void DueDate_IsPersisted_BetweenInstances()
        {
            var date = new DateTime(2030, 1, 1, 14, 30, 0);

            var first = new TaskService(_path);
            first.Add("Buy milk", TaskPriority.High, date);
            
            var second = new TaskService(_path);

            Assert.Equal(date, second.GetAll()[0].DueDate);
        }

        [Fact]
        public void Save_ClearsLastError_AfterSuccessfulSave()
        {
            File.WriteAllText(_path, "abc");
            var service = new TaskService(_path);
            Assert.NotNull(service.LastError);
            service.Add("Buy milk", TaskPriority.High);
            Assert.Null(service.LastError);
        }
        [Fact]
        public void GetOverdue_UsesTimeOfDay_NotJustDate()
        {
            var service = new TaskService(_path);

            service.Add("One hour ago", TaskPriority.High, DateTime.Now.AddHours(-1));
            service.Add("In one hour", TaskPriority.High, DateTime.Now.AddHours(1));

            var overdue = service.GetOverdue();

            Assert.Single(overdue);
            Assert.Equal("One hour ago", overdue[0].Title);
        }
        public void Dispose()
        {
            if (File.Exists(_path))

            {
                File.Delete(_path);
            }
        }
    }
}
