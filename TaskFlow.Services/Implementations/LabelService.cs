using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Data.Models;
using TaskFlow.Services.Interfaces;
using TaskFlow.Services.Models;

namespace TaskFlow.Services.Implementations;

public class LabelService : ILabelService
{
    private readonly ApplicationDbContext dbContext;

    public LabelService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<LabelViewModel>> GetAllAsync()
    {
        return await this.dbContext.Labels
            .Select(l => new LabelViewModel { Id = l.Id, Name = l.Name, Color = l.Color })
            .ToListAsync();
    }

    public async Task<int> CreateAsync(LabelInputModel model)
    {
        var label = new Label { Name = model.Name, Color = model.Color };
        this.dbContext.Labels.Add(label);
        await this.dbContext.SaveChangesAsync();
        return label.Id;
    }

    public async Task DeleteAsync(int id)
    {
        var label = await this.dbContext.Labels.FindAsync(id);
        if (label == null)
        {
            throw new InvalidOperationException("Label not found.");
        }

        this.dbContext.Labels.Remove(label);
        await this.dbContext.SaveChangesAsync();
    }
}
