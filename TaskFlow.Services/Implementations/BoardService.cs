using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Data.Models;
using TaskFlow.Services.Interfaces;
using TaskFlow.Services.Models;

namespace TaskFlow.Services.Implementations;

public class BoardService : IBoardService
{
    private readonly ApplicationDbContext dbContext;
    private readonly IProjectService projectService;

    public BoardService(ApplicationDbContext dbContext, IProjectService projectService)
    {
        this.dbContext = dbContext;
        this.projectService = projectService;
    }

    public async Task<IEnumerable<BoardViewModel>> GetByProjectAsync(int projectId, string userId)
    {
        if (!await this.projectService.UserHasAccessAsync(projectId, userId))
        {
            return Enumerable.Empty<BoardViewModel>();
        }

        return await this.dbContext.Boards
            .Where(b => b.ProjectId == projectId)
            .Select(b => new BoardViewModel
            {
                Id = b.Id,
                Name = b.Name,
                ProjectId = b.ProjectId,
                ProjectName = b.Project.Name
            })
            .ToListAsync();
    }

    public async Task<int> CreateAsync(BoardInputModel model, string userId)
    {
        if (!await this.projectService.UserHasAccessAsync(model.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You do not have access to this project.");
        }

        var board = new Board
        {
            Name = model.Name,
            ProjectId = model.ProjectId
        };

        this.dbContext.Boards.Add(board);
        await this.dbContext.SaveChangesAsync();
        return board.Id;
    }

    public async Task<BoardViewModel?> GetByIdAsync(int id, string userId)
    {
        var board = await this.dbContext.Boards
            .Include(b => b.Project)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (board == null || !await this.projectService.UserHasAccessAsync(board.ProjectId, userId))
        {
            return null;
        }

        return new BoardViewModel
        {
            Id = board.Id,
            Name = board.Name,
            ProjectId = board.ProjectId,
            ProjectName = board.Project.Name
        };
    }
}
