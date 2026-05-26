using TaskFlow.Services.Models;

namespace TaskFlow.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectViewModel>> GetMineAsync(string userId);
    Task<ProjectViewModel?> GetByIdAsync(int id, string userId, bool isAdmin = false);
    Task<int> CreateAsync(ProjectInputModel model, string ownerId);
    Task EditAsync(int id, ProjectInputModel model, string userId, bool isAdmin = false);
    Task DeleteAsync(int id, string userId, bool isAdmin = false);
    Task<bool> UserHasAccessAsync(int projectId, string userId);
}
