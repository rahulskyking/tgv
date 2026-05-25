using Microsoft.AspNetCore.Identity;
using TheGameVoice.Infrastructure.Identity.Entities;

namespace TheGameVoice.Infrastructure.Identity.Seed;

public static class DefaultAdminSeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager)
    {
        const string adminEmail =
            "admin@thegamevoice.com";

        const string adminPassword =
            "Admin@123";

        var existingAdmin =
            await userManager.FindByEmailAsync(adminEmail);

        if (existingAdmin != null)
        {
            return;
        }

        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "Super Admin",
            EmailConfirmed = true,
            IsActive = true
        };

        var result = await userManager
            .CreateAsync(adminUser, adminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(
                adminUser,
                Roles.SuperAdmin);
        }
    }
}