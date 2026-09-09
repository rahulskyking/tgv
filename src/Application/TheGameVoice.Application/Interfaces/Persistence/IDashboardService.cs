using TheGameVoice.Application.Common.Dashboard;

namespace TheGameVoice.Application.Interfaces.Persistence;

/// <summary>
/// Builds the admin editorial dashboard from the existing persistence model.
/// Implementations must aggregate in the database (no full-table loads).
/// </summary>
public interface IDashboardService
{
    Task<DashboardData> GetDashboardAsync(
        DashboardFilter filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Drill-down statistics for a single author.
    /// Returns <c>null</c> when the author does not exist.
    /// </summary>
    Task<AuthorStatsData?> GetAuthorStatsAsync(
        Guid authorId,
        DashboardFilter filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lifetime publishing KPIs for every author with at least one article,
    /// aggregated in a single database pass. Used to render per-user stats on
    /// the admin Users list without N+1 queries.
    /// </summary>
    Task<IReadOnlyList<AuthorKpiData>> GetAuthorPerformanceAsync(
        CancellationToken cancellationToken = default);
}
