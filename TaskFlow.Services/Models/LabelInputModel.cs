using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Services.Models;

public class LabelInputModel
{
    [Required]
    [StringLength(30, MinimumLength = 2)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string Color { get; set; } = "secondary";
}
