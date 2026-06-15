using Microsoft.EntityFrameworkCore;
using TheGameVoice.Application.Interfaces.Persistence;
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
    public override async Task<Article?> GetByIdAsync(Guid id)
    {
        return await _context.Articles
            .Include(x => x.Category)
            .Include(x => x.FeaturedImage)

            .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)

            .Include(x => x.ArticleGames)
                .ThenInclude(x => x.Game)

            .Include(x => x.ArticleMedia)
                .ThenInclude(x => x.Media)

            .Include(x => x.ArticleVideos)

            .FirstOrDefaultAsync(x =>
                x.Id == id);
    }
    public async Task<IReadOnlyList<Article>>
        GetLatestPublishedAsync(int count)
    {
        return await _dbSet
            .Where(x => x.Status == ArticleStatus.Published)
            .Include(x => x.Category)
            .OrderByDescending(x => x.PublishedAt)
            .Take(count)
            .ToListAsync();
    }



    public async Task<IReadOnlyList<Article>>GetAllWithMediaAsync()
    {
        return await _context.Articles
            .Include(x => x.FeaturedImage)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }


    public async Task<IReadOnlyList<Article>>
    GetPublishedAsync()
    {
        return await _context.Articles
            .Include(x => x.FeaturedImage ).Include(x => x.Category)
            .Where(x =>
                x.Status ==
                ArticleStatus.Published)
            .OrderByDescending(x =>
                x.PublishedAt)
            .ToListAsync();
    }
    public async Task<Article?> GetBySlugAsync(string slug)
    {
        return await _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
            .Include(x => x.ArticleGames)
                .ThenInclude(x => x.Game)
            .FirstOrDefaultAsync(x =>
                x.Slug == slug);
    }
    public async Task<IReadOnlyList<Article>> GetAllWithDetailsAsync()
    {
        return await _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Article>>
    GetPublishedByCategoryAsync(
        Guid categoryId)
    {
        return await _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .Where(x =>
                x.Status ==
                TheGameVoice.Domain.Enums.ArticleStatus.Published
                &&
                x.CategoryId == categoryId)
            .OrderByDescending(x =>
                x.PublishedAt)
            .ToListAsync();
    }
    public async Task<IReadOnlyList<Article>>
    GetRelatedArticlesAsync(
        Guid categoryId,
        Guid articleId)
    {
        return await _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .Where(x =>
                x.Status ==
                TheGameVoice.Domain.Enums.ArticleStatus.Published
                &&
                x.CategoryId == categoryId
                &&
                x.Id != articleId)
            .OrderByDescending(x =>
                x.PublishedAt)
            .Take(4)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Article>> SearchAsync(string query)
    {
        query = query.ToLower();

        return await _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .Where(x =>
                x.Status ==
                TheGameVoice.Domain.Enums.ArticleStatus.Published
                &&
                (
                    x.Title.ToLower().Contains(query)
                    ||
                    x.Summary.ToLower().Contains(query)
                ))
            .OrderByDescending(x =>
                x.PublishedAt)
            .ToListAsync();
    }
    public async Task<IReadOnlyList<Article>> GetPublishedByTagAsync(
        string slug)
    {
        return await _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
            .Where(x =>
                x.Status ==
                ArticleStatus.Published
                &&
                x.ArticleTags.Any(t =>
                    t.Tag.Slug == slug))
            .OrderByDescending(x =>
                x.PublishedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Article>>
        GetMostReadAsync(int count)
    {
        return await _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .Where(x =>
                x.Status ==
                ArticleStatus.Published)
            .OrderByDescending(x =>
                x.ViewCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Article>>
    GetPublishedByAuthorAsync(
        Guid authorId)
    {
        return await _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .Where(x =>
                x.Status == ArticleStatus.Published
                &&
                x.AuthorId == authorId)
            .OrderByDescending(x =>
                x.PublishedAt)
            .ToListAsync();
    }
}