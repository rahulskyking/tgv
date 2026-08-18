namespace TheGameVoice.Application.Common.Dashboard;

public sealed class DashboardKpiData
{
    public int TotalArticles { get; set; }

    public int PublishedArticles { get; set; }

    public int ScheduledArticles { get; set; }

    public int PendingReviewArticles { get; set; }

    public int DraftArticles { get; set; }

    public int RejectedArticles { get; set; }

    public int ArchivedArticles { get; set; }

    public long TotalViews { get; set; }

    /// <summary>Articles created within the selected date range (CreatedAt).</summary>
    public int CreatedInPeriod { get; set; }

    /// <summary>Articles published within the selected date range (PublishedAt).</summary>
    public int PublishedInPeriod { get; set; }

    public string? NextScheduledTitle { get; set; }

    public DateTime? NextScheduledAtUtc { get; set; }
}
