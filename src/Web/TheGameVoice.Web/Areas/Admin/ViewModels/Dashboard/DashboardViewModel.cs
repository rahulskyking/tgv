using TheGameVoice.Application.Common.Dashboard;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Dashboard;

public class DashboardViewModel
{
    public DashboardKpiViewModel Kpis { get; set; } = new();

    public ArticlePerformanceViewModel Performance { get; set; } = new();

    public IReadOnlyList<WorkflowStatusViewModel> Workflow { get; set; }
        = Array.Empty<WorkflowStatusViewModel>();

    public ScheduleHealthViewModel ScheduleHealth { get; set; } = new();

    public IReadOnlyList<UpcomingArticleViewModel> UpcomingPublications
    { get; set; } = Array.Empty<UpcomingArticleViewModel>();

    public IReadOnlyList<MostReadArticleViewModel> MostReadArticles
    { get; set; } = Array.Empty<MostReadArticleViewModel>();

    public IReadOnlyList<AuthorPerformanceViewModel> Authors { get; set; }
        = Array.Empty<AuthorPerformanceViewModel>();

    public IReadOnlyList<DashboardActivityViewModel> Activity { get; set; }
        = Array.Empty<DashboardActivityViewModel>();

    /// <summary>Currently selected date range, e.g. <c>Last30Days</c>.</summary>
    public string SelectedRange { get; set; }
        = DashboardDateRange.Last30Days.ToString();

    public string SelectedRangeLabel { get; set; } = "Last 30 Days";

    public bool ScopeToAuthor { get; set; }

    public string? ScopedAuthorName { get; set; }

    public bool LoadFailed { get; set; }

    public DateTime GeneratedAtUtc { get; set; }
}
