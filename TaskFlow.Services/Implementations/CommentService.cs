using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Data.Models;
using TaskFlow.Services.Interfaces;
using TaskFlow.Services.Models;

namespace TaskFlow.Services.Implementations;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext dbContext;
    private readonly IProjectService projectService;

    public CommentService(
        ApplicationDbContext dbContext,
        IProjectService projectService)
    {
        this.dbContext = dbContext;
        this.projectService = projectService;
    }

    public async Task<bool> CreateAsync(CommentInputModel model, string userId)
    {
        var task = await this.dbContext.TaskItems
            .Include(t => t.Board)
            .FirstOrDefaultAsync(t => t.Id == model.TaskItemId);

        if (task == null)
        {
            return false;
        }

        var hasAccess = await this.projectService.UserHasAccessAsync(task.Board.ProjectId, userId);

        if (!hasAccess)
        {
            return false;
        }

        var comment = new Comment
        {
            Content = model.Content,
            TaskItemId = model.TaskItemId,
            AuthorId = userId,
            CreatedOn = DateTime.UtcNow
        };

        await this.dbContext.Comments.AddAsync(comment);
        await this.dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int commentId, string userId, bool isAdmin)
    {
        var comment = await this.dbContext.Comments
            .Include(c => c.TaskItem)
            .ThenInclude(t => t.Board)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null)
        {
            return false;
        }

        var hasAccess = await this.projectService.UserHasAccessAsync(comment.TaskItem.Board.ProjectId, userId);

        if (!hasAccess)
        {
            return false;
        }

        if (!isAdmin && comment.AuthorId != userId)
        {
            return false;
        }

        this.dbContext.Comments.Remove(comment);
        await this.dbContext.SaveChangesAsync();

        return true;
    }
}