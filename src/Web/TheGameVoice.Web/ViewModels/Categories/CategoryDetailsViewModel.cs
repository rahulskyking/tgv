using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Web.ViewModels.Categories;

public class CategoryDetailsViewModel
{
    public Category Category { get; set; }
        = default!;

    public IReadOnlyList<Article> Articles
    { get; set; }
        = new List<Article>();
}