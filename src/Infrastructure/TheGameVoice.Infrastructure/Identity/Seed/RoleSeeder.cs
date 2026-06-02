using Microsoft.AspNetCore.Identity;

namespace TheGameVoice.Infrastructure.Identity.Seed;

public static class RoleSeeder
{
    public static async Task SeedAsync(
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        var roles = new[]
     {
    Roles.SuperAdmin,
    Roles.Admin,
    Roles.Editor,
    Roles.Author
};
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new IdentityRole<Guid>(role));
            }
        }
    }
}