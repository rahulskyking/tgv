using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TheGameVoice.Application.Constants;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Infrastructure.Persistence.Context;

namespace TheGameVoice.Infrastructure.BackgroundServices;

public class ScheduledArticlePublisher : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ScheduledArticlePublisher> _logger;

    public ScheduledArticlePublisher(
        IServiceScopeFactory scopeFactory,
        ILogger<ScheduledArticlePublisher> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation("Scheduled Article Publisher Started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

                var cache = scope.ServiceProvider
                    .GetRequiredService<ICacheService>();

                var utcNow = DateTime.UtcNow;

                var articles = await db.Articles
                    .Where(x =>
                        x.Status == ArticleStatus.Scheduled &&
                        x.ScheduledPublishAt != null &&
                        x.ScheduledPublishAt <= utcNow)
                    .Take(50)
                    .ToListAsync(stoppingToken);

                if (articles.Any())
                {
                    foreach (var article in articles)
                    {
                        article.Status = ArticleStatus.Published;

                        article.PublishedAt = utcNow;

                        article.ScheduledPublishAt = null;

                        article.ScheduledById = null;

                        article.LastModifiedAt = utcNow;
                    }

                    await db.SaveChangesAsync(stoppingToken);

                    cache.RemoveMany(CacheKeys.HomePage);

                    _logger.LogInformation(
                        "{Count} article(s) published automatically.",
                        articles.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while publishing scheduled articles.");
            }

#if DEBUG
            await Task.Delay(
                TimeSpan.FromMinutes(1),
                stoppingToken);
#else
            await Task.Delay(
                TimeSpan.FromMinutes(1),
                stoppingToken);
#endif
        }
    }
}