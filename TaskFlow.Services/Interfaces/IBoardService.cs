using TaskFlow.Services.Models;

namespace TaskFlow.Services.Interfaces;

public interface IBoardService
{
    Task<IEnumerable<BoardViewModel>> GetByProjectAsync(int projectId, string userId);
    Task<int> CreateAsync(BoardInputModel model, string userId);
    Task<BoardViewModel?> GetByIdAsync(int id, string userId);
}
