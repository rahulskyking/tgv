using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Web.ViewModels.Home;

public class HomePageViewModel
{
    public IReadOnlyList<Article> LatestNews
    { get; set; }
        = new List<Article>();

    public IReadOnlyList<Article> Reviews
    { get; set; }
        = new List<Article>();

    public Article? HeroArticle
    { get; set; }

    public IReadOnlyList<Game> TrendingGames
    { get; set; }
    = new List<Game>();

    public Article? FeaturedReview
    { get; set; }

    public List<Article> LatestReviews
    { get; set; }
= new();
    public IReadOnlyList<Article>
    TrendingArticles
    {
        get;
        set;
    }
= new List<Article>();
}