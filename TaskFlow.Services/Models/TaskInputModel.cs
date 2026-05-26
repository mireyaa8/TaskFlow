using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Services.Models;

public class TaskInputModel
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Description { get; set; } = null!;

    [Required]
    public int BoardId { get; set; }

    [Required]
    public string Status { get; set; } = "To Do";

    [Required]
    public string Priority { get; set; } = "Medium";

    public DateTime? DueDate { get; set; }

    public string? AssigneeId { get; set; }
}
