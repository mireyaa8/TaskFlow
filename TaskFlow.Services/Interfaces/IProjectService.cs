using TaskFlow.Services.Models;

namespace TaskFlow.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectViewModel>> GetMineAsync(string userId);

    Task<ProjectDetailsViewModel?> GetDetailsAsync(int projectId, string userId, bool isAdmin);

    Task CreateAsync(ProjectInputModel model, string userId);

    Task<ProjectInputModel?> GetForEditAsync(int projectId, string userId, bool isAdmin);

    Task<bool> EditAsync(int projectId, ProjectInputModel model, string userId, bool isAdmin);

    Task<bool> DeleteAsync(int projectId, string userId, bool isAdmin);

    Task<bool> UserHasAccessAsync(int projectId, string userId);
}