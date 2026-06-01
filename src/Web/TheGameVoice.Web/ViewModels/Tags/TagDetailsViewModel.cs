using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Web.ViewModels.Tags;

public class TagDetailsViewModel
{
    public Tag Tag { get; set; }
        = default!;

    public IReadOnlyList<Article>
        Articles
    { get; set; }
        = new List<Article>();
}