using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Application.Interfaces.Persistence;

public interface ITagRepository
{
    Task<IReadOnlyList<Tag>>
        GetAllAsync();

    Task<Tag?> GetByIdAsync(Guid id);

    Task<Tag?> GetBySlugAsync(string slug);

    Task AddAsync(Tag tag);

    Task<bool> IsInUseAsync(Guid id);

    void Remove(Tag tag);
}