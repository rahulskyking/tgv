namespace TheGameVoice.Application.Common.Dashboard;

public sealed class AuthorKpiData
{
    public Guid AuthorId { get; set; }

    public string AuthorName { get; set; } = default!;

    public int TotalArticles { get; set; }

    public int PublishedArticles { get; set; }

    public int DraftArticles { get; set; }

    public int ReviewPendingArticles { get; set; }

    public int ScheduledArticles { get; set; }

    public int RejectedArticles { get; set; }

    public long TotalViews { get; set; }

    public double AverageViewsPerPublishedArticle { get; set; }
}
