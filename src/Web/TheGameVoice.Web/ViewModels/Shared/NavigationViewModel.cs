using TheGameVoice.Domain.Entities;
using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Web.ViewModels.Shared;

public class NavigationViewModel
{
    public IReadOnlyList<Category> Categories
    { get; set; }
        = new List<Category>();

    /// <summary>Site mode the header is rendered for.</summary>
    public GameSegment CurrentSegment { get; set; }
        = GameSegment.PcConsole;

    /// <summary>
    /// Path the reader should come back to after flipping the mode
    /// (so the toggle does not always dump them on the homepage).
    /// </summary>
    public string ReturnUrl { get; set; } = "/";
}
