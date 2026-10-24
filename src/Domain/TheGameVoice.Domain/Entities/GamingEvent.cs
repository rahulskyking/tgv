namespace TheGameVoice.Domain.Entities;

/// <summary>An editorial event hub, independent of an article's primary category.</summary>
public class GamingEvent : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? BannerImageId { get; set; }
    public Media? BannerImage { get; set; }
    public string? BannerAltText { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Location { get; set; }
    public string? OfficialWebsite { get; set; }
    public bool IsActive { get; set; } = true;
    public bool ShowInNavigation { get; set; }
    public int DisplayOrder { get; set; }
    public ICollection<Article> Articles { get; set; } = new List<Article>();
}
