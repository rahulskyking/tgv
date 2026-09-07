using Microsoft.EntityFrameworkCore;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Infrastructure.Persistence.Context;

namespace TheGameVoice.Infrastructure.Persistence.Repositories;

public class ArticleViewRepository
    : IArticleViewRepository
{
    private readonly AppDbContext _context;

    public ArticleViewRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        ArticleView articleView)
    {
        await _context.ArticleViews
            .AddAsync(articleView);
    }
    public async Task<IReadOnlyList<Article>>
        GetTrendingArticlesAsync(
            int count,
            GameSegment? segment = null)
    {
        var sevenDaysAgo =
            DateTime.UtcNow.AddDays(-7);

        var articleIds =
            await _context.ArticleViews
                .Where(x =>
                    x.ViewedAt >= sevenDaysAgo)
                .GroupBy(x =>
                    x.ArticleId)
                .OrderByDescending(x =>
                    x.Count())
                .Take(count)
                .Select(x =>
                    x.Key)
                .ToListAsync();

        var trending = _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .Where(x =>
                articleIds.Contains(x.Id)
                &&
                x.Status ==
                ArticleStatus.Published);

        var articles =
            await ApplySegment(trending, segment)
                .ToListAsync();

        if (articles.Any())
        {
            return articles;
        }

        // Fallback: not enough view data yet, show the newest published
        // articles of the current site mode instead.
        var latest = _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .Where(x =>
                x.Status ==
                ArticleStatus.Published);

        return await ApplySegment(latest, segment)
            .OrderByDescending(x =>
                x.PublishedAt)
            .Take(count)
            .ToListAsync();
    }

    /// <summary>
    /// Restricts a query to a single audience segment. Articles flagged for
    /// both segments match either value (the column is a bit flag).
    /// </summary>
    private static IQueryable<Article> ApplySegment(
        IQueryable<Article> query,
        GameSegment? segment)
    {
        if (segment is null ||
            segment == GameSegment.None ||
            segment == GameSegment.All)
        {
            return query;
        }

        var flag = segment.Value;

        return query.Where(x =>
            (x.Segment & flag) == flag);
    }
}