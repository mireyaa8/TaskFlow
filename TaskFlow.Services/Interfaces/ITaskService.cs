using TaskFlow.Services.Models;

namespace TaskFlow.Services.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskViewModel>> GetByBoardAsync(int boardId, string userId);
    Task<IEnumerable<TaskViewModel>> GetMineAsync(string userId);
    Task<TaskViewModel?> GetByIdAsync(int id, string userId, bool isAdmin = false);
    Task<int> CreateAsync(TaskInputModel model, string userId);
    Task EditAsync(int id, TaskInputModel model, string userId, bool isAdmin = false);
    Task DeleteAsync(int id, string userId, bool isAdmin = false);
    Task ChangeStatusAsync(int id, string status, string userId, bool isAdmin = false);
    Task<IEnumerable<TaskViewModel>> SearchAsync(string keyword, string userId);
}
