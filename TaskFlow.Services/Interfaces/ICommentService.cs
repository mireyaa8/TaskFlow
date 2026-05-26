using TaskFlow.Services.Models;

namespace TaskFlow.Services.Interfaces;

public interface ICommentService
{
    Task AddAsync(CommentInputModel model, string authorId);
    Task DeleteAsync(int id, string userId, bool isAdmin = false);
}
