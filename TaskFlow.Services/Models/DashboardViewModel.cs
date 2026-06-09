namespace TaskFlow.Services.Models;

public class DashboardViewModel
{
    public int ProjectsCount { get; set; }

    public int BoardsCount { get; set; }

    public int MyTasksCount { get; set; }

    public int ToDoTasksCount { get; set; }

    public int InProgressTasksCount { get; set; }

    public int DoneTasksCount { get; set; }

    public IEnumerable<TaskViewModel> RecentTasks { get; set; } = new HashSet<TaskViewModel>();
}