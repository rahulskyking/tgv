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
            int count)
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

        var articles =
            await _context.Articles
                .Include(x => x.FeaturedImage)
                .Include(x => x.Category)
                .Where(x =>
                    articleIds.Contains(x.Id)
                    &&
                    x.Status ==
                    ArticleStatus.Published)
                .ToListAsync();

        if (articles.Any())
        {
            return articles;
        }

        return await _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .Where(x =>
                x.Status ==
                ArticleStatus.Published)
            .OrderByDescending(x =>
                x.PublishedAt)
            .Take(count)
            .ToListAsync();
    }
}