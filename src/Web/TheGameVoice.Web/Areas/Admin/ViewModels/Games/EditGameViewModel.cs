using System.ComponentModel.DataAnnotations;
using TheGameVoice.Web.Areas.Admin.ViewModels.Media;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Games;

public class EditGameViewModel
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
        = default!;

    public string? Summary { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public Guid? CoverImageId { get; set; }

    public string? CoverImagePath { get; set; }

    public List<MediaPickerItemViewModel>
        MediaItems
    { get; set; }
    = new();
}