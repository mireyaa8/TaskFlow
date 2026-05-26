namespace TaskFlow.Data.Models;

public class Comment
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public int TaskItemId { get; set; }

    public TaskItem TaskItem { get; set; } = null!;

    public string AuthorId { get; set; } = null!;

    public ApplicationUser Author { get; set; } = null!;
}
