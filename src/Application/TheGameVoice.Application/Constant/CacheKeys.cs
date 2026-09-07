using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Application.Constants;

public static class CacheKeys
{
    public const string HomePage =
        "home_page";

    public const string Categories =
        "categories";

    public const string Games =
        "games";

    public const string Tags =
        "tags";

    /// <summary>
    /// The public site renders one <see cref="GameSegment"/> at a time, so
    /// every cached page must be scoped to it — otherwise mobile content
    /// leaks into the PC / console cache and vice versa.
    /// </summary>
    public static string ForSegment(
        string key,
        GameSegment segment)
        => $"{key}_{(int)segment}";

    /// <summary>
    /// All homepage cache keys (legacy unsegmented key included) so admin
    /// actions can invalidate every variant in one call.
    /// </summary>
    public static string[] AllHomePageKeys()
        => new[]
        {
            HomePage,
            ForSegment(HomePage, GameSegment.PcConsole),
            ForSegment(HomePage, GameSegment.Mobile)
        };
}
