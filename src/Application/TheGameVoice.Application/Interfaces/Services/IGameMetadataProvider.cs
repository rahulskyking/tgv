using TheGameVoice.Application.Common.Games;

namespace TheGameVoice.Application.Interfaces.Services;

public interface IGameMetadataProvider
{
    bool IsConfigured { get; }

    Task<IReadOnlyList<GameMetadata>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default);
}
