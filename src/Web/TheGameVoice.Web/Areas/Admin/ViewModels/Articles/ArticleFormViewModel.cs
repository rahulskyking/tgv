using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TheGameVoice.Web.Areas.Admin.ViewModels.Media;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Articles;

public class ArticleFormViewModel
{
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = default!;

    [Required]
    public string Summary { get; set; } = default!;

    [Required]
    public string Content { get; set; } = default!;

    public string? SeoTitle { get; set; }

    public string? SeoDescription { get; set; }

    public Guid? FeaturedImageId { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    public List<Guid> SelectedTagIds { get; set; }
        = new();

    public List<Guid> SelectedGameIds { get; set; }
        = new();

    public string? FeaturedImagePath { get; set; }


    public List<SelectListItem> Categories { get; set; }
    = new();

    public List<SelectListItem> Tags { get; set; }
        = new();

    public List<SelectListItem> Games { get; set; }
        = new();

    public List<MediaPickerItemViewModel> MediaItems { get; set; }
        = new();
}