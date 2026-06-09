using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TaskFlow.Data.Models;
using TaskFlow.Services.Implementations;
using TaskFlow.Services.Models;

namespace TaskFlow.Tests;

public class BoardServiceTests
{
    [Test]
    public async Task CreateAsync_ShouldCreateBoard_WhenUserHasAccess()
    {
        using var db = TestDbFactory.Create();

        db.Projects.Add(new Project
        {
            Id = 1,
            Name = "Project",
            Description = "Project description",
            OwnerId = "owner"
        });

        await db.SaveChangesAsync();

        var projectService = new ProjectService(db);
        var boardService = new BoardService(db, projectService);

        var boardId = await boardService.CreateAsync(new BoardInputModel
        {
            Name = "Development Board",
            ProjectId = 1
        }, "owner");

        var board = await db.Boards.FindAsync(boardId);

        Assert.That(board, Is.Not.Null);
        Assert.That(board!.Name, Is.EqualTo("Development Board"));
        Assert.That(board.ProjectId, Is.EqualTo(1));
    }

    [Test]
    public void CreateAsync_ShouldThrow_WhenUserHasNoAccess()
    {
        using var db = TestDbFactory.Create();

        db.Projects.Add(new Project
        {
            Id = 1,
            Name = "Project",
            Description = "Project description",
            OwnerId = "owner"
        });

        db.SaveChanges();

        var projectService = new ProjectService(db);
        var boardService = new BoardService(db, projectService);

        Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            boardService.CreateAsync(new BoardInputModel
            {
                Name = "Forbidden Board",
                ProjectId = 1
            }, "other-user"));
    }

    [Test]
    public async Task GetByProjectAsync_ShouldReturnProjectBoards()
    {
        using var db = TestDbFactory.Create();

        db.Projects.Add(new Project
        {
            Id = 1,
            Name = "Project",
            Description = "Project description",
            OwnerId = "owner"
        });

        db.Boards.AddRange(
            new Board { Id = 1, Name = "Board One", ProjectId = 1 },
            new Board { Id = 2, Name = "Board Two", ProjectId = 1 });

        await db.SaveChangesAsync();

        var projectService = new ProjectService(db);
        var boardService = new BoardService(db, projectService);

        var boards = await boardService.GetByProjectAsync(1, "owner");

        Assert.That(boards.Count(), Is.EqualTo(2));
    }
}