using System.ComponentModel.DataAnnotations;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Tags;

public class CreateTagViewModel
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
        = default!;
}