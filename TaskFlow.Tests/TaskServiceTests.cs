using NUnit.Framework;
using TaskFlow.Data.Models;
using TaskFlow.Services.Implementations;
using TaskFlow.Services.Models;

namespace TaskFlow.Tests;

public class TaskServiceTests
{
    [Test]
    public async Task ChangeStatusAsync_ShouldUpdateStatus_WhenUserHasAccess()
    {
        using var db = TestDbFactory.Create();
        db.Users.Add(new ApplicationUser { Id = "user-1", UserName = "u@test.com", FirstName = "A", LastName = "B" });
        db.Projects.Add(new Project { Id = 1, Name = "P", Description = "Description", OwnerId = "user-1" });
        db.ProjectMembers.Add(new ProjectMember { ProjectId = 1, UserId = "user-1", Role = "Owner" });
        db.Boards.Add(new Board { Id = 1, Name = "Board", ProjectId = 1 });
        db.TaskItems.Add(new TaskItem { Id = 1, Title = "Task", Description = "Task description", BoardId = 1, Status = "To Do", Priority = "Medium" });
        await db.SaveChangesAsync();

        var projectService = new ProjectService(db);
        var taskService = new TaskService(db, projectService);

        await taskService.ChangeStatusAsync(1, "Done", "user-1");

        Assert.That(db.TaskItems.First().Status, Is.EqualTo("Done"));
    }

    [Test]
    public void ChangeStatusAsync_ShouldThrow_WhenStatusIsInvalid()
    {
        using var db = TestDbFactory.Create();
        var taskService = new TaskService(db, new ProjectService(db));

        Assert.ThrowsAsync<InvalidOperationException>(() => taskService.ChangeStatusAsync(1, "Invalid", "user-1"));
    }
}
