using Microsoft.AspNetCore.Identity;

namespace TaskFlow.Data.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? ProfilePictureUrl { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public ICollection<Project> OwnedProjects { get; set; } = new HashSet<Project>();

    public ICollection<ProjectMember> ProjectMemberships { get; set; } = new HashSet<ProjectMember>();

    public ICollection<TaskItem> AssignedTasks { get; set; } = new HashSet<TaskItem>();

    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
}
