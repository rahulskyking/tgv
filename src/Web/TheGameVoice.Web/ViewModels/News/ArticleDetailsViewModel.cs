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

    /// <summary>
    /// Most-viewed published articles (by ViewCount), shown in the sidebar's
    /// "Trending Now" card. Kept separate from RelatedArticles, which is the
    /// contextual category/tag recommendation list.
    /// </summary>
    public IReadOnlyList<Article>
        TrendingArticles
    {
        get;
        set;
    }
        = new List<Article>();

    public string AuthorName { get; set; }
        = string.Empty;

    public string? AuthorAvatarUrl { get; set; }
}