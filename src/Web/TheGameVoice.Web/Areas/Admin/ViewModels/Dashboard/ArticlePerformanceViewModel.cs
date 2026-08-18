namespace TheGameVoice.Web.Areas.Admin.ViewModels.Dashboard;

public class ArticlePerformanceViewModel
{
    public long TotalViews { get; set; }

    public int PublishedArticles { get; set; }

    public double AverageViewsPerArticle { get; set; }

    public Guid? MostReadArticleId { get; set; }

    public string? MostReadArticleTitle { get; set; }

    public int MostReadArticleViews { get; set; }
}
