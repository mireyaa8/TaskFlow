namespace TaskFlow.Services.Models;

public class TaskViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string Priority { get; set; } = null!;
    public DateTime? DueDate { get; set; }
    public string? AssigneeName { get; set; }
    public int BoardId { get; set; }
}
