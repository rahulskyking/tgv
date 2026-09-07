using System.ComponentModel.DataAnnotations;
using TheGameVoice.Domain.Common.Extensions;
using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Categories;

public class CreateCategoryViewModel
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
        = default!;

    /// <summary>Show this category in the PC / Console section.</summary>
    [Display(Name = "PC / Console games")]
    public bool IsPcConsole { get; set; } = true;

    /// <summary>Show this category in the Mobile section.</summary>
    [Display(Name = "Mobile games")]
    public bool IsMobile { get; set; } = true;

    /// <summary>
    /// Sections this category appears in. Falls back to both so a category
    /// can never become unreachable by accident.
    /// </summary>
    public GameSegment Segment
    {
        get
        {
            var segment =
                GameSegmentExtensions.FromFlags(IsPcConsole, IsMobile);

            return segment == GameSegment.None
                ? GameSegment.All
                : segment;
        }
    }
}
