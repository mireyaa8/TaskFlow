using NUnit.Framework;
using TaskFlow.Data.Models;
using TaskFlow.Services.Implementations;
using TaskFlow.Services.Models;

namespace TaskFlow.Tests;

public class ProjectServiceTests
{
    [Test]
    public async Task CreateAsync_ShouldCreateProjectAndAddOwnerAsMember()
    {
        using var db = TestDbFactory.Create();
        var user = new ApplicationUser { Id = "user-1", UserName = "user@test.com", FirstName = "Test", LastName = "User" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new ProjectService(db);
        var id = await service.CreateAsync(new ProjectInputModel
        {
            Name = "Demo Project",
            Description = "This is a demo project."
        }, user.Id);

        var project = await db.Projects.FindAsync(id);
        Assert.That(project, Is.Not.Null);
        Assert.That(project!.OwnerId, Is.EqualTo(user.Id));
        Assert.That(db.ProjectMembers.Count(), Is.EqualTo(1));
    }

    [Test]
    public void EditAsync_ShouldThrow_WhenUserIsNotOwner()
    {
        using var db = TestDbFactory.Create();
        db.Projects.Add(new Project
        {
            Id = 1,
            Name = "Original",
            Description = "Original description",
            OwnerId = "owner"
        });
        db.SaveChanges();

        var service = new ProjectService(db);

        Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.EditAsync(1, new ProjectInputModel
        {
            Name = "Changed",
            Description = "Changed description"
        }, "other-user"));
    }
}
