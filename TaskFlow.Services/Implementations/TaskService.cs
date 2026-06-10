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
    public async Task<IEnumerable<BoardSelectViewModel>> GetAvailableBoardsAsync(string userId, bool isAdmin = false)
    {
        return await this.dbContext.Boards
            .Where(b =>
                isAdmin ||
                b.Project.OwnerId == userId ||
                b.Project.Members.Any(m => m.UserId == userId))
            .OrderBy(b => b.Project.Name)
            .ThenBy(b => b.Name)
            .Select(b => new BoardSelectViewModel
            {
                Id = b.Id,
                Name = b.Name,
                ProjectName = b.Project.Name
            })
            .ToListAsync();
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
        var query = this.dbContext.TaskItems
            .Where(t => t.Board.Project.OwnerId == userId ||
                        t.Board.Project.Members.Any(m => m.UserId == userId));

        return await this.MapTasks(query).ToListAsync();
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

        var model = await this.dbContext.TaskItems
            .Where(t => t.Id == id)
            .Select(t => new TaskViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                DueDate = t.DueDate,
                BoardId = t.BoardId,
                AssigneeName = t.Assignee == null
                    ? null
                    : ((t.Assignee.FirstName ?? string.Empty) + " " + (t.Assignee.LastName ?? string.Empty)).Trim(),

                Comments = t.Comments
                    .OrderByDescending(c => c.CreatedOn)
                    .Select(c => new CommentViewModel
                    {
                        Id = c.Id,
                        Content = c.Content,
                        AuthorId = c.AuthorId,
                        AuthorName = c.Author.UserName ?? "Unknown",
                        CreatedOn = c.CreatedOn
                    })
                    .ToList(),

                Labels = t.TaskLabels
                    .OrderBy(tl => tl.Label.Name)
                    .Select(tl => new LabelViewModel
                    {
                        Id = tl.Label.Id,
                        Name = tl.Label.Name,
                        Color = tl.Label.Color
                    })
                    .ToList(),

                NewComment = new CommentInputModel
                {
                    TaskItemId = t.Id
                }
            })
            .FirstOrDefaultAsync();

        if (model == null)
        {
            return null;
        }

        var usedLabelIds = model.Labels.Select(l => l.Id).ToList();

        model.AvailableLabels = await this.dbContext.Labels
            .Where(l => !usedLabelIds.Contains(l.Id))
            .OrderBy(l => l.Name)
            .Select(l => new LabelViewModel
            {
                Id = l.Id,
                Name = l.Name,
                Color = l.Color
            })
            .ToListAsync();

        return model;
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
        var task = await this.dbContext.TaskItems
            .Include(t => t.Board)
            .FirstOrDefaultAsync(t => t.Id == id);

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
        var task = await this.dbContext.TaskItems
            .Include(t => t.Board)
            .FirstOrDefaultAsync(t => t.Id == id);

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

        var task = await this.dbContext.TaskItems
            .Include(t => t.Board)
            .FirstOrDefaultAsync(t => t.Id == id);

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

    public async Task AddLabelAsync(int taskId, int labelId, string userId, bool isAdmin = false)
    {
        var task = await this.dbContext.TaskItems
            .Include(t => t.Board)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
        {
            throw new InvalidOperationException("Task not found.");
        }

        if (!isAdmin && !await this.projectService.UserHasAccessAsync(task.Board.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You do not have access to this task.");
        }

        var labelExists = await this.dbContext.Labels.AnyAsync(l => l.Id == labelId);

        if (!labelExists)
        {
            throw new InvalidOperationException("Label not found.");
        }

        var alreadyAdded = await this.dbContext.TaskLabels
            .AnyAsync(tl => tl.TaskItemId == taskId && tl.LabelId == labelId);

        if (alreadyAdded)
        {
            return;
        }

        this.dbContext.TaskLabels.Add(new TaskLabel
        {
            TaskItemId = taskId,
            LabelId = labelId
        });

        await this.dbContext.SaveChangesAsync();
    }

    public async Task RemoveLabelAsync(int taskId, int labelId, string userId, bool isAdmin = false)
    {
        var task = await this.dbContext.TaskItems
            .Include(t => t.Board)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
        {
            throw new InvalidOperationException("Task not found.");
        }

        if (!isAdmin && !await this.projectService.UserHasAccessAsync(task.Board.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You do not have access to this task.");
        }

        var taskLabel = await this.dbContext.TaskLabels
            .FirstOrDefaultAsync(tl => tl.TaskItemId == taskId && tl.LabelId == labelId);

        if (taskLabel == null)
        {
            return;
        }

        this.dbContext.TaskLabels.Remove(taskLabel);

        await this.dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<TaskViewModel>> SearchAsync(string keyword, string userId)
    {
        keyword = keyword?.Trim() ?? string.Empty;

        var query = this.dbContext.TaskItems
            .Where(t => t.Board.Project.OwnerId == userId ||
                        t.Board.Project.Members.Any(m => m.UserId == userId));

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
            AssigneeName = t.Assignee == null
                ? null
                : ((t.Assignee.FirstName ?? string.Empty) + " " + (t.Assignee.LastName ?? string.Empty)).Trim(),

            Comments = t.Comments
                .OrderByDescending(c => c.CreatedOn)
                .Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    AuthorId = c.AuthorId,
                    AuthorName = c.Author.UserName ?? "Unknown",
                    CreatedOn = c.CreatedOn
                })
                .ToList(),

            Labels = t.TaskLabels
                .OrderBy(tl => tl.Label.Name)
                .Select(tl => new LabelViewModel
                {
                    Id = tl.Label.Id,
                    Name = tl.Label.Name,
                    Color = tl.Label.Color
                })
                .ToList(),

            NewComment = new CommentInputModel
            {
                TaskItemId = t.Id
            }
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