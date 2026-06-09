using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Services.Interfaces;
using TaskFlow.Services.Models;

namespace TaskFlow.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext dbContext;

    public DashboardService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<DashboardViewModel> GetDashboardAsync(string userId)
    {
        var accessibleProjects = this.dbContext.Projects
            .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId));

        var accessibleProjectIds = accessibleProjects.Select(p => p.Id);

        var accessibleBoards = this.dbContext.Boards
            .Where(b => accessibleProjectIds.Contains(b.ProjectId));

        var myTasks = this.dbContext.TaskItems
            .Where(t => t.AssigneeId == userId);

        return new DashboardViewModel
        {
            ProjectsCount = await accessibleProjects.CountAsync(),
            BoardsCount = await accessibleBoards.CountAsync(),
            MyTasksCount = await myTasks.CountAsync(),
            ToDoTasksCount = await myTasks.CountAsync(t => t.Status == "To Do"),
            InProgressTasksCount = await myTasks.CountAsync(t => t.Status == "In Progress"),
            DoneTasksCount = await myTasks.CountAsync(t => t.Status == "Done"),

            RecentTasks = await myTasks
                .OrderByDescending(t => t.CreatedOn)
                .Take(5)
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
                        : ((t.Assignee.FirstName ?? string.Empty) + " " + (t.Assignee.LastName ?? string.Empty)).Trim()
                })
                .ToListAsync()
        };
    }
}