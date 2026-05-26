using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Services.Models;

public class BoardInputModel
{
    [Required]
    [StringLength(80, MinimumLength = 2)]
    public string Name { get; set; } = null!;

    [Required]
    public int ProjectId { get; set; }
}
