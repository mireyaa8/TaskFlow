namespace TaskFlow.Services.Models;

public class BoardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
}
