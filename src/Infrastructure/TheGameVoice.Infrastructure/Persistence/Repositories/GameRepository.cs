using Microsoft.EntityFrameworkCore;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Infrastructure.Persistence.Context;

namespace TheGameVoice.Infrastructure.Persistence.Repositories;

public class GameRepository : IGameRepository
{
    private readonly AppDbContext _context;

    public GameRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Game>>
        GetAllAsync()
    {
        return await _context.Games
            .Include(x => x.CoverImage)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Game?>
        GetByIdAsync(Guid id)
    {
        return await _context.Games
            .Include(x => x.CoverImage)
            .FirstOrDefaultAsync(x =>
                x.Id == id);
    }

    public async Task<Game?>
        GetBySlugAsync(string slug)
    {
        return await _context.Games
            .Include(x => x.CoverImage)
            .FirstOrDefaultAsync(x =>
                x.Slug == slug);
    }

    public async Task AddAsync(Game game)
    {
        await _context.Games
            .AddAsync(game);
    }
    public void Update(Game game)
    {
        _context.Games.Update(game);
    }

    public async Task<Game?> GetBySlugWithArticlesAsync(
       string slug)
    {
        return await _context.Games
            .Include(x => x.CoverImage)
            .Include(x => x.BannerImage)

            .Include(x => x.ArticleGames)
                .ThenInclude(x => x.Article)
                    .ThenInclude(x => x.FeaturedImage)

            .Include(x => x.ArticleGames)
                .ThenInclude(x => x.Article)
                    .ThenInclude(x => x.Category)

            .FirstOrDefaultAsync(x =>
                x.Slug == slug);
    }
    public void Remove(Game game)
    {
        _context.Games.Remove(game);
    }
}