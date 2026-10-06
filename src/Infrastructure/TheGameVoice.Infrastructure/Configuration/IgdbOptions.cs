namespace TheGameVoice.Infrastructure.Configuration;

public sealed class IgdbOptions
{
    public const string SectionName = "Igdb";

    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string BaseUrl { get; set; } = "https://api.igdb.com/v4/";
    public string TokenUrl { get; set; } = "https://id.twitch.tv/oauth2/token";
}
