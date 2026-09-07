using TheGameVoice.Domain.Entities;
using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Web.ViewModels.Home;

public class HomePageViewModel
{
    /// <summary>Site mode this homepage was built for.</summary>
    public GameSegment Segment { get; set; }
        = GameSegment.PcConsole;

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