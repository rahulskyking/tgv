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
}
