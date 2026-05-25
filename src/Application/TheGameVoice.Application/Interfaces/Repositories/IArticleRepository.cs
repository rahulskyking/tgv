using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Application.Interfaces.Repositories;

public interface IArticleRepository
    : IRepository<Article>
{
    Task<IReadOnlyList<Article>>
        GetLatestPublishedAsync(int count);

    Task<Article?> GetBySlugAsync(string slug);
}