using System.ComponentModel.DataAnnotations;
using TheGameVoice.Domain.Common.Extensions;
using TheGameVoice.Domain.Enums;
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

    #region Section

    /// <summary>Show this game in the PC / Console section.</summary>
    [Display(Name = "PC / Console game")]
    public bool IsPcConsole { get; set; } = true;

    /// <summary>Show this game in the Mobile section.</summary>
    [Display(Name = "Mobile game")]
    public bool IsMobile { get; set; }

    /// <summary>
    /// Sections this game shows up in (trending games widget, game pages).
    /// Defaults to PC / Console when nothing is ticked.
    /// </summary>
    public GameSegment Segment
    {
        get
        {
            var segment =
                GameSegmentExtensions.FromFlags(IsPcConsole, IsMobile);

            return segment == GameSegment.None
                ? GameSegment.PcConsole
                : segment;
        }
    }

    /// <summary>Fills the checkboxes from a saved game.</summary>
    public void SetSegment(GameSegment segment)
    {
        IsPcConsole = segment.Includes(GameSegment.PcConsole);

        IsMobile = segment.Includes(GameSegment.Mobile);
    }

    #endregion


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