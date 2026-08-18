namespace TheGameVoice.Web.Areas.Admin.ViewModels.Dashboard;

public class DashboardKpiViewModel
{
    public int TotalArticles { get; set; }

    public int PublishedArticles { get; set; }

    public int ScheduledArticles { get; set; }

    public int PendingReviewArticles { get; set; }

    public int DraftArticles { get; set; }

    public int RejectedArticles { get; set; }

    public int ArchivedArticles { get; set; }

    public long TotalViews { get; set; }

    public int CreatedInPeriod { get; set; }

    public int PublishedInPeriod { get; set; }

    public double PublishedPercentage { get; set; }

    public string? NextScheduledTitle { get; set; }

    public DateTime? NextScheduledAtUtc { get; set; }
}
