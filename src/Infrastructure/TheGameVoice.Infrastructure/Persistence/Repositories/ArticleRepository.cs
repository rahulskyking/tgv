using Microsoft.EntityFrameworkCore;
using TheGameVoice.Application.Common.Pagination;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Modules.Articles;
using TheGameVoice.Application.Modules.Articles.Filters;
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

            .Include(x => x.ReviewPoints)
            .FirstOrDefaultAsync(x =>
                x.Id == id);
    }
    /// <summary>
    /// Restricts a query to a single audience segment (PC/Console or Mobile).
    /// Articles flagged for both segments match either value because the
    /// column stores a bit flag. Null / All means "no filtering", which is
    /// what the admin area, sitemap and RSS feed use.
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

    public async Task<IReadOnlyList<Article>>
        GetLatestPublishedAsync(
            int count,
            GameSegment? segment = null)
    {
        var query = _dbSet
            .Where(x => x.Status == ArticleStatus.Published)
            .Include(x => x.Category);

        return await ApplySegment(query, segment)
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
    GetPublishedAsync(
        GameSegment? segment = null)
    {
        var query = _context.Articles
            .Include(x => x.FeaturedImage).Include(x => x.Category)
            .Where(x =>
                x.Status ==
                ArticleStatus.Published);

        return await ApplySegment(query, segment)
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

            .Include(x => x.ArticleMedia)
                .ThenInclude(x => x.Media)

            .Include(x => x.ArticleVideos)

            .Include(x => x.ReviewPoints)

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
        Guid categoryId,
        GameSegment? segment = null)
    {
        var query = _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .Where(x =>
                x.Status ==
                ArticleStatus.Published
                &&
                x.CategoryId == categoryId);

        return await ApplySegment(query, segment)
            .OrderByDescending(x =>
                x.PublishedAt)
            .ToListAsync();
    }
    public async Task<IReadOnlyList<Article>>
    GetRelatedArticlesAsync(
        Guid categoryId,
        Guid articleId,
        GameSegment? segment = null)
    {
        var query = _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .Where(x =>
                x.Status ==
                ArticleStatus.Published
                &&
                x.CategoryId == categoryId
                &&
                x.Id != articleId);

        return await ApplySegment(query, segment)
            .OrderByDescending(x =>
                x.PublishedAt)
            .Take(4)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Article>>
    GetRelatedByTagsAsync(
        Guid articleId,
        GameSegment? segment = null)
    {
        var tagIds = await _context.ArticleTags
            .Where(x => x.ArticleId == articleId)
            .Select(x => x.TagId)
            .ToListAsync();

        if (tagIds.Count == 0)
        {
            return Array.Empty<Article>();
        }

        var query = _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .Where(x =>
                x.Status == ArticleStatus.Published &&
                x.Id != articleId &&
                x.ArticleTags.Any(t => tagIds.Contains(t.TagId)));

        // Most shared tags first, then newest.
        return await ApplySegment(query, segment)
            .Select(x => new
            {
                Article = x,
                MatchCount = x.ArticleTags
                    .Count(t => tagIds.Contains(t.TagId))
            })
            .OrderByDescending(x => x.MatchCount)
            .ThenByDescending(x => x.Article.PublishedAt)
            .Select(x => x.Article)
            .Take(8)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Article>> SearchAsync(
        string query,
        GameSegment? segment = null)
    {
        query = query.ToLower();

        var articles = _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .Where(x =>
                x.Status ==
                ArticleStatus.Published
                &&
                (
                    x.Title.ToLower().Contains(query)
                    ||
                    x.Summary.ToLower().Contains(query)
                ));

        return await ApplySegment(articles, segment)
            .OrderByDescending(x =>
                x.PublishedAt)
            .ToListAsync();
    }
    public async Task<IReadOnlyList<Article>> GetPublishedByTagAsync(
        string slug,
        GameSegment? segment = null)
    {
        var query = _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
            .Where(x =>
                x.Status ==
                ArticleStatus.Published
                &&
                x.ArticleTags.Any(t =>
                    t.Tag.Slug == slug));

        return await ApplySegment(query, segment)
            .OrderByDescending(x =>
                x.PublishedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Article>>
        GetMostReadAsync(
            int count,
            GameSegment? segment = null)
    {
        var query = _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .Where(x =>
                x.Status ==
                ArticleStatus.Published);

        return await ApplySegment(query, segment)
            .OrderByDescending(x =>
                x.ViewCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Article>>
    GetPublishedByAuthorAsync(
        Guid authorId,
        GameSegment? segment = null)
    {
        var query = _context.Articles
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category)
            .Where(x =>
                x.Status == ArticleStatus.Published
                &&
                x.AuthorId == authorId);

        return await ApplySegment(query, segment)
            .OrderByDescending(x =>
                x.PublishedAt)
            .ToListAsync();
    }
    public async Task DeleteReviewPointsAsync(Guid articleId)
    {
        var existing = await _context.ArticleReviewPoints
            .Where(x => x.ArticleId == articleId)
            .ToListAsync();

        _context.ArticleReviewPoints.RemoveRange(existing);
    }

    public async Task AddReviewPointsAsync(
        IEnumerable<ArticleReviewPoint> reviewPoints)
    {
        await _context.ArticleReviewPoints.AddRangeAsync(reviewPoints);
    }

    public async Task<List<ArticleReviewPoint>> GetReviewPointsAsync(Guid articleId)
    {
        return await _context.ArticleReviewPoints
            .Where(x => x.ArticleId == articleId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();
    }

    public async Task<PagedResult<Article>> GetPagedAsync(
    ArticleFilter filter)
    {
        var query = BuildArticleQuery();

        // Search
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(x.Title, $"%{search}%") ||
                EF.Functions.ILike(x.Summary, $"%{search}%"));
        }

        // Status
        if (filter.Status.HasValue)
        {
            query = query.Where(x =>
                x.Status == filter.Status.Value);
        }

        // Audience segment (PC / Console vs Mobile)
        query = ApplySegment(query, filter.Segment);

        // Category
        if (filter.CategoryId.HasValue)
        {
            query = query.Where(x =>
                x.CategoryId == filter.CategoryId.Value);
        }

        // Author
        if (filter.AuthorId.HasValue)
        {
            query = query.Where(x =>
                x.AuthorId == filter.AuthorId.Value);
        }

        // Sorting
        query = filter.SortBy switch
        {
            ArticleSort.Oldest =>
                query.OrderBy(x => x.CreatedAt),

            ArticleSort.Updated =>
                query.OrderByDescending(x => x.UpdatedAt),

            ArticleSort.MostViewed =>
                query.OrderByDescending(x => x.ViewCount),

            ArticleSort.Title =>
                query.OrderBy(x => x.Title),

            _ =>
                query.OrderByDescending(x => x.CreatedAt)
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<Article>
        {
            Items = items,
            TotalCount = totalCount,
            CurrentPage = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<ArticleStatsSummary> GetSummaryAsync(
        ArticleFilter filter)
    {
        var query = _context.Articles.AsNoTracking();

        // Search
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(x.Title, $"%{search}%") ||
                EF.Functions.ILike(x.Summary, $"%{search}%"));
        }

        // Category
        if (filter.CategoryId.HasValue)
        {
            query = query.Where(x =>
                x.CategoryId == filter.CategoryId.Value);
        }

        // Author
        if (filter.AuthorId.HasValue)
        {
            query = query.Where(x =>
                x.AuthorId == filter.AuthorId.Value);
        }

        // Audience segment (PC / Console vs Mobile)
        query = ApplySegment(query, filter.Segment);

        // Status is deliberately not applied: the cards show the breakdown.
        var buckets = await query
            .GroupBy(x => x.Status)
            .Select(g => new
            {
                Status = g.Key,
                Count = g.Count(),
                Views = g.Sum(x => (long)x.ViewCount)
            })
            .ToListAsync();

        int CountFor(ArticleStatus status) =>
            buckets.FirstOrDefault(x => x.Status == status)?.Count ?? 0;

        long ViewsFor(ArticleStatus status) =>
            buckets.FirstOrDefault(x => x.Status == status)?.Views ?? 0;

        return new ArticleStatsSummary
        {
            TotalArticles = buckets.Sum(x => x.Count),
            PublishedArticles = CountFor(ArticleStatus.Published),
            DraftArticles = CountFor(ArticleStatus.Draft),
            ReviewPendingArticles = CountFor(ArticleStatus.ReviewPending),
            ScheduledArticles = CountFor(ArticleStatus.Scheduled),
            RejectedArticles = CountFor(ArticleStatus.Rejected),
            ArchivedArticles = CountFor(ArticleStatus.Archived),
            TotalViews = buckets.Sum(x => x.Views),
            PublishedViews = ViewsFor(ArticleStatus.Published)
        };
    }

    private IQueryable<Article> BuildArticleQuery()
    {
        return _context.Articles
            .AsNoTracking()
            .Include(x => x.FeaturedImage)
            .Include(x => x.Category);
    }

}