using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TaskFlow.Data.Models;
using TaskFlow.Services.Implementations;
using TaskFlow.Services.Models;

namespace TaskFlow.Tests;

public class ProjectServiceTests
{
    [Test]
    public async Task CreateAsync_ShouldCreateProject()
    {
        using var db = TestDbFactory.Create();

        var user = new ApplicationUser
        {
            Id = "user-1",
            UserName = "user@test.com",
            Email = "user@test.com",
            FirstName = "Test",
            LastName = "User"
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new ProjectService(db);

        await service.CreateAsync(new ProjectInputModel
        {
            Name = "Demo Project",
            Description = "This is a demo project."
        }, user.Id);

        var project = await db.Projects.FirstOrDefaultAsync(p => p.Name == "Demo Project");

        Assert.That(project, Is.Not.Null);
        Assert.That(project!.OwnerId, Is.EqualTo(user.Id));
    }

    [Test]
    public async Task GetMineAsync_ShouldReturnOnlyUserProjects()
    {
        using var db = TestDbFactory.Create();

        db.Users.AddRange(
            new ApplicationUser
            {
                Id = "user-1",
                UserName = "user1@test.com",
                Email = "user1@test.com",
                FirstName = "User",
                LastName = "One"
            },
            new ApplicationUser
            {
                Id = "user-2",
                UserName = "user2@test.com",
                Email = "user2@test.com",
                FirstName = "User",
                LastName = "Two"
            });

        db.Projects.AddRange(
            new Project
            {
                Id = 1,
                Name = "My Project",
                Description = "Owned by current user",
                OwnerId = "user-1"
            },
            new Project
            {
                Id = 2,
                Name = "Other Project",
                Description = "Owned by another user",
                OwnerId = "user-2"
            });

        await db.SaveChangesAsync();

        var service = new ProjectService(db);

        var projects = await service.GetMineAsync("user-1");

        Assert.That(projects.Count(), Is.EqualTo(1));
        Assert.That(projects.First().Name, Is.EqualTo("My Project"));
    }

    [Test]
    public async Task EditAsync_ShouldReturnTrue_WhenUserIsOwner()
    {
        using var db = TestDbFactory.Create();

        db.Projects.Add(new Project
        {
            Id = 1,
            Name = "Original",
            Description = "Original description",
            OwnerId = "owner"
        });

        await db.SaveChangesAsync();

        var service = new ProjectService(db);

        var result = await service.EditAsync(1, new ProjectInputModel
        {
            Name = "Changed",
            Description = "Changed description"
        }, "owner", false);

        var project = await db.Projects.FindAsync(1);

        Assert.That(result, Is.True);
        Assert.That(project!.Name, Is.EqualTo("Changed"));
    }

    [Test]
    public async Task EditAsync_ShouldReturnFalse_WhenUserIsNotOwnerOrAdmin()
    {
        using var db = TestDbFactory.Create();

        db.Projects.Add(new Project
        {
            Id = 1,
            Name = "Original",
            Description = "Original description",
            OwnerId = "owner"
        });

        await db.SaveChangesAsync();

        var service = new ProjectService(db);

        var result = await service.EditAsync(1, new ProjectInputModel
        {
            Name = "Changed",
            Description = "Changed description"
        }, "other-user", false);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveProject_WhenUserIsOwner()
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

        var service = new ProjectService(db);

        var result = await service.DeleteAsync(1, "owner", false);

        Assert.That(result, Is.True);
        Assert.That(await db.Projects.AnyAsync(), Is.False);
    }

    [Test]
    public async Task UserHasAccessAsync_ShouldReturnTrue_WhenUserIsOwner()
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

        var service = new ProjectService(db);

        var result = await service.UserHasAccessAsync(1, "owner");

        Assert.That(result, Is.True);
    }
}