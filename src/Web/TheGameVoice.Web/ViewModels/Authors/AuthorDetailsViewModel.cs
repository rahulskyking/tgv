using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Web.ViewModels.Authors;

public class AuthorDetailsViewModel
{
    public string FullName { get; set; }
        = default!;

    public string? Bio { get; set; }

    public string? Slug { get; set; }

    public string? TwitterUrl { get; set; }

    public string? YouTubeUrl { get; set; }

    public string? WebsiteUrl { get; set; }

    public string? AvatarImagePath { get; set; }

    public IReadOnlyList<Article> Articles
    {
        get;
        set;
    }
    = new List<Article>();
}