using Microsoft.EntityFrameworkCore;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Infrastructure.Persistence.Context;

namespace TheGameVoice.Infrastructure.Persistence.Repositories;

public class EventRepository : IEventRepository
{
    private readonly AppDbContext _context;
    public EventRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<GamingEvent>> GetAllAsync() =>
        await _context.Events.Include(x => x.BannerImage)
            .OrderBy(x => x.DisplayOrder).ThenByDescending(x => x.StartDate)
            .ThenBy(x => x.Name).ToListAsync();

    public async Task<IReadOnlyList<GamingEvent>> GetNavigationAsync() =>
        await _context.Events.AsNoTracking()
            .Where(x => x.IsActive && x.ShowInNavigation)
            .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync();

    public Task<GamingEvent?> GetByIdAsync(Guid id) =>
        _context.Events.Include(x => x.BannerImage).FirstOrDefaultAsync(x => x.Id == id);

    // Archived hubs remain public; IsActive must not be a global query filter.
    public Task<GamingEvent?> GetBySlugAsync(string slug) =>
        _context.Events.Include(x => x.BannerImage).FirstOrDefaultAsync(x => x.Slug == slug);

    public Task<bool> SlugExistsAsync(string slug, Guid? exceptId = null) =>
        _context.Events.IgnoreQueryFilters().AnyAsync(x => x.Slug == slug && x.Id != exceptId);

    public async Task AddAsync(GamingEvent gamingEvent)
    {
        await _context.Events.AddAsync(gamingEvent);
    }

    public async Task<IReadOnlyList<Article>> GetPublishedArticlesAsync(Guid eventId, int page, int pageSize) =>
        await Published(eventId).AsNoTracking().Include(x => x.Category).Include(x => x.FeaturedImage)
            .OrderByDescending(x => x.PublishedAt).ThenBy(x => x.Id)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

    public Task<int> CountPublishedArticlesAsync(Guid eventId) => Published(eventId).CountAsync();

    public async Task<IReadOnlyList<string>> GetArticleSlugsAsync(Guid eventId) =>
        await _context.Articles.Where(x => x.EventId == eventId).Select(x => x.Slug).ToListAsync();

    // Event coverage spans both site modes, and never includes drafts/scheduled articles.
    private IQueryable<Article> Published(Guid eventId) =>
        _context.Articles.Where(x => x.EventId == eventId && x.Status == ArticleStatus.Published);
}
