using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Application.Interfaces.Persistence;

public interface IArticleRepository
    : IRepository<Article>
{
    Task<IReadOnlyList<Article>>
        GetLatestPublishedAsync(int count);

    Task<Article?> GetBySlugAsync(string slug);

    Task<IReadOnlyList<Article>>
    GetAllWithMediaAsync();

    Task<IReadOnlyList<Article>>
    GetPublishedAsync();
    Task<IReadOnlyList<Article>>GetAllWithDetailsAsync();

    Task<IReadOnlyList<Article>>
    GetPublishedByCategoryAsync(
        Guid categoryId);

    Task<IReadOnlyList<Article>>
    GetRelatedArticlesAsync(
        Guid categoryId,
        Guid articleId);

    Task<IReadOnlyList<Article>>
    SearchAsync(string query);

    Task<IReadOnlyList<Article>>GetPublishedByTagAsync(string slug);
    void Update(Article article);

    Task<IReadOnlyList<Article>>GetMostReadAsync(int count);

    Task<IReadOnlyList<Article>>
    GetPublishedByAuthorAsync(
        Guid authorId);
}