using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Application.Interfaces.Persistence;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllAsync();

    Task<Category?> GetByIdAsync(Guid id);

    Task<Category?> GetBySlugAsync(string slug);

    Task AddAsync(Category category);

  

   
}