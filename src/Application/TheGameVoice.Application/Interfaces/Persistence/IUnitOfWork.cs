using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Application.Interfaces.Persistence;

public interface IUnitOfWork
{
    IArticleRepository Articles { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);

    IMediaRepository Media { get; }
    ICategoryRepository Categories { get; }

    ITagRepository Tags { get; }
    IGameRepository Games { get; }
    IArticleViewRepository ArticleViews { get; }
} 