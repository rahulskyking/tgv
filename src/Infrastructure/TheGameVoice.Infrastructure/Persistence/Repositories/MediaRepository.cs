using Microsoft.EntityFrameworkCore;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Infrastructure.Persistence.Context;

namespace TheGameVoice.Infrastructure.Persistence.Repositories;

public class MediaRepository : IMediaRepository
{
    private readonly AppDbContext _context;

    public MediaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Media>>
        GetAllAsync()
    {
        return await _context.Media
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Media media)
    {
        await _context.Media.AddAsync(media);
    }

    public async Task<Media?> GetByIdAsync(Guid id)
    {
        return await _context.Media
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public void Update(Media media)
    {
        _context.Media.Update(media);
    }

    public void Remove(Media media)
    {
        _context.Media.Remove(media);
    }
}