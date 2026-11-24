using TheGameVoice.Application.Modules.Tags;
using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Application.Interfaces.Persistence;

public interface ITagRepository
{
    Task<IReadOnlyList<Tag>>
        GetAllAsync();

    Task<TagAdminPage> GetAdminPageAsync(string? search, string usage, string sort, int page);

    Task<bool> NameExistsAsync(string name, Guid? excludingId = null);

    Task<bool> SlugExistsAsync(string slug);

    Task<IReadOnlyList<string>> GetArticleSlugsAsync(Guid tagId);

    Task<Tag?> GetByIdAsync(Guid id);

    Task<Tag?> GetBySlugAsync(string slug);

    Task AddAsync(Tag tag);

    Task<bool> IsInUseAsync(Guid id);

    void Remove(Tag tag);
}