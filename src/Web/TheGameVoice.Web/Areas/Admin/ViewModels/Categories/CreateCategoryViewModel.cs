using System.ComponentModel.DataAnnotations;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Categories;

public class CreateCategoryViewModel
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
        = default!;
}