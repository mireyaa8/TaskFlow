namespace TaskFlow.Services.Models;

public class ProjectDetailsViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string OwnerId { get; set; } = null!;

    public string OwnerName { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public IEnumerable<ProjectBoardViewModel> Boards { get; set; }
        = new HashSet<ProjectBoardViewModel>();
}