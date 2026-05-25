using Microsoft.EntityFrameworkCore;
using TheGameVoice.Application.Interfaces.Repositories;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Infrastructure.Persistence.Context;

namespace TheGameVoice.Infrastructure.Persistence.Repositories;

public class ArticleRepository
    : Repository<Article>, IArticleRepository
{
    public ArticleRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Article>>
        GetLatestPublishedAsync(int count)
    {
        return await _dbSet
            .Where(x => x.Status == ArticleStatus.Published)
            .OrderByDescending(x => x.PublishedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Article?> GetBySlugAsync(string slug)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.Slug == slug);
    }
}