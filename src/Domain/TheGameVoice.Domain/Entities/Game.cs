using TheGameVoice.Domain.Common.Base;

namespace TheGameVoice.Domain.Entities;

public class Game : AuditableEntity
{
    public string Name { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public string Summary { get; set; } = default!;

    public string Description { get; set; } = default!;

    public DateTime? ReleaseDate { get; set; }

    public string? Developer { get; set; }

    public string? Publisher { get; set; }

    public string? Platforms { get; set; }

    public string? Genres { get; set; }

    public string? OfficialWebsite { get; set; }

    public string? SteamUrl { get; set; }

    public int? SteamAppId { get; set; }

    public GameDataSource DataSource { get; set; }

    public DateTime? LastSteamSyncAt { get; set; }

    public Guid? CoverImageId { get; set; }

    public Media? CoverImage { get; set; }

    public Guid? BannerImageId { get; set; }

    public Media? BannerImage { get; set; }

    public ICollection<ArticleGame> ArticleGames
    { get; set; } = new List<ArticleGame>();
}