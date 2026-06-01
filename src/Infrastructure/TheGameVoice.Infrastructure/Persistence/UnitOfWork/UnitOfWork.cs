using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Infrastructure.Persistence.Context;
using TheGameVoice.Infrastructure.Persistence.Repositories;

namespace TheGameVoice.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IMediaRepository Media { get; }
    public IArticleRepository Articles { get; }
    public ICategoryRepository Categories { get; }
    public ITagRepository Tags { get; }

    public IGameRepository Games { get; }
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Media = new MediaRepository(_context);
        Articles = new ArticleRepository(_context);
        Categories = new CategoryRepository(_context);
        Tags = new TagRepository(_context);
        Games = new GameRepository(_context);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context
            .SaveChangesAsync(cancellationToken);
    }
}