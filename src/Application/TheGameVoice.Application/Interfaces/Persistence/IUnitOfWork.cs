using TheGameVoice.Application.Interfaces.Repositories;

namespace TheGameVoice.Application.Interfaces.Persistence;

public interface IUnitOfWork
{
    IArticleRepository Articles { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}