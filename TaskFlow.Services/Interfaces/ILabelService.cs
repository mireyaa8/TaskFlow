using TaskFlow.Services.Models;

namespace TaskFlow.Services.Interfaces;

public interface ILabelService
{
    Task<IEnumerable<LabelViewModel>> GetAllAsync();

    Task<LabelInputModel?> GetForEditAsync(int id);

    Task<int> CreateAsync(LabelInputModel model);

    Task<bool> EditAsync(int id, LabelInputModel model);

    Task<bool> DeleteAsync(int id);
}