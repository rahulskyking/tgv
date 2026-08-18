namespace TheGameVoice.Application.Modules.Articles;

/// <summary>
/// Aggregate counters for the admin article list. Computed in the database
/// with the current filters applied (status is intentionally ignored so the
/// cards always show the full breakdown of the current selection).
/// </summary>
public sealed class ArticleStatsSummary
{
    public int TotalArticles { get; set; }

    public int PublishedArticles { get; set; }

    public int DraftArticles { get; set; }

    public int ReviewPendingArticles { get; set; }

    public int ScheduledArticles { get; set; }

    public int RejectedArticles { get; set; }

    public int ArchivedArticles { get; set; }

    /// <summary>Total views across every article in the current selection.</summary>
    public long TotalViews { get; set; }

    /// <summary>Total views across published articles only.</summary>
    public long PublishedViews { get; set; }

    public double AverageViewsPerPublishedArticle =>
        PublishedArticles > 0
            ? (double)PublishedViews / PublishedArticles
            : 0;
}
