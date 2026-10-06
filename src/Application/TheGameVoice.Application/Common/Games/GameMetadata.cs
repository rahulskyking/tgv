namespace TheGameVoice.Application.Common.Games;

public sealed class GameMetadata
{
    public long ExternalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Description { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public string? Developer { get; set; }
    public string? Publisher { get; set; }
    public IReadOnlyList<string> Platforms { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> Genres { get; set; } = Array.Empty<string>();
    public string? OfficialWebsite { get; set; }
    public string? SteamUrl { get; set; }
    public int? SteamAppId { get; set; }
    public string? CoverUrl { get; set; }
    public string? BannerUrl { get; set; }
    public bool IsPcConsole { get; set; }
    public bool IsMobile { get; set; }
}
