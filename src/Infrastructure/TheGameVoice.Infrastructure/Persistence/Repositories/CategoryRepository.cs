using Microsoft.EntityFrameworkCore;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Infrastructure.Persistence.Context;

namespace TheGameVoice.Infrastructure.Persistence.Repositories;

public class CategoryRepository
    : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Category>>
        GetAllAsync()
    {
        return await _context.Categories
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Category?>
        GetByIdAsync(Guid id)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(x =>
                x.Id == id);
    }

    public async Task<Category?>
        GetBySlugAsync(string slug)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(x =>
                x.Slug == slug);
    }

    public async Task AddAsync(
        Category category)
    {
        await _context.Categories
            .AddAsync(category);
    }
}