using TheGameVoice.Domain.Entities;

public class AuthorProfileViewModel
{
    public string FullName { get; set; }

    public string? Bio { get; set; }

    public string? Slug { get; set; }

    public string? TwitterUrl { get; set; }

    public string? WebsiteUrl { get; set; }

    public string? AvatarImageUrl { get; set; }

    public IReadOnlyList<Article> Articles { get; set; }
}