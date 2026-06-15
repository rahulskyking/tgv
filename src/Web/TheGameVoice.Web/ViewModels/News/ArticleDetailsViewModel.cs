using TheGameVoice.Domain.Entities;
namespace TheGameVoice.Web.ViewModels.News; 
public class ArticleDetailsViewModel {
    public Article Article { get; set; }
 = default!;

    public IReadOnlyList<Article>
        RelatedArticles
    {
        get;
        set;
    }
        = new List<Article>();

    public string AuthorName { get; set; }
        = string.Empty;

    public string? AuthorAvatarUrl { get; set; }
}