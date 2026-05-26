namespace TaskFlow.Services.Models;

public class ProjectViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string OwnerName { get; set; } = null!;
    public int BoardsCount { get; set; }
}
