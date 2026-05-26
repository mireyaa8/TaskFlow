using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data.Models;

namespace TaskFlow.Data.Seed;

public static class ApplicationSeeder
{
    public static async Task SeedAsync(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        await dbContext.Database.MigrateAsync();

        if (!await roleManager.RoleExistsAsync(SeedConstants.AdministratorRoleName))
        {
            await roleManager.CreateAsync(new IdentityRole(SeedConstants.AdministratorRoleName));
        }

        var admin = await userManager.FindByEmailAsync(SeedConstants.AdminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = SeedConstants.AdminEmail,
                Email = SeedConstants.AdminEmail,
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Admin"
            };

            await userManager.CreateAsync(admin, SeedConstants.AdminPassword);
            await userManager.AddToRoleAsync(admin, SeedConstants.AdministratorRoleName);
        }

        if (!await dbContext.Labels.AnyAsync())
        {
            dbContext.Labels.AddRange(
                new Label { Name = "Bug", Color = "danger" },
                new Label { Name = "Feature", Color = "primary" },
                new Label { Name = "Research", Color = "info" },
                new Label { Name = "Urgent", Color = "warning" }
            );
            await dbContext.SaveChangesAsync();
        }
    }
}
