using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Data.Models;
using TaskFlow.Services.Interfaces;
using TaskFlow.Services.Models;

namespace TaskFlow.Services.Implementations;

public class TaskService : ITaskService
{
    private static readonly string[] ValidStatuses = { "To Do", "In Progress", "Done" };
    private static readonly string[] ValidPriorities = { "Low", "Medium", "High" };

    private readonly ApplicationDbContext dbContext;
    private readonly IProjectService projectService;

    public TaskService(ApplicationDbContext dbContext, IProjectService projectService)
    {
        this.dbContext = dbContext;
        this.projectService = projectService;
    }

    public async Task<IEnumerable<TaskViewModel>> GetByBoardAsync(int boardId, string userId)
    {
        var board = await this.dbContext.Boards.FirstOrDefaultAsync(b => b.Id == boardId);
        if (board == null || !await this.projectService.UserHasAccessAsync(board.ProjectId, userId))
        {
            return Enumerable.Empty<TaskViewModel>();
        }

        return await this.MapTasks(this.dbContext.TaskItems.Where(t => t.BoardId == boardId)).ToListAsync();
    }

    public async Task<IEnumerable<TaskViewModel>> GetMineAsync(string userId)
    {
        return await this.MapTasks(this.dbContext.TaskItems.Where(t => t.AssigneeId == userId)).ToListAsync();
    }

    public async Task<TaskViewModel?> GetByIdAsync(int id, string userId, bool isAdmin = false)
    {
        var task = await this.dbContext.TaskItems
            .Include(t => t.Board)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return null;
        }

        if (!isAdmin && !await this.projectService.UserHasAccessAsync(task.Board.ProjectId, userId))
        {
            return null;
        }

        return await this.MapTasks(this.dbContext.TaskItems.Where(t => t.Id == id)).FirstAsync();
    }

    public async Task<int> CreateAsync(TaskInputModel model, string userId)
    {
        await this.ValidateBoardAccessAsync(model.BoardId, userId);
        this.ValidateTaskFields(model.Status, model.Priority);

        var task = new TaskItem
        {
            Title = model.Title,
            Description = model.Description,
            BoardId = model.BoardId,
            Status = model.Status,
            Priority = model.Priority,
            DueDate = model.DueDate,
            AssigneeId = model.AssigneeId
        };

        this.dbContext.TaskItems.Add(task);
        await this.dbContext.SaveChangesAsync();
        return task.Id;
    }

    public async Task EditAsync(int id, TaskInputModel model, string userId, bool isAdmin = false)
    {
        var task = await this.dbContext.TaskItems.Include(t => t.Board).FirstOrDefaultAsync(t => t.Id == id);
        if (task == null)
        {
            throw new InvalidOperationException("Task not found.");
        }

        if (!isAdmin && !await this.projectService.UserHasAccessAsync(task.Board.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You do not have access to this task.");
        }

        this.ValidateTaskFields(model.Status, model.Priority);
        task.Title = model.Title;
        task.Description = model.Description;
        task.Status = model.Status;
        task.Priority = model.Priority;
        task.DueDate = model.DueDate;
        task.AssigneeId = model.AssigneeId;
        await this.dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string userId, bool isAdmin = false)
    {
        var task = await this.dbContext.TaskItems.Include(t => t.Board).FirstOrDefaultAsync(t => t.Id == id);
        if (task == null)
        {
            throw new InvalidOperationException("Task not found.");
        }

        if (!isAdmin && !await this.projectService.UserHasAccessAsync(task.Board.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You do not have access to this task.");
        }

        this.dbContext.TaskItems.Remove(task);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task ChangeStatusAsync(int id, string status, string userId, bool isAdmin = false)
    {
        if (!ValidStatuses.Contains(status))
        {
            throw new InvalidOperationException("Invalid status.");
        }

        var task = await this.dbContext.TaskItems.Include(t => t.Board).FirstOrDefaultAsync(t => t.Id == id);
        if (task == null)
        {
            throw new InvalidOperationException("Task not found.");
        }

        if (!isAdmin && !await this.projectService.UserHasAccessAsync(task.Board.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You do not have access to this task.");
        }

        task.Status = status;
        await this.dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<TaskViewModel>> SearchAsync(string keyword, string userId)
    {
        keyword = keyword?.Trim() ?? string.Empty;

        var query = this.dbContext.TaskItems
            .Where(t => t.Board.Project.OwnerId == userId || t.Board.Project.Members.Any(m => m.UserId == userId));

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(t => t.Title.Contains(keyword) || t.Description.Contains(keyword));
        }

        return await this.MapTasks(query).ToListAsync();
    }

    private IQueryable<TaskViewModel> MapTasks(IQueryable<TaskItem> query)
    {
        return query.Select(t => new TaskViewModel
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status,
            Priority = t.Priority,
            DueDate = t.DueDate,
            BoardId = t.BoardId,
            AssigneeName = t.Assignee == null ? null : t.Assignee.FirstName + " " + t.Assignee.LastName
        });
    }

    private async Task ValidateBoardAccessAsync(int boardId, string userId)
    {
        var board = await this.dbContext.Boards.FirstOrDefaultAsync(b => b.Id == boardId);
        if (board == null)
        {
            throw new InvalidOperationException("Board not found.");
        }

        if (!await this.projectService.UserHasAccessAsync(board.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You do not have access to this board.");
        }
    }

    private void ValidateTaskFields(string status, string priority)
    {
        if (!ValidStatuses.Contains(status))
        {
            throw new InvalidOperationException("Invalid status.");
        }

        if (!ValidPriorities.Contains(priority))
        {
            throw new InvalidOperationException("Invalid priority.");
        }
    }
}
