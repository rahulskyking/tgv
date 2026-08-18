namespace TheGameVoice.Application.Common.Dashboard;

/// <summary>
/// Aggregated dashboard payload returned by <see cref="TheGameVoice.Application.Interfaces.Persistence.IDashboardService"/>.
/// All values are computed by the database; no full Article table is ever loaded.
/// </summary>
public sealed class DashboardData
{
    public DashboardKpiData Kpis { get; set; } = new();

    public ArticlePerformanceData Performance { get; set; } = new();

    public IReadOnlyList<WorkflowStatusData> Workflow
    { get; set; } = Array.Empty<WorkflowStatusData>();

    public ScheduleHealthData ScheduleHealth { get; set; } = new();

    public IReadOnlyList<UpcomingArticleData> UpcomingPublications
    { get; set; } = Array.Empty<UpcomingArticleData>();

    public IReadOnlyList<MostReadArticleData> MostReadArticles
    { get; set; } = Array.Empty<MostReadArticleData>();

    public IReadOnlyList<AuthorKpiData> AuthorPerformance
    { get; set; } = Array.Empty<AuthorKpiData>();

    public IReadOnlyList<DashboardActivityData> RecentActivity
    { get; set; } = Array.Empty<DashboardActivityData>();

    public DashboardDateRange DateRange { get; set; }
        = DashboardDateRange.Last30Days;

    public DateTime PeriodStartUtc { get; set; }

    public DateTime GeneratedAtUtc { get; set; } = DateTime.UtcNow;

    public bool ScopeToAuthor { get; set; }

    public Guid? ScopedAuthorId { get; set; }
}
