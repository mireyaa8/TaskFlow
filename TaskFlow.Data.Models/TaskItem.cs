namespace TaskFlow.Data.Models;

public class TaskItem
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime? DueDate { get; set; }

    public string Status { get; set; } = "To Do";

    public string Priority { get; set; } = "Medium";

    public int BoardId { get; set; }

    public Board Board { get; set; } = null!;

    public string? AssigneeId { get; set; }

    public ApplicationUser? Assignee { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    public ICollection<TaskLabel> TaskLabels { get; set; } = new HashSet<TaskLabel>();
}
