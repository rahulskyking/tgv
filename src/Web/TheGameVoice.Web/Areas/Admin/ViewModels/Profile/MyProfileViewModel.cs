using System.ComponentModel.DataAnnotations;
using TheGameVoice.Web.Areas.Admin.ViewModels.Media;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Profile;

/// <summary>
/// The signed-in user's own profile. Deliberately smaller than
/// EditUserViewModel: role and active status are not editable here, because
/// nobody should be able to promote themselves.
/// </summary>
public class MyProfileViewModel
{
    public Guid Id { get; set; }

    [Required]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Display(Name = "Profile URL")]
    public string? Slug { get; set; }

    [Display(Name = "Short bio")]
    [StringLength(600)]
    public string? Bio { get; set; }

    public Guid? AvatarImageId { get; set; }

    public string? AvatarImagePath { get; set; }

    [Url]
    [Display(Name = "X / Twitter")]
    public string? TwitterUrl { get; set; }

    [Url]
    [Display(Name = "YouTube")]
    public string? YouTubeUrl { get; set; }

    [Url]
    [Display(Name = "Website")]
    public string? WebsiteUrl { get; set; }

    [Url]
    [Display(Name = "Steam")]
    public string? SteamUrl { get; set; }

    public List<MediaPickerItemViewModel> MediaItems { get; set; }
        = new();

    /* ---- read-only context ---- */

    public string UserName { get; set; } = default!;

    public string Role { get; set; } = default!;

    public int PublishedArticles { get; set; }

    public int DraftArticles { get; set; }

    public int PendingArticles { get; set; }

    public long TotalViews { get; set; }

    public DateTime? LastPublishedAtUtc { get; set; }

    public string Initials =>
        string.Join(
            string.Empty,
            (FullName ?? string.Empty)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Take(2)
                .Select(part => char.ToUpperInvariant(part[0])));
}

/// <summary>Separate model so a password change cannot ride on a profile save.</summary>
public class ChangePasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Current password")]
    public string CurrentPassword { get; set; } = default!;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    [Display(Name = "New password")]
    public string NewPassword { get; set; } = default!;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "The passwords do not match.")]
    [Display(Name = "Confirm new password")]
    public string ConfirmPassword { get; set; } = default!;
}
