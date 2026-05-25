using System.ComponentModel.DataAnnotations;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Articles;

public class CreateArticleViewModel
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
}