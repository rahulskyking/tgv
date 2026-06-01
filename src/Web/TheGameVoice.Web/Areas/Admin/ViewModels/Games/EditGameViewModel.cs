using System.ComponentModel.DataAnnotations;

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
}