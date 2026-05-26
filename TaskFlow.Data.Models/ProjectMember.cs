namespace TaskFlow.Data.Models;

public class ProjectMember
{
    public int ProjectId { get; set; }

    public Project Project { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;

    public string Role { get; set; } = "Member";
}
