using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Application.Modules;
using TheGameVoice.Infrastructure.Persistence.Context;

namespace TheGameVoice.Infrastructure.Services;

public class MediaMigrationService : IMediaMigrationService
{
    private readonly AppDbContext _db;
    private readonly CloudflareR2StorageService _r2Storage;
    private readonly HttpClient _httpClient;
    private readonly ILogger<MediaMigrationService> _logger;

    public MediaMigrationService(
        AppDbContext db,
        CloudflareR2StorageService r2Storage,
        IHttpClientFactory httpClientFactory,
        ILogger<MediaMigrationService> logger)
    {
        _db = db;
        _r2Storage = r2Storage;
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public async Task<MediaMigrationResult> MigrateAsync(
    bool dryRun = false,
    CancellationToken cancellationToken = default)
    {
        var result = new MediaMigrationResult();

        var mediaItems = await _db.Media
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        result.Total = mediaItems.Count;

        foreach (var media in mediaItems)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (IsR2File(media.FilePath))
            {
                result.Skipped++;
                continue;
            }

            if (!IsSupabaseFile(media.FilePath))
            {
                result.Skipped++;
                continue;
            }

            if (dryRun)
            {
                result.Migrated++;

                _logger.LogInformation(
                    "[DRY RUN] Would migrate media {MediaId}: {FilePath}",
                    media.Id,
                    media.FilePath);

                continue;
            }

            try
            {
                _logger.LogInformation(
                    "Migrating media {MediaId}: {FilePath}",
                    media.Id,
                    media.FilePath);

                using var response =
                    await _httpClient.GetAsync(
                        media.FilePath,
                        HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken);

                response.EnsureSuccessStatusCode();

                await using var sourceStream =
                    await response.Content.ReadAsStreamAsync(
                        cancellationToken);

                var newFilePath =
                    await _r2Storage.UploadAsync(
                        sourceStream,
                        media.FileName,
                        media.ContentType,
                        cancellationToken);

                media.FilePath = newFilePath;

                await _db.SaveChangesAsync(
                    cancellationToken);

                result.Migrated++;

                _logger.LogInformation(
                    "Successfully migrated media {MediaId}",
                    media.Id);
            }
            catch (Exception ex)
            {
                result.Failed++;

                var error =
                    $"Media {media.Id}: {ex.Message}";

                result.Errors.Add(error);

                _logger.LogError(
                    ex,
                    "Failed to migrate media {MediaId}",
                    media.Id);
            }
        }

        return result;
    }

    private static bool IsR2File(string filePath)
    {
        return filePath.StartsWith(
            "https://media.thegamevoice.com/",
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSupabaseFile(string filePath)
    {
        return filePath.Contains(
            ".supabase.co/storage/",
            StringComparison.OrdinalIgnoreCase);
    }
}