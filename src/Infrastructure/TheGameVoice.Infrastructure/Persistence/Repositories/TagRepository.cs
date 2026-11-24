using TheGameVoice.Application.Common.Pagination;
using TheGameVoice.Application.Modules.Tags;
using Microsoft.EntityFrameworkCore;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Infrastructure.Persistence.Context;

namespace TheGameVoice.Infrastructure.Persistence.Repositories;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _context;

    public TagRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Tag>>
        GetAllAsync()
    {
        return await _context.Tags
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<TagAdminPage> GetAdminPageAsync(string? search, string usage, string sort, int page)
    {
        var all = _context.Tags.AsNoTracking();
        var total = await all.CountAsync();
        var used = await all.CountAsync(x => x.ArticleTags.Any());
        var query = all.Select(x => new TagAdminItem
        {
            Id = x.Id, Name = x.Name, Slug = x.Slug, CreatedAt = x.CreatedAt,
            // Count all assignments, matching IsInUseAsync's deletion guard.
            ArticleCount = x.ArticleTags.Count()
        });

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(x => x.Name.ToLower().Contains(term) || x.Slug.ToLower().Contains(term));
        }
        if (usage == "used") query = query.Where(x => x.ArticleCount > 0);
        if (usage == "unused") query = query.Where(x => x.ArticleCount == 0);

        var count = await query.CountAsync();
        const int pageSize = 20;
        page = Math.Clamp(page, 1, Math.Max(1, (int)Math.Ceiling(count / (double)pageSize)));
        var ordered = sort switch
        {
            "newest" => query.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id),
            "most-used" => query.OrderByDescending(x => x.ArticleCount).ThenBy(x => x.Name).ThenBy(x => x.Id),
            "name-desc" => query.OrderByDescending(x => x.Name).ThenBy(x => x.Id),
            _ => query.OrderBy(x => x.Name).ThenBy(x => x.Id)
        };
        return new TagAdminPage
        {
            TotalTags = total, UsedTags = used,
            Tags = new PagedResult<TagAdminItem>
            {
                Items = await ordered.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(),
                CurrentPage = page, PageSize = pageSize, TotalCount = count
            }
        };
    }

    public Task<bool> NameExistsAsync(string name, Guid? excludingId = null)
    {
        var normalized = name.Trim().ToLowerInvariant();
        return _context.Tags.AnyAsync(x => x.Id != excludingId && x.Name.Trim().ToLower() == normalized);
    }

    public Task<bool> SlugExistsAsync(string slug) =>
        // The unique index also includes soft-deleted rows.
        _context.Tags.IgnoreQueryFilters().AnyAsync(x => x.Slug == slug);

    public async Task<IReadOnlyList<string>> GetArticleSlugsAsync(Guid tagId) =>
        await _context.Articles.AsNoTracking()
            .Where(x => x.ArticleTags.Any(t => t.TagId == tagId))
            .Select(x => x.Slug).ToListAsync();

    public async Task<Tag?>
        GetByIdAsync(Guid id)
    {
        return await _context.Tags
            .FirstOrDefaultAsync(x =>
                x.Id == id);
    }

    public async Task<Tag?>
        GetBySlugAsync(string slug)
    {
        return await _context.Tags
            .FirstOrDefaultAsync(x =>
                x.Slug == slug);
    }

    public async Task AddAsync(Tag tag)
    {
        await _context.Tags
            .AddAsync(tag);
    }

    public async Task<bool> IsInUseAsync(Guid id)
    {
        return await _context.ArticleTags
            .AnyAsync(x => x.TagId == id);
    }

    public void Remove(Tag tag)
    {
        _context.Tags.Remove(tag);
    }
}