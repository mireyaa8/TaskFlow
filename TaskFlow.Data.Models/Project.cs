namespace TaskFlow.Data.Models;

public class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string OwnerId { get; set; } = null!;

    public ApplicationUser Owner { get; set; } = null!;

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; }

    public ICollection<Board> Boards { get; set; } = new HashSet<Board>();

    public ICollection<ProjectMember> Members { get; set; } = new HashSet<ProjectMember>();
}
