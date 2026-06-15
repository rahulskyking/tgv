using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Infrastructure.Persistence.Context;

namespace TheGameVoice.Infrastructure.Services;

public class SlugService : ISlugService
{
    private readonly AppDbContext _context;

    public SlugService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateSlugAsync(
        string title)
    {
        var baseSlug = GenerateSlug(title);

        var slug = baseSlug;

        var counter = 2;

        while (await _context.Articles
            .AnyAsync(x => x.Slug == slug))
        {
            slug = $"{baseSlug}-{counter}";

            counter++;
        }

        return slug;
    }

    private static string GenerateSlug(string phrase)
    {
        var str = phrase.ToLowerInvariant();

        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

        str = Regex.Replace(str, @"\s+", "-")
            .Trim('-');

        str = Regex.Replace(str, @"-+", "-");

        return str;
    }
    public async Task<string>
    GenerateAuthorSlugAsync(
        string fullName)
    {
        var baseSlug =
            GenerateSlug(fullName);

        var slug =
            baseSlug;

        var counter = 2;

        while (await _context.Users
            .AnyAsync(x => x.Slug == slug))
        {
            slug = $"{baseSlug}-{counter}";

            counter++;
        }

        return slug;
    }
}