using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Repositories;
using TheGameVoice.Infrastructure.Persistence.Context;
using TheGameVoice.Infrastructure.Persistence.Repositories;
using TheGameVoice.Infrastructure.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using TheGameVoice.Infrastructure.Identity.Entities;
namespace TheGameVoice.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection
        AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString(
                    "DefaultConnection"))
            .UseSnakeCaseNamingConvention();
        });
        services.AddIdentity<ApplicationUser,
    IdentityRole<Guid>>(options =>
    {
        options.Password.RequireDigit = true;

        options.Password.RequireUppercase = false;

        options.Password.RequireNonAlphanumeric = false;

        options.Password.RequiredLength = 6;
    })
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

        services.AddScoped<IArticleRepository,
            ArticleRepository>();

        services.AddScoped<IUnitOfWork,
            UnitOfWork>();

        return services;
    }
}