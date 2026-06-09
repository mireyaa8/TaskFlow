using TaskFlow.Services.Models;

namespace TaskFlow.Services.Interfaces;

public interface ICommentService
{
    Task<bool> CreateAsync(CommentInputModel model, string userId);

    Task<bool> DeleteAsync(int commentId, string userId, bool isAdmin);
}