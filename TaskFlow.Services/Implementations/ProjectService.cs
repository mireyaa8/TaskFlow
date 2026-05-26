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
            .Where(p => !p.IsDeleted && (p.OwnerId == userId || p.Members.Any(m => m.UserId == userId)))
            .Select(p => new ProjectViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                OwnerName = p.Owner.FirstName + " " + p.Owner.LastName,
                BoardsCount = p.Boards.Count
            })
            .ToListAsync();
    }

    public async Task<ProjectViewModel?> GetByIdAsync(int id, string userId, bool isAdmin = false)
    {
        if (!isAdmin && !await this.UserHasAccessAsync(id, userId))
        {
            return null;
        }

        return await this.dbContext.Projects
            .Where(p => p.Id == id && !p.IsDeleted)
            .Select(p => new ProjectViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                OwnerName = p.Owner.FirstName + " " + p.Owner.LastName,
                BoardsCount = p.Boards.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(ProjectInputModel model, string ownerId)
    {
        var project = new Project
        {
            Name = model.Name,
            Description = model.Description,
            OwnerId = ownerId
        };

        project.Members.Add(new ProjectMember
        {
            UserId = ownerId,
            Role = "Owner"
        });

        this.dbContext.Projects.Add(project);
        await this.dbContext.SaveChangesAsync();
        return project.Id;
    }

    public async Task EditAsync(int id, ProjectInputModel model, string userId, bool isAdmin = false)
    {
        var project = await this.dbContext.Projects.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (project == null)
        {
            throw new InvalidOperationException("Project not found.");
        }

        if (!isAdmin && project.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("Only the owner can edit this project.");
        }

        project.Name = model.Name;
        project.Description = model.Description;
        await this.dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string userId, bool isAdmin = false)
    {
        var project = await this.dbContext.Projects.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (project == null)
        {
            throw new InvalidOperationException("Project not found.");
        }

        if (!isAdmin && project.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("Only the owner can delete this project.");
        }

        project.IsDeleted = true;
        await this.dbContext.SaveChangesAsync();
    }

    public async Task<bool> UserHasAccessAsync(int projectId, string userId)
    {
        return await this.dbContext.Projects
            .AnyAsync(p => p.Id == projectId && !p.IsDeleted && (p.OwnerId == userId || p.Members.Any(m => m.UserId == userId)));
    }
}
