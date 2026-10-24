using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Application.Interfaces.Persistence;

public interface IEventRepository
{
    Task<IReadOnlyList<GamingEvent>> GetAllAsync();
    Task<IReadOnlyList<GamingEvent>> GetNavigationAsync();
    Task<GamingEvent?> GetByIdAsync(Guid id);
    Task<GamingEvent?> GetBySlugAsync(string slug);
    Task<bool> SlugExistsAsync(string slug, Guid? exceptId = null);
    Task AddAsync(GamingEvent gamingEvent);
    Task<IReadOnlyList<Article>> GetPublishedArticlesAsync(Guid eventId, int page, int pageSize);
    Task<int> CountPublishedArticlesAsync(Guid eventId);
    Task<IReadOnlyList<string>> GetArticleSlugsAsync(Guid eventId);
}
