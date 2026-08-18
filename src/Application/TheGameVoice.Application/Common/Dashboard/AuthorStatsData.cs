using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Application.Common.Dashboard;

/// <summary>
/// A single article row shown on the individual author statistics page.
/// </summary>
public sealed class AuthorArticleData
{
    public Guid Id { get; set; }

    public string Title { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public ArticleStatus Status { get; set; }

    public Guid CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public int ViewCount { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? PublishedAtUtc { get; set; }

    public DateTime? ScheduledPublishAtUtc { get; set; }
}

/// <summary>
/// One bucket of the author's publishing trend (one calendar day, IST).
/// </summary>
public sealed class AuthorTrendPointData
{
    public DateOnly Day { get; set; }

    public int PublishedCount { get; set; }
}

/// <summary>
/// How an author's output is distributed across categories.
/// </summary>
public sealed class AuthorCategoryBreakdownData
{
    public Guid CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public int TotalArticles { get; set; }

    public int PublishedArticles { get; set; }

    public long TotalViews { get; set; }
}

/// <summary>
/// Full drill-down payload for a single author, returned by
/// <see cref="TheGameVoice.Application.Interfaces.Persistence.IDashboardService.GetAuthorStatsAsync"/>.
/// Everything is aggregated in the database; no full-table loads.
/// </summary>
public sealed class AuthorStatsData
{
    public Guid AuthorId { get; set; }

    public string AuthorName { get; set; } = default!;

    public string? AuthorEmail { get; set; }

    public string? AuthorSlug { get; set; }

    public string? Bio { get; set; }

    public string? AvatarPath { get; set; }

    public bool IsActive { get; set; } = true;

    public IReadOnlyList<string> Roles { get; set; }
        = Array.Empty<string>();

    /// <summary>Lifetime totals for this author (not limited to the period).</summary>
    public AuthorKpiData Kpis { get; set; } = new();

    public int ArchivedArticles { get; set; }

    /// <summary>Articles created by the author inside the selected period.</summary>
    public int CreatedInPeriod { get; set; }

    /// <summary>Articles published by the author inside the selected period.</summary>
    public int PublishedInPeriod { get; set; }

    public IReadOnlyList<WorkflowStatusData> Workflow { get; set; }
        = Array.Empty<WorkflowStatusData>();

    public IReadOnlyList<MostReadArticleData> TopArticles { get; set; }
        = Array.Empty<MostReadArticleData>();

    public IReadOnlyList<AuthorArticleData> RecentArticles { get; set; }
        = Array.Empty<AuthorArticleData>();

    public IReadOnlyList<UpcomingArticleData> UpcomingPublications { get; set; }
        = Array.Empty<UpcomingArticleData>();

    public IReadOnlyList<AuthorTrendPointData> PublishingTrend { get; set; }
        = Array.Empty<AuthorTrendPointData>();

    public IReadOnlyList<AuthorCategoryBreakdownData> CategoryBreakdown { get; set; }
        = Array.Empty<AuthorCategoryBreakdownData>();

    public DateTime? FirstPublishedAtUtc { get; set; }

    public DateTime? LastPublishedAtUtc { get; set; }

    /// <summary>1-based rank of this author by total views across all authors.</summary>
    public int? RankByViews { get; set; }

    /// <summary>1-based rank of this author by published article count.</summary>
    public int? RankByPublished { get; set; }

    public int TotalAuthors { get; set; }

    /// <summary>Site-wide total views, used to show the author's share.</summary>
    public long SiteTotalViews { get; set; }

    /// <summary>Site-wide published articles, used to show the author's share.</summary>
    public int SitePublishedArticles { get; set; }

    public DashboardDateRange DateRange { get; set; }
        = DashboardDateRange.Last30Days;

    public DateTime PeriodStartUtc { get; set; }

    public DateTime GeneratedAtUtc { get; set; } = DateTime.UtcNow;
}
