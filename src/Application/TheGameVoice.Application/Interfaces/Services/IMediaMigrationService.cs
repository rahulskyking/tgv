using TheGameVoice.Application.Modules;

namespace TheGameVoice.Application.Interfaces.Services;

public interface IMediaMigrationService
{
    Task<MediaMigrationResult> MigrateAsync(
        bool dryRun = false,
        CancellationToken cancellationToken = default);
}