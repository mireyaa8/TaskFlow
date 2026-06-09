using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TaskFlow.Data.Models;
using TaskFlow.Services.Implementations;
using TaskFlow.Services.Models;

namespace TaskFlow.Tests;

public class CommentServiceTests
{
    [Test]
    public async Task CreateAsync_ShouldCreateComment_WhenUserHasAccess()
    {
        using var db = TestDbFactory.Create();

        db.Projects.Add(new Project
        {
            Id = 1,
            Name = "Project",
            Description = "Project description",
            OwnerId = "owner"
        });

        db.Boards.Add(new Board
        {
            Id = 1,
            Name = "Board",
            ProjectId = 1
        });

        db.TaskItems.Add(new TaskItem
        {
            Id = 1,
            Title = "Task",
            Description = "Task description",
            Status = "To Do",
            Priority = "High",
            BoardId = 1
        });

        await db.SaveChangesAsync();

        var projectService = new ProjectService(db);
        var commentService = new CommentService(db, projectService);

        var result = await commentService.CreateAsync(new CommentInputModel
        {
            TaskItemId = 1,
            Content = "This is a test comment."
        }, "owner");

        var comment = await db.Comments.FirstOrDefaultAsync();

        Assert.That(result, Is.True);
        Assert.That(comment, Is.Not.Null);
        Assert.That(comment!.Content, Is.EqualTo("This is a test comment."));
    }

    [Test]
    public async Task CreateAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
    {
        using var db = TestDbFactory.Create();

        var projectService = new ProjectService(db);
        var commentService = new CommentService(db, projectService);

        var result = await commentService.CreateAsync(new CommentInputModel
        {
            TaskItemId = 999,
            Content = "Invalid comment"
        }, "owner");

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteAsync_ShouldDeleteComment_WhenAuthorDeletesOwnComment()
    {
        using var db = TestDbFactory.Create();

        db.Projects.Add(new Project
        {
            Id = 1,
            Name = "Project",
            Description = "Project description",
            OwnerId = "owner"
        });

        db.Boards.Add(new Board
        {
            Id = 1,
            Name = "Board",
            ProjectId = 1
        });

        db.TaskItems.Add(new TaskItem
        {
            Id = 1,
            Title = "Task",
            Description = "Task description",
            Status = "To Do",
            Priority = "High",
            BoardId = 1
        });

        db.Comments.Add(new Comment
        {
            Id = 1,
            Content = "Delete me",
            TaskItemId = 1,
            AuthorId = "owner",
            CreatedOn = DateTime.UtcNow
        });

        await db.SaveChangesAsync();

        var projectService = new ProjectService(db);
        var commentService = new CommentService(db, projectService);

        var result = await commentService.DeleteAsync(1, "owner", false);

        Assert.That(result, Is.True);
        Assert.That(await db.Comments.AnyAsync(), Is.False);
    }

    [Test]
    public async Task DeleteAsync_ShouldReturnFalse_WhenUserIsNotAuthorOrAdmin()
    {
        using var db = TestDbFactory.Create();

        db.Projects.Add(new Project
        {
            Id = 1,
            Name = "Project",
            Description = "Project description",
            OwnerId = "owner"
        });

        db.Boards.Add(new Board
        {
            Id = 1,
            Name = "Board",
            ProjectId = 1
        });

        db.TaskItems.Add(new TaskItem
        {
            Id = 1,
            Title = "Task",
            Description = "Task description",
            Status = "To Do",
            Priority = "High",
            BoardId = 1
        });

        db.Comments.Add(new Comment
        {
            Id = 1,
            Content = "Protected comment",
            TaskItemId = 1,
            AuthorId = "owner",
            CreatedOn = DateTime.UtcNow
        });

        await db.SaveChangesAsync();

        var projectService = new ProjectService(db);
        var commentService = new CommentService(db, projectService);

        var result = await commentService.DeleteAsync(1, "other-user", false);

        Assert.That(result, Is.False);
        Assert.That(await db.Comments.AnyAsync(), Is.True);
    }
}