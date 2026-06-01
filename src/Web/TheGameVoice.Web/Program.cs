using Microsoft.EntityFrameworkCore;
using TheGameVoice.Infrastructure.DependencyInjection;
using TheGameVoice.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using TheGameVoice.Infrastructure.Identity.Seed;
using TheGameVoice.Infrastructure.Identity.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructureServices(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern:
    "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager =
        services.GetRequiredService<
            RoleManager<IdentityRole<Guid>>>();

    var userManager =
        services.GetRequiredService<
            UserManager<ApplicationUser>>();

    await RoleSeeder.SeedAsync(roleManager);

    await DefaultAdminSeeder
        .SeedAsync(userManager);
}

app.Run();
