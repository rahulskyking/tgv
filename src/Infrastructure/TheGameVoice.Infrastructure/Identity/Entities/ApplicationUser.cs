using Microsoft.AspNetCore.Identity;
using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Infrastructure.Identity.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = default!;

    public string? Slug
    { get; set; }

    public string? Bio
    { get; set; }

    public Guid? AvatarImageId
    { get; set; }

    public Media? AvatarImage
    { get; set; }

    public string? TwitterUrl
    { get; set; }

    public string? YouTubeUrl
    { get; set; }

    public string? WebsiteUrl
    { get; set; }
    public bool IsActive { get; set; } = true;
}