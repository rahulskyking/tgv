using System.ComponentModel.DataAnnotations;
using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.HomepagePins;

public class HomepagePinRowViewModel
{
    public Article Article { get; init; } = default!;
    public string PcPlacement { get; init; } = string.Empty;
    public string MobilePlacement { get; init; } = string.Empty;
}

// Only pin metadata may be updated from the dedicated manager.
public class UpdateHomepagePinViewModel
{
    public Guid Id { get; set; }

    [Required]
    [Range(1, 9999)]
    public int? HomepagePinOrder { get; set; }

    [Required]
    [Range(1, 9999)]
    public int? ExpectedOrder { get; set; }
}
