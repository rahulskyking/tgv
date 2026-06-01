using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Infrastructure.Identity.Entities;
using TheGameVoice.Infrastructure.Persistence.Context;
using TheGameVoice.Infrastructure.Persistence.Repositories;
using TheGameVoice.Infrastructure.Persistence.UnitOfWork;
using TheGameVoice.Infrastructure.Services;
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

        services.AddScoped<ICategoryRepository,
    CategoryRepository>();
        services.AddScoped<IUnitOfWork,
            UnitOfWork>();
        services.AddScoped<ISlugService,
    SlugService>();

        services.AddScoped<IGameRepository,
GameRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        return services;

    }
}