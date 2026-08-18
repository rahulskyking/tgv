namespace TheGameVoice.Application.Common.Dashboard;

public sealed class ArticlePerformanceData
{
    public long TotalViews { get; set; }

    public int PublishedArticles { get; set; }

    public double AverageViewsPerArticle { get; set; }

    public Guid? MostReadArticleId { get; set; }

    public string? MostReadArticleTitle { get; set; }

    public int MostReadArticleViews { get; set; }
}
