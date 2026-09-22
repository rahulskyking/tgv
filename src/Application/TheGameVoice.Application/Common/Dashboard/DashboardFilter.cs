namespace TheGameVoice.Application.Common.Dashboard;

public enum DashboardDateRange
{
    Last7Days = 7,

    Last30Days = 30,

    Last90Days = 90
}

public sealed class DashboardFilter
{
    public DashboardDateRange DateRange { get; set; }
        = DashboardDateRange.Last30Days;

    /// <summary>
    /// Optional inclusive calendar dates in the dashboard timezone (IST).
    /// When both values are supplied they take precedence over DateRange.
    /// </summary>
    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    /// <summary>
    /// When <c>true</c>, every dashboard metric is scoped to the articles
    /// authored by <see cref="AuthorId"/> (used for the Author role).
    /// </summary>
    public bool ScopeToAuthor { get; set; }

    public Guid? AuthorId { get; set; }
}
