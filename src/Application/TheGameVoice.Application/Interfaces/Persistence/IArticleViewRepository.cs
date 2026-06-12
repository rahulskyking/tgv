using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Application.Interfaces.Persistence;

public interface IArticleViewRepository
{
    Task AddAsync(
        ArticleView articleView);

    Task<IReadOnlyList<Article>>
        GetTrendingArticlesAsync(
            int count);
}