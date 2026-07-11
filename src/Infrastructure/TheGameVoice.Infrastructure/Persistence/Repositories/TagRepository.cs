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