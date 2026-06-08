namespace TaskFlow.Services.Models;

public class ProjectBoardViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int TasksCount { get; set; }
}