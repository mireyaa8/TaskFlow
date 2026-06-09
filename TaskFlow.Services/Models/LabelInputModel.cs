using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Services.Models;

public class LabelInputModel
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = null!;

    [Required]
    public string Color { get; set; } = "primary";
}