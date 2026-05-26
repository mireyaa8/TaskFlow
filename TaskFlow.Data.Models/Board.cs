namespace TaskFlow.Data.Models;

public class Board
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int ProjectId { get; set; }

    public Project Project { get; set; } = null!;

    public ICollection<TaskItem> Tasks { get; set; } = new HashSet<TaskItem>();
}
