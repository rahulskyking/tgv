using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Application.Modules;
using TheGameVoice.Infrastructure.Persistence.Context;

namespace TheGameVoice.Infrastructure.Services;

public class ArticleContentMigrationService : IArticleContentMigrationService
{
    private readonly AppDbContext _db;
    private readonly ILogger<ArticleContentMigrationService> _logger;

    public ArticleContentMigrationService(
        AppDbContext db,
        ILogger<ArticleContentMigrationService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ArticleContentMigrationResult> ScanAsync(
        CancellationToken cancellationToken = default)
    {
        var result = new ArticleContentMigrationResult();

        var articles = await _db.Articles
            .AsNoTracking()
            .Where(x => x.Content != null)
            .ToListAsync(cancellationToken);

        result.ArticlesScanned = articles.Count;

        // Load all migrated R2 media
        var r2Media = await _db.Media
     .AsNoTracking()
     .Where(x =>
         x.FilePath.StartsWith(
             "https://media.thegamevoice.com/"))
     .ToListAsync(cancellationToken);

        // Match by filename
        var mediaByFileName = r2Media
     .Where(x => !string.IsNullOrWhiteSpace(x.FileName))
     .GroupBy(
         x => x.FileName.Trim(),
         StringComparer.OrdinalIgnoreCase)
     .ToDictionary(
         x => x.Key,
         x => x.First(),
         StringComparer.OrdinalIgnoreCase);

        foreach (var article in articles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!article.Content!.Contains(
                    "supabase.co/storage/",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            result.ArticlesWithSupabaseUrls++;

            var urls = ExtractSupabaseImageUrls(article.Content);

            foreach (var url in urls)
            {
                result.ImagesFound++;

                var fileName = ExtractFileName(url);

                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = RemoveStorageGuidPrefix(fileName);
                }

                _logger.LogWarning(
                    "ARTICLE CONTENT IMAGE DEBUG | URL: {Url} | EXTRACTED FILE: {FileName}",
                    url,
                    fileName);

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    result.ImagesUnmatched++;

                    result.UnmatchedFiles.Add(
                        $"Article {article.Id} | {article.Title} | URL: {url}");

                    continue;
                }
                if (mediaByFileName.TryGetValue(
                        fileName.Trim(),
                        out var matchedMedia))
                {
                    result.ImagesMatched++;

                    result.MatchedImages.Add(
                        new ArticleContentImageMapping
                        {
                            ArticleId = article.Id,
                            ArticleTitle = article.Title,
                            OldUrl = url,
                            OldFileName = fileName,
                            MediaId = matchedMedia.Id,
                            R2Url = matchedMedia.FilePath
                        });
                }
                else
                {
                    result.ImagesUnmatched++;

                    result.UnmatchedFiles.Add(
                        $"Article {article.Id} | {article.Title} | File: {fileName}");
                }
            }

            if (urls.Count > 0 &&
     urls.All(url =>
     {
         var fileName = ExtractFileName(url);

         if (string.IsNullOrWhiteSpace(fileName))
             return false;

         fileName = RemoveStorageGuidPrefix(fileName);

         return mediaByFileName.ContainsKey(fileName.Trim());
     }))
            {
                result.ArticlesReadyToMigrate++;
            }
        }

        _logger.LogInformation(
            "Article content migration scan complete. " +
            "Articles: {Articles}, " +
            "Articles with Supabase URLs: {SupabaseArticles}, " +
            "Images found: {ImagesFound}, " +
            "Matched: {Matched}, " +
            "Unmatched: {Unmatched}",
            result.ArticlesScanned,
            result.ArticlesWithSupabaseUrls,
            result.ImagesFound,
            result.ImagesMatched,
            result.ImagesUnmatched);

        return result;
    }
    private static string RemoveStorageGuidPrefix(string fileName)
    {
        var underscoreIndex = fileName.IndexOf('_');

        if (underscoreIndex <= 0)
            return fileName;

        var possibleGuid = fileName[..underscoreIndex];

        return Guid.TryParse(possibleGuid, out _)
            ? fileName[(underscoreIndex + 1)..]
            : fileName;
    }
    public async Task<ArticleContentMigrationResult> MigrateAsync(
     CancellationToken cancellationToken = default)
    {
        var result = new ArticleContentMigrationResult();

        var articles = await _db.Articles
            .Where(x => x.Content != null)
            .ToListAsync(cancellationToken);

        result.ArticlesScanned = articles.Count;

        var r2Media = await _db.Media
            .AsNoTracking()
            .Where(x =>
                x.FilePath.StartsWith(
                    "https://media.thegamevoice.com/"))
            .ToListAsync(cancellationToken);

        var mediaByFileName = r2Media
            .Where(x => !string.IsNullOrWhiteSpace(x.FileName))
            .GroupBy(
                x => x.FileName.Trim(),
                StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                x => x.Key,
                x => x.First(),
                StringComparer.OrdinalIgnoreCase);

        foreach (var article in articles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!article.Content!.Contains(
                    "supabase.co/storage/",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            result.ArticlesWithSupabaseUrls++;

            var urls = ExtractSupabaseImageUrls(article.Content);

            if (urls.Count == 0)
                continue;

            var replacements = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            var articleCanBeMigrated = true;

            foreach (var url in urls)
            {
                result.ImagesFound++;

                var fileName = ExtractFileName(url);

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    result.ImagesUnmatched++;
                    articleCanBeMigrated = false;
                    continue;
                }

                fileName = RemoveStorageGuidPrefix(fileName);

                if (!mediaByFileName.TryGetValue(
                        fileName.Trim(),
                        out var media))
                {
                    result.ImagesUnmatched++;
                    articleCanBeMigrated = false;

                    result.UnmatchedFiles.Add(
                        $"Article {article.Id}: {fileName}");

                    continue;
                }

                result.ImagesMatched++;

                replacements[url] = media.FilePath;

                result.MatchedImages.Add(
                    new ArticleContentImageMapping
                    {
                        ArticleId = article.Id,
                        ArticleTitle = article.Title,
                        OldUrl = url,
                        OldFileName = fileName,
                        MediaId = media.Id,
                        R2Url = media.FilePath
                    });
            }

            // IMPORTANT:
            // Only modify an article when ALL of its detected
            // Supabase images have a verified R2 match.
            if (!articleCanBeMigrated)
                continue;

            foreach (var replacement in replacements)
            {
                article.Content = article.Content.Replace(
                    replacement.Key,
                    replacement.Value,
                    StringComparison.OrdinalIgnoreCase);
            }

            if (replacements.Count > 0)
            {
                await _db.SaveChangesAsync(cancellationToken);

                //result.ArticlesReadyToMigrate++;
                result.ArticlesMigrated++;
            }
        }

        return result;
    }
    private static List<string> ExtractSupabaseImageUrls(string content)
    {
        var matches = Regex.Matches(
            content,
            @"https?://[^""'<>]+supabase\.co/storage/[^""'<>]+",
            RegexOptions.IgnoreCase);

        return matches
            .Select(x => x.Value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string? ExtractFileName(string url)
    {
        try
        {
            var uri = new Uri(url);

            var path = uri.AbsolutePath.TrimEnd('/');

            var fileName = path.Split(
                '/',
                StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault();

            return string.IsNullOrWhiteSpace(fileName)
                ? null
                : Uri.UnescapeDataString(fileName);
        }
        catch
        {
            return null;
        }
    }
}