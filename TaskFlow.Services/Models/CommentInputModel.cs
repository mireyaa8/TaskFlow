using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Services.Models;

public class CommentInputModel
{
    [Required]
    public int TaskItemId { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 2)]
    public string Content { get; set; } = null!;
}
