using System.ComponentModel.DataAnnotations;
using TheGameVoice.Web.Areas.Admin.ViewModels.Media;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Games;

public class EditGameViewModel
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = default!;

    public string? Summary { get; set; }

    public string? Description { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public string? Developer { get; set; }

    public string? Publisher { get; set; }

    public string? Platforms { get; set; }

    public string? Genres { get; set; }

    public string? OfficialWebsite { get; set; }

    public string? SteamUrl { get; set; }

    public int? SteamAppId { get; set; }

    public Guid? CoverImageId { get; set; }

    public Guid? BannerImageId { get; set; }

    public string? CoverImagePath { get; set; }

    public string? BannerImagePath { get; set; }

    public List<MediaPickerItemViewModel>
        MediaItems
    { get; set; }
    = new();
}