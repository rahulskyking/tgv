using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Application.Interfaces.Persistence;

public interface IMediaRepository
{
    Task<IReadOnlyList<Media>> GetAllAsync();

    Task<Media?> GetByIdAsync(Guid id);

    Task AddAsync(Media media);

    void Update(Media media);

    void Remove(Media media);
}