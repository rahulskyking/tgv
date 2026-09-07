using TheGameVoice.Application.Modules;

namespace TheGameVoice.Application.Interfaces.Services;

public interface IArticleContentMigrationService
{
    Task<ArticleContentMigrationResult> ScanAsync(
        CancellationToken cancellationToken = default);

    Task<ArticleContentMigrationResult> MigrateAsync(
        CancellationToken cancellationToken = default);
}