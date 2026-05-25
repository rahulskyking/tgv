using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Repositories;
using TheGameVoice.Infrastructure.Persistence.Context;
using TheGameVoice.Infrastructure.Persistence.Repositories;

namespace TheGameVoice.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IArticleRepository Articles { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;

        Articles = new ArticleRepository(context);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context
            .SaveChangesAsync(cancellationToken);
    }
}