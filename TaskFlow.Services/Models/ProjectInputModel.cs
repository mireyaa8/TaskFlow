using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Services.Models;

public class ProjectInputModel
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(1000, MinimumLength = 10)]
    public string Description { get; set; } = null!;
}
