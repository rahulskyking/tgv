using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Infrastructure.Configuration;
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
            var connectionString =
                configuration.GetConnectionString(
                    "DefaultConnection");

            var databaseUrl =
                Environment.GetEnvironmentVariable(
                    "DATABASE_URL");

            if (!string.IsNullOrWhiteSpace(databaseUrl))
            {
                var uri =
                    new Uri(databaseUrl);

                var userInfo =
                    uri.UserInfo.Split(':');

                connectionString =
                    $"Host={uri.Host};" +
                    $"Port={uri.Port};" +
                    $"Database={uri.AbsolutePath.TrimStart('/')};" +
                    $"Username={userInfo[0]};" +
                    $"Password={userInfo[1]};" +
                    $"SSL Mode=Require;" +
                    $"Trust Server Certificate=true";
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string not configured.");
            }

            Console.WriteLine(
                $"Database Source: " +
                (string.IsNullOrWhiteSpace(databaseUrl)
                    ? "DefaultConnection"
                    : "DATABASE_URL"));

            options
                .UseNpgsql(connectionString)
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
       .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

        services.AddScoped<IArticleRepository,
            ArticleRepository>();

        services.AddScoped<ICategoryRepository,
    CategoryRepository>();
        services.AddScoped<IUnitOfWork,
            UnitOfWork>();
        services.AddScoped<ISlugService,
    SlugService>();

        services.AddScoped<IGameRepository,GameRepository>();
        services.AddScoped<ITagRepository, TagRepository>();

        services.Configure<SupabaseStorageOptions>(configuration.GetSection("SupabaseStorage"));
        services.AddHttpClient();


        services.AddScoped<IStorageService,SupabaseStorageService>();

        services.AddMemoryCache();

        services.AddScoped<ICacheService,
            MemoryCacheService>();
        return services;

    }
}