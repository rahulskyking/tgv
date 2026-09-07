using TheGameVoice.Domain.Entities;
using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Application.Interfaces.Persistence;

public interface IArticleViewRepository
{
    Task AddAsync(
        ArticleView articleView);

    /// <summary>
    /// Most viewed articles of the last 7 days, optionally restricted to a
    /// single audience segment (the visitor's current site mode).
    /// </summary>
    Task<IReadOnlyList<Article>>
        GetTrendingArticlesAsync(
            int count,
            GameSegment? segment = null);
}
