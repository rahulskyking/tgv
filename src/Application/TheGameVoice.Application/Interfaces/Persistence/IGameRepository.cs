using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Application.Interfaces.Persistence;

public interface IGameRepository
{
    Task<IReadOnlyList<Game>>
        GetAllAsync();

    Task<Game?> GetByIdAsync(Guid id);

    Task<Game?> GetBySlugAsync(string slug);

    Task AddAsync(Game game);

    void Update(Game game);
    Task<Game?> GetBySlugWithArticlesAsync(
    string slug);
    void Remove(Game game);
}