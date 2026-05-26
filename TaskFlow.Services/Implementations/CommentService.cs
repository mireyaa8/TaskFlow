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

    public CommentService(ApplicationDbContext dbContext, IProjectService projectService)
    {
        this.dbContext = dbContext;
        this.projectService = projectService;
    }

    public async Task AddAsync(CommentInputModel model, string authorId)
    {
        var task = await this.dbContext.TaskItems.Include(t => t.Board).FirstOrDefaultAsync(t => t.Id == model.TaskItemId);
        if (task == null)
        {
            throw new InvalidOperationException("Task not found.");
        }

        if (!await this.projectService.UserHasAccessAsync(task.Board.ProjectId, authorId))
        {
            throw new UnauthorizedAccessException("You do not have access to this task.");
        }

        this.dbContext.Comments.Add(new Comment
        {
            TaskItemId = model.TaskItemId,
            Content = model.Content,
            AuthorId = authorId
        });

        await this.dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string userId, bool isAdmin = false)
    {
        var comment = await this.dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null)
        {
            throw new InvalidOperationException("Comment not found.");
        }

        if (!isAdmin && comment.AuthorId != userId)
        {
            throw new UnauthorizedAccessException("Only the author can delete this comment.");
        }

        this.dbContext.Comments.Remove(comment);
        await this.dbContext.SaveChangesAsync();
    }
}
