namespace TheGameVoice.Application.Interfaces.Repositories;

public interface IRepository<T>
    where T : class
{
    Task<T?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<T>> GetAllAsync();

    Task AddAsync(T entity);

    void Update(T entity);

    void Remove(T entity);
}