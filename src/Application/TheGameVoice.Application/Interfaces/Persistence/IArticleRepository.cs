using TheGameVoice.Application.Common.Pagination;
using TheGameVoice.Application.Modules.Articles;
using TheGameVoice.Application.Modules.Articles.Filters;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Application.Interfaces.Persistence;

/// <summary>
/// Public read methods accept an optional <see cref="GameSegment"/>.
/// Passing null keeps the old behaviour (every segment) which is what the
/// admin area, sitemap and RSS feed want; the public site always passes the
/// visitor's current site mode.
/// </summary>
public interface IArticleRepository
    : IRepository<Article>
{
    Task<IReadOnlyList<Article>>
        GetLatestPublishedAsync(
            int count,
            GameSegment? segment = null);

    Task<Article?> GetBySlugAsync(string slug);

    Task<IReadOnlyList<Article>>
    GetAllWithMediaAsync();

    Task<IReadOnlyList<Article>>
    GetPublishedAsync(
        GameSegment? segment = null);

    Task<IReadOnlyList<Article>>GetAllWithDetailsAsync();

    Task<IReadOnlyList<Article>>
    GetPublishedByCategoryAsync(
        Guid categoryId,
        GameSegment? segment = null);

    Task<IReadOnlyList<Article>>
    GetRelatedArticlesAsync(
        Guid categoryId,
        Guid articleId,
        GameSegment? segment = null);

    /// <summary>
    /// Published articles that share at least one tag with the given article,
    /// ordered by the number of shared tags (then recency). Falls back to an
    /// empty list when the article has no tags.
    /// </summary>
    Task<IReadOnlyList<Article>>
    GetRelatedByTagsAsync(
        Guid articleId,
        GameSegment? segment = null);

    Task<IReadOnlyList<Article>>
    SearchAsync(
        string query,
        GameSegment? segment = null);

    Task<IReadOnlyList<Article>>GetPublishedByTagAsync(
        string slug,
        GameSegment? segment = null);

    void Update(Article article);

    Task<IReadOnlyList<Article>>GetMostReadAsync(
        int count,
        GameSegment? segment = null);

    Task<IReadOnlyList<Article>>
    GetPublishedByAuthorAsync(
        Guid authorId,
        GameSegment? segment = null);

    Task DeleteReviewPointsAsync(Guid articleId);
    

    Task AddReviewPointsAsync(IEnumerable<ArticleReviewPoint> reviewPoints);

    Task<List<ArticleReviewPoint>> GetReviewPointsAsync(Guid articleId);
    Task<PagedResult<Article>> GetPagedAsync(
    ArticleFilter filter);

    /// <summary>
    /// Aggregate counters (article counts per status and view totals) for the
    /// supplied filter. The <see cref="ArticleFilter.Status"/> value is ignored
    /// so the caller always gets the complete breakdown.
    /// </summary>
    Task<ArticleStatsSummary> GetSummaryAsync(ArticleFilter filter);

}
