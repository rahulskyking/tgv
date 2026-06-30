using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Domain.Common.Base;
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
    public IArticleViewRepository ArticleViews { get; }

    public IGameRepository Games { get; }
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Media = new MediaRepository(_context);
        Articles = new ArticleRepository(_context);
        Categories = new CategoryRepository(_context);
        Tags = new TagRepository(_context);
        Games = new GameRepository(_context);
        ArticleViews = new ArticleViewRepository(_context);
    }

    public async Task<int> SaveChangesAsync(
     CancellationToken cancellationToken = default)
    {
        foreach (var entry in _context.ChangeTracker.Entries())
        {
            Console.WriteLine("----------------------------------");
            Console.WriteLine($"Entity : {entry.Entity.GetType().Name}");
            Console.WriteLine($"State  : {entry.State}");

            var values = entry.Properties
                .Select(p => $"{p.Metadata.Name} = {p.CurrentValue}");

            Console.WriteLine(string.Join(", ", values));
        }

        return await _context.SaveChangesAsync(cancellationToken);
    }
}