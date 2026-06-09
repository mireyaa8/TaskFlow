using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data.Models;

namespace TaskFlow.Data.Seed;

public static class ApplicationSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        await dbContext.Database.MigrateAsync();

        await SeedAdministratorAsync(userManager, roleManager);
        await SeedLabelsAsync(dbContext);
    }

    private static async Task SeedAdministratorAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        const string adminRoleName = "Administrator";
        const string adminEmail = "admin@taskflow.local";
        const string adminPassword = "Admin123!";

        if (!await roleManager.RoleExistsAsync(adminRoleName))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRoleName));
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Administrator",
                CreatedOn = DateTime.UtcNow
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);

            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not create admin user: " +
                    string.Join("; ", createResult.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, adminRoleName))
        {
            var roleResult = await userManager.AddToRoleAsync(adminUser, adminRoleName);

            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not add admin user to role: " +
                    string.Join("; ", roleResult.Errors.Select(e => e.Description)));
            }
        }
    }

    private static async Task SeedLabelsAsync(ApplicationDbContext dbContext)
    {
        if (await dbContext.Labels.AnyAsync())
        {
            return;
        }

        dbContext.Labels.AddRange(
            new Label { Name = "Bug", Color = "danger" },
            new Label { Name = "Feature", Color = "primary" },
            new Label { Name = "Research", Color = "info" },
            new Label { Name = "Urgent", Color = "warning" }
        );

        await dbContext.SaveChangesAsync();
    }
}