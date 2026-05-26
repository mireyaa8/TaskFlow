using TaskFlow.Services.Models;

namespace TaskFlow.Services.Interfaces;

public interface ILabelService
{
    Task<IEnumerable<LabelViewModel>> GetAllAsync();
    Task<int> CreateAsync(LabelInputModel model);
    Task DeleteAsync(int id);
}
