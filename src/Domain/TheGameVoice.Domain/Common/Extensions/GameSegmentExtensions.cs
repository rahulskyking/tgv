using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Domain.Common.Extensions;

/// <summary>
/// Helpers for working with the <see cref="GameSegment"/> bit flags.
/// </summary>
public static class GameSegmentExtensions
{
    /// <summary>
    /// True when <paramref name="value"/> covers <paramref name="segment"/>.
    /// Use this for in-memory checks only — inside EF queries write the
    /// bitwise comparison explicitly so it translates to SQL.
    /// </summary>
    public static bool Includes(
        this GameSegment value,
        GameSegment segment)
        => (value & segment) == segment && segment != GameSegment.None;

    /// <summary>Builds a segment from the two admin checkboxes.</summary>
    public static GameSegment FromFlags(
        bool isPcConsole,
        bool isMobile)
    {
        var segment = GameSegment.None;

        if (isPcConsole)
        {
            segment |= GameSegment.PcConsole;
        }

        if (isMobile)
        {
            segment |= GameSegment.Mobile;
        }

        return segment;
    }

    /// <summary>Short human readable label, e.g. "PC / Console + Mobile".</summary>
    public static string ToDisplayName(
        this GameSegment value)
        => value switch
        {
            GameSegment.PcConsole => "PC / Console",
            GameSegment.Mobile => "Mobile",
            GameSegment.All => "PC / Console + Mobile",
            _ => "Not set"
        };

    /// <summary>Slug used in URLs and the mode cookie ("pc" / "mobile").</summary>
    public static string ToSlug(
        this GameSegment value)
        => value == GameSegment.Mobile
            ? "mobile"
            : "pc";

    /// <summary>Parses the mode cookie / query value back into a segment.</summary>
    public static GameSegment ParseSegment(
        string? value)
        => value?.Trim().ToLowerInvariant() switch
        {
            "mobile" => GameSegment.Mobile,
            "mobile-games" => GameSegment.Mobile,
            _ => GameSegment.PcConsole
        };
}
