using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Data.Models;
using TaskFlow.Services.Interfaces;
using TaskFlow.Services.Models;

namespace TaskFlow.Services.Implementations;

public class ProjectService : IProjectService
{
    private readonly ApplicationDbContext dbContext;

    public ProjectService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<ProjectViewModel>> GetMineAsync(string userId)
    {
        return await this.dbContext.Projects
            .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId))
            .Select(p => new ProjectViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                OwnerName = p.Owner.UserName ?? "Unknown",
                BoardsCount = p.Boards.Count,
                CreatedOn = p.CreatedOn
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectViewModel>> GetAllAsync()
    {
        return await this.dbContext.Projects
            .Select(p => new ProjectViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                OwnerName = p.Owner.UserName ?? "Unknown",
                BoardsCount = p.Boards.Count,
                CreatedOn = p.CreatedOn
            })
            .ToListAsync();
    }

    public async Task<ProjectDetailsViewModel?> GetDetailsAsync(int projectId, string userId, bool isAdmin)
    {
        return await this.dbContext.Projects
            .Where(p =>
                p.Id == projectId &&
                (isAdmin || p.OwnerId == userId || p.Members.Any(m => m.UserId == userId)))
            .Select(p => new ProjectDetailsViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                OwnerId = p.OwnerId,
                OwnerName = p.Owner.UserName ?? "Unknown",
                CreatedOn = p.CreatedOn,
                Boards = p.Boards.Select(b => new ProjectBoardViewModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    TasksCount = b.Tasks.Count
                })
                .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(ProjectInputModel model, string userId)
    {
        var project = new Project
        {
            Name = model.Name,
            Description = model.Description,
            OwnerId = userId,
            CreatedOn = DateTime.UtcNow
        };

        await this.dbContext.Projects.AddAsync(project);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task<ProjectInputModel?> GetForEditAsync(int projectId, string userId, bool isAdmin)
    {
        return await this.dbContext.Projects
            .Where(p => p.Id == projectId && (isAdmin || p.OwnerId == userId))
            .Select(p => new ProjectInputModel
            {
                Name = p.Name,
                Description = p.Description
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> EditAsync(int projectId, ProjectInputModel model, string userId, bool isAdmin)
    {
        var project = await this.dbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId && (isAdmin || p.OwnerId == userId));

        if (project == null)
        {
            return false;
        }

        project.Name = model.Name;
        project.Description = model.Description;

        await this.dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int projectId, string userId, bool isAdmin)
    {
        var project = await this.dbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId && (isAdmin || p.OwnerId == userId));

        if (project == null)
        {
            return false;
        }

        this.dbContext.Projects.Remove(project);
        await this.dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UserHasAccessAsync(int projectId, string userId)
    {
        var isAdmin = await this.dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(
                this.dbContext.Roles,
                userRole => userRole.RoleId,
                role => role.Id,
                (userRole, role) => role.Name)
            .AnyAsync(roleName => roleName == "Administrator");

        if (isAdmin)
        {
            return true;
        }

        return await this.dbContext.Projects
            .AnyAsync(p =>
                p.Id == projectId &&
                (p.OwnerId == userId || p.Members.Any(m => m.UserId == userId)));
    }
}