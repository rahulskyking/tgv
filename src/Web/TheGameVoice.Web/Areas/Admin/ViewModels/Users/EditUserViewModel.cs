using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TheGameVoice.Web.Areas.Admin.ViewModels.Media;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Users;

public class EditUserViewModel
{
    public Guid Id { get; set; }

    [Required]
    public string FullName { get; set; } = default!;

    [Required]
    public string UserName { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string Role { get; set; } = default!;

    public bool IsActive { get; set; }

    public List<SelectListItem> Roles { get; set; }
        = new();
    public string? Slug { get; set; }

    public string? Bio { get; set; }

    public Guid? AvatarImageId { get; set; }

    public string? TwitterUrl { get; set; }

    public string? YouTubeUrl { get; set; }

    public string? WebsiteUrl { get; set; }

    public string? AvatarImagePath { get; set; }

    public List<MediaPickerItemViewModel> MediaItems
    {
        get;
        set;
    }
    = new();
}