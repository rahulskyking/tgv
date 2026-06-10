using Microsoft.Extensions.Caching.Memory;
using TheGameVoice.Application.Interfaces.Services;

public class MemoryCacheService
    : ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(
        IMemoryCache cache)
    {
        _cache = cache;
    }
    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan expiration)
    {
        if (_cache.TryGetValue(key, out T? value))
        {
            Console.WriteLine($"CACHE HIT => {key}");
            return value!;
        }

        Console.WriteLine($"CACHE MISS => {key}");

        value = await factory();

        _cache.Set(
            key,
            value,
            expiration);

        return value;
    }

    public void Remove(
        string key)
    {
        _cache.Remove(key);
    }

    public void RemoveMany(
        params string[] keys)
    {
        foreach (var key in keys)
        {
            _cache.Remove(key);
        }
    }
}