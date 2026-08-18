using Microsoft.EntityFrameworkCore;
using TheGameVoice.Application.Common.Dashboard;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Infrastructure.Persistence.Context;

namespace TheGameVoice.Infrastructure.Services;

/// <summary>
/// Aggregates the admin dashboard straight from <see cref="AppDbContext"/>.
/// Every read is <c>AsNoTracking</c>, filtered, and projected so PostgreSQL does the
/// heavy lifting (GROUP BY / COUNT / SUM / LIMIT) instead of loading articles into memory.
/// </summary>
public class DashboardService : IDashboardService
{
    private static readonly TimeZoneInfo IndiaTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows()
                ? "India Standard Time"
                : "Asia/Kolkata");

    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardData> GetDashboardAsync(
        DashboardFilter filter,
        CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;
        var periodStart = utcNow.AddDays(-(int)filter.DateRange);

        var scoped = _context.Articles.AsNoTracking();

        if (filter.ScopeToAuthor && filter.AuthorId.HasValue)
        {
            var authorId = filter.AuthorId.Value;
            scoped = scoped.Where(a => a.AuthorId == authorId);
        }

        // ---- KPI counts: one grouped scan over the Status index ----
        var statusCounts = await scoped
            .GroupBy(a => a.Status)
            .Select(g => new WorkflowStatusData
            {
                Status = g.Key,
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        var countsByStatus =
            statusCounts.ToDictionary(x => x.Status, x => x.Count);

        var totalArticles = countsByStatus.Values.Sum();

        var totalViews = await scoped
            .SumAsync(a => (long)a.ViewCount, cancellationToken);

        var createdInPeriod = await scoped
            .CountAsync(a => a.CreatedAt >= periodStart, cancellationToken);

        var publishedInPeriod = await scoped
            .CountAsync(a => a.PublishedAt >= periodStart, cancellationToken);

        // ---- Upcoming scheduled publications (next 5, future only) ----
        var upcoming = await scoped
            .Where(a =>
                a.Status == ArticleStatus.Scheduled &&
                a.ScheduledPublishAt > utcNow)
            .OrderBy(a => a.ScheduledPublishAt)
            .Take(5)
            .Select(a => new UpcomingArticleData
            {
                Id = a.Id,
                Title = a.Title,
                Slug = a.Slug,
                ScheduledPublishAtUtc = a.ScheduledPublishAt!.Value,
                AuthorId = a.AuthorId,
                CategoryId = a.CategoryId
            })
            .ToListAsync(cancellationToken);

        var nextUpcoming = upcoming.FirstOrDefault();

        // ---- Schedule health (IST calendar day boundaries) ----
        var (todayStartUtc, tomorrowStartUtc) =
            GetIstDayBoundaries(utcNow);

        var dueTodayCount = await scoped
            .CountAsync(a =>
                a.Status == ArticleStatus.Scheduled &&
                a.ScheduledPublishAt >= todayStartUtc &&
                a.ScheduledPublishAt < tomorrowStartUtc,
                cancellationToken);

        var dueTomorrowCount = await scoped
            .CountAsync(a =>
                a.Status == ArticleStatus.Scheduled &&
                a.ScheduledPublishAt >= tomorrowStartUtc &&
                a.ScheduledPublishAt < tomorrowStartUtc.AddDays(1),
                cancellationToken);

        var overdueCount = await scoped
            .CountAsync(a =>
                a.Status == ArticleStatus.Scheduled &&
                a.ScheduledPublishAt < utcNow,
                cancellationToken);

        // ---- Most read: published articles ordered by ViewCount ----
        var mostRead = await scoped
            .Where(a => a.Status == ArticleStatus.Published)
            .OrderByDescending(a => a.ViewCount)
            .ThenByDescending(a => a.PublishedAt)
            .Take(5)
            .Select(a => new MostReadArticleData
            {
                Id = a.Id,
                Title = a.Title,
                Slug = a.Slug,
                CategoryId = a.CategoryId,
                ViewCount = a.ViewCount,
                PublishedAtUtc = a.PublishedAt
            })
            .ToListAsync(cancellationToken);

        // ---- Author performance: one grouped aggregation per author ----
        var authorAggregates = await scoped
            .GroupBy(a => a.AuthorId)
            .Select(g => new
            {
                AuthorId = g.Key,
                TotalArticles = g.Count(),
                PublishedArticles =
                    g.Count(a => a.Status == ArticleStatus.Published),
                DraftArticles =
                    g.Count(a => a.Status == ArticleStatus.Draft),
                ReviewPendingArticles =
                    g.Count(a => a.Status == ArticleStatus.ReviewPending),
                ScheduledArticles =
                    g.Count(a => a.Status == ArticleStatus.Scheduled),
                RejectedArticles =
                    g.Count(a => a.Status == ArticleStatus.Rejected),
                TotalViews = g.Sum(a => (long)a.ViewCount)
            })
            .ToListAsync(cancellationToken);

        // ---- Recent activity: derived from existing article timestamps ----
        var activityRows = new List<DashboardActivityRow>();

        activityRows.AddRange(await scoped
            .Where(a =>
                a.Status == ArticleStatus.Published &&
                a.PublishedAt != null)
            .OrderByDescending(a => a.PublishedAt)
            .Take(10)
            .Select(a => new DashboardActivityRow
            {
                Type = DashboardActivityType.Published,
                ArticleId = a.Id,
                ArticleTitle = a.Title,
                ActorId = a.PublishedById,
                OccurredAtUtc = a.PublishedAt!.Value
            })
            .ToListAsync(cancellationToken));

        activityRows.AddRange(await scoped
            .Where(a => a.Status == ArticleStatus.ReviewPending)
            .OrderByDescending(a =>
                a.LastModifiedAt ?? a.UpdatedAt ?? a.CreatedAt)
            .Take(10)
            .Select(a => new DashboardActivityRow
            {
                Type = DashboardActivityType.SubmittedForReview,
                ArticleId = a.Id,
                ArticleTitle = a.Title,
                ActorId = null,
                OccurredAtUtc =
                    a.LastModifiedAt ?? a.UpdatedAt ?? a.CreatedAt
            })
            .ToListAsync(cancellationToken));

        activityRows.AddRange(await scoped
            .Where(a => a.Status == ArticleStatus.Scheduled)
            .OrderByDescending(a =>
                a.LastModifiedAt ?? a.UpdatedAt ?? a.CreatedAt)
            .Take(10)
            .Select(a => new DashboardActivityRow
            {
                Type = DashboardActivityType.Scheduled,
                ArticleId = a.Id,
                ArticleTitle = a.Title,
                ActorId = a.ScheduledById,
                OccurredAtUtc =
                    a.LastModifiedAt ?? a.UpdatedAt ?? a.CreatedAt
            })
            .ToListAsync(cancellationToken));

        activityRows.AddRange(await scoped
            .Where(a => a.Status == ArticleStatus.Rejected)
            .OrderByDescending(a =>
                a.LastModifiedAt ?? a.UpdatedAt ?? a.CreatedAt)
            .Take(10)
            .Select(a => new DashboardActivityRow
            {
                Type = DashboardActivityType.Rejected,
                ArticleId = a.Id,
                ArticleTitle = a.Title,
                ActorId = null,
                OccurredAtUtc =
                    a.LastModifiedAt ?? a.UpdatedAt ?? a.CreatedAt
            })
            .ToListAsync(cancellationToken));

        activityRows.AddRange(await scoped
            .Where(a => a.Status == ArticleStatus.Archived)
            .OrderByDescending(a =>
                a.LastModifiedAt ?? a.UpdatedAt ?? a.CreatedAt)
            .Take(10)
            .Select(a => new DashboardActivityRow
            {
                Type = DashboardActivityType.Archived,
                ArticleId = a.Id,
                ArticleTitle = a.Title,
                ActorId = null,
                OccurredAtUtc =
                    a.LastModifiedAt ?? a.UpdatedAt ?? a.CreatedAt
            })
            .ToListAsync(cancellationToken));

        activityRows.AddRange(await scoped
            .Where(a => a.Status == ArticleStatus.Draft)
            .OrderByDescending(a => a.CreatedAt)
            .Take(10)
            .Select(a => new DashboardActivityRow
            {
                Type = DashboardActivityType.Created,
                ArticleId = a.Id,
                ArticleTitle = a.Title,
                ActorId = null,
                OccurredAtUtc = a.CreatedAt
            })
            .ToListAsync(cancellationToken));

        // ---- Lookups: only the ids we actually need ----
        var userIds = authorAggregates
            .Select(x => x.AuthorId)
            .Concat(upcoming.Select(x => x.AuthorId))
            .Concat(activityRows
                .Where(x => x.ActorId.HasValue)
                .Select(x => x.ActorId!.Value))
            .Distinct()
            .ToList();

        var userNames = new Dictionary<Guid, string>();

        if (userIds.Count > 0)
        {
            var users = await _context.Users
                .AsNoTracking()
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync(cancellationToken);

            userNames = users.ToDictionary(u => u.Id, u => u.FullName);
        }

        var categoryIds = upcoming
            .Select(x => x.CategoryId)
            .Concat(mostRead.Select(x => x.CategoryId))
            .Distinct()
            .ToList();

        var categoryNames = new Dictionary<Guid, string>();

        if (categoryIds.Count > 0)
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => categoryIds.Contains(c.Id))
                .Select(c => new { c.Id, c.Name })
                .ToListAsync(cancellationToken);

            categoryNames =
                categories.ToDictionary(c => c.Id, c => c.Name);
        }

        // ---- Compose ----
        foreach (var item in upcoming)
        {
            userNames.TryGetValue(item.AuthorId, out var authorName);
            item.AuthorName = authorName;

            categoryNames.TryGetValue(
                item.CategoryId, out var categoryName);
            item.CategoryName = categoryName;
        }

        foreach (var item in mostRead)
        {
            categoryNames.TryGetValue(
                item.CategoryId, out var categoryName);
            item.CategoryName = categoryName;
        }

        var publishedCount =
            GetCount(countsByStatus, ArticleStatus.Published);

        var authorPerformance = authorAggregates
            .Select(x =>
            {
                userNames.TryGetValue(x.AuthorId, out var name);

                return new AuthorKpiData
                {
                    AuthorId = x.AuthorId,
                    AuthorName =
                        string.IsNullOrWhiteSpace(name)
                            ? "Unknown Author"
                            : name,
                    TotalArticles = x.TotalArticles,
                    PublishedArticles = x.PublishedArticles,
                    DraftArticles = x.DraftArticles,
                    ReviewPendingArticles = x.ReviewPendingArticles,
                    ScheduledArticles = x.ScheduledArticles,
                    RejectedArticles = x.RejectedArticles,
                    TotalViews = x.TotalViews,
                    AverageViewsPerPublishedArticle =
                        x.PublishedArticles > 0
                            ? (double)x.TotalViews / x.PublishedArticles
                            : 0
                };
            })
            .OrderByDescending(x => x.PublishedArticles)
            .ThenByDescending(x => x.TotalViews)
            .Take(8)
            .ToList();

        var recentActivity = activityRows
            .Where(x => x.OccurredAtUtc >= periodStart)
            .OrderByDescending(x => x.OccurredAtUtc)
            .Take(10)
            .Select(x => new DashboardActivityData
            {
                Type = x.Type,
                ArticleId = x.ArticleId,
                ArticleTitle = x.ArticleTitle,
                ActorName =
                    x.ActorId.HasValue &&
                    userNames.TryGetValue(x.ActorId.Value, out var actor)
                        ? actor
                        : null,
                OccurredAtUtc = x.OccurredAtUtc
            })
            .ToList();

        return new DashboardData
        {
            Kpis = new DashboardKpiData
            {
                TotalArticles = totalArticles,
                PublishedArticles = publishedCount,
                ScheduledArticles =
                    GetCount(countsByStatus, ArticleStatus.Scheduled),
                PendingReviewArticles =
                    GetCount(countsByStatus, ArticleStatus.ReviewPending),
                DraftArticles =
                    GetCount(countsByStatus, ArticleStatus.Draft),
                RejectedArticles =
                    GetCount(countsByStatus, ArticleStatus.Rejected),
                ArchivedArticles =
                    GetCount(countsByStatus, ArticleStatus.Archived),
                TotalViews = totalViews,
                CreatedInPeriod = createdInPeriod,
                PublishedInPeriod = publishedInPeriod,
                NextScheduledTitle = nextUpcoming?.Title,
                NextScheduledAtUtc = nextUpcoming?.ScheduledPublishAtUtc
            },
            Performance = new ArticlePerformanceData
            {
                TotalViews = totalViews,
                PublishedArticles = publishedCount,
                AverageViewsPerArticle =
                    publishedCount > 0
                        ? (double)totalViews / publishedCount
                        : 0,
                MostReadArticleId = mostRead.FirstOrDefault()?.Id,
                MostReadArticleTitle = mostRead.FirstOrDefault()?.Title,
                MostReadArticleViews =
                    mostRead.FirstOrDefault()?.ViewCount ?? 0
            },
            Workflow = WorkflowOrder
                .Select(status => new WorkflowStatusData
                {
                    Status = status,
                    Count = GetCount(countsByStatus, status)
                })
                .ToList(),
            ScheduleHealth = new ScheduleHealthData
            {
                ScheduledCount =
                    GetCount(countsByStatus, ArticleStatus.Scheduled),
                DueTodayCount = dueTodayCount,
                DueTomorrowCount = dueTomorrowCount,
                OverdueCount = overdueCount,
                NextScheduledAtUtc = nextUpcoming?.ScheduledPublishAtUtc,
                NextScheduledTitle = nextUpcoming?.Title
            },
            UpcomingPublications = upcoming,
            MostReadArticles = mostRead,
            AuthorPerformance = authorPerformance,
            RecentActivity = recentActivity,
            DateRange = filter.DateRange,
            PeriodStartUtc = periodStart,
            GeneratedAtUtc = utcNow,
            ScopeToAuthor = filter.ScopeToAuthor,
            ScopedAuthorId = filter.AuthorId
        };
    }

    public async Task<AuthorStatsData?> GetAuthorStatsAsync(
        Guid authorId,
        DashboardFilter filter,
        CancellationToken cancellationToken = default)
    {
        var author = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == authorId)
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.Slug,
                u.Bio,
                u.IsActive,
                u.AvatarImageId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (author is null)
        {
            return null;
        }

        var utcNow = DateTime.UtcNow;
        var periodStart = utcNow.AddDays(-(int)filter.DateRange);

        var scoped = _context.Articles
            .AsNoTracking()
            .Where(a => a.AuthorId == authorId);

        // ---- Lifetime status breakdown (single grouped scan) ----
        var statusCounts = await scoped
            .GroupBy(a => a.Status)
            .Select(g => new WorkflowStatusData
            {
                Status = g.Key,
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        var countsByStatus =
            statusCounts.ToDictionary(x => x.Status, x => x.Count);

        var totalArticles = countsByStatus.Values.Sum();

        var publishedCount =
            GetCount(countsByStatus, ArticleStatus.Published);

        var totalViews = await scoped
            .SumAsync(a => (long)a.ViewCount, cancellationToken);

        var createdInPeriod = await scoped
            .CountAsync(a => a.CreatedAt >= periodStart, cancellationToken);

        var publishedInPeriod = await scoped
            .CountAsync(a => a.PublishedAt >= periodStart, cancellationToken);

        var firstPublishedAt = await scoped
            .Where(a => a.PublishedAt != null)
            .MinAsync(a => (DateTime?)a.PublishedAt, cancellationToken);

        var lastPublishedAt = await scoped
            .Where(a => a.PublishedAt != null)
            .MaxAsync(a => (DateTime?)a.PublishedAt, cancellationToken);

        // ---- Top performing articles ----
        var topArticles = await scoped
            .Where(a => a.Status == ArticleStatus.Published)
            .OrderByDescending(a => a.ViewCount)
            .ThenByDescending(a => a.PublishedAt)
            .Take(10)
            .Select(a => new MostReadArticleData
            {
                Id = a.Id,
                Title = a.Title,
                Slug = a.Slug,
                CategoryId = a.CategoryId,
                ViewCount = a.ViewCount,
                PublishedAtUtc = a.PublishedAt
            })
            .ToListAsync(cancellationToken);

        // ---- Most recently touched articles (any status) ----
        var recentArticles = await scoped
            .OrderByDescending(a =>
                a.LastModifiedAt ?? a.UpdatedAt ?? a.CreatedAt)
            .Take(10)
            .Select(a => new AuthorArticleData
            {
                Id = a.Id,
                Title = a.Title,
                Slug = a.Slug,
                Status = a.Status,
                CategoryId = a.CategoryId,
                ViewCount = a.ViewCount,
                CreatedAtUtc = a.CreatedAt,
                PublishedAtUtc = a.PublishedAt,
                ScheduledPublishAtUtc = a.ScheduledPublishAt
            })
            .ToListAsync(cancellationToken);

        // ---- Upcoming scheduled publications for this author ----
        var upcoming = await scoped
            .Where(a =>
                a.Status == ArticleStatus.Scheduled &&
                a.ScheduledPublishAt > utcNow)
            .OrderBy(a => a.ScheduledPublishAt)
            .Take(5)
            .Select(a => new UpcomingArticleData
            {
                Id = a.Id,
                Title = a.Title,
                Slug = a.Slug,
                ScheduledPublishAtUtc = a.ScheduledPublishAt!.Value,
                AuthorId = a.AuthorId,
                CategoryId = a.CategoryId
            })
            .ToListAsync(cancellationToken);

        // ---- Category distribution ----
        var categoryBreakdown = await scoped
            .GroupBy(a => a.CategoryId)
            .Select(g => new AuthorCategoryBreakdownData
            {
                CategoryId = g.Key,
                TotalArticles = g.Count(),
                PublishedArticles =
                    g.Count(a => a.Status == ArticleStatus.Published),
                TotalViews = g.Sum(a => (long)a.ViewCount)
            })
            .ToListAsync(cancellationToken);

        // ---- Publishing trend: only the timestamps, bucketed in memory ----
        var publishedTimestamps = await scoped
            .Where(a =>
                a.Status == ArticleStatus.Published &&
                a.PublishedAt != null &&
                a.PublishedAt >= periodStart)
            .Select(a => a.PublishedAt!.Value)
            .ToListAsync(cancellationToken);

        var trend = BuildTrend(publishedTimestamps, periodStart, utcNow);

        // ---- Ranking against every other author ----
        var allAuthorAggregates = await _context.Articles
            .AsNoTracking()
            .GroupBy(a => a.AuthorId)
            .Select(g => new
            {
                AuthorId = g.Key,
                PublishedArticles =
                    g.Count(a => a.Status == ArticleStatus.Published),
                TotalViews = g.Sum(a => (long)a.ViewCount)
            })
            .ToListAsync(cancellationToken);

        var rankByViews = allAuthorAggregates
            .OrderByDescending(x => x.TotalViews)
            .ToList()
            .FindIndex(x => x.AuthorId == authorId) + 1;

        var rankByPublished = allAuthorAggregates
            .OrderByDescending(x => x.PublishedArticles)
            .ToList()
            .FindIndex(x => x.AuthorId == authorId) + 1;

        // ---- Lookups ----
        var categoryIds = categoryBreakdown
            .Select(x => x.CategoryId)
            .Concat(topArticles.Select(x => x.CategoryId))
            .Concat(recentArticles.Select(x => x.CategoryId))
            .Concat(upcoming.Select(x => x.CategoryId))
            .Distinct()
            .ToList();

        var categoryNames = new Dictionary<Guid, string>();

        if (categoryIds.Count > 0)
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => categoryIds.Contains(c.Id))
                .Select(c => new { c.Id, c.Name })
                .ToListAsync(cancellationToken);

            categoryNames =
                categories.ToDictionary(c => c.Id, c => c.Name);
        }

        foreach (var item in topArticles)
        {
            categoryNames.TryGetValue(item.CategoryId, out var name);
            item.CategoryName = name;
        }

        foreach (var item in recentArticles)
        {
            categoryNames.TryGetValue(item.CategoryId, out var name);
            item.CategoryName = name;
        }

        foreach (var item in upcoming)
        {
            categoryNames.TryGetValue(item.CategoryId, out var name);
            item.CategoryName = name;
            item.AuthorName = author.FullName;
        }

        foreach (var item in categoryBreakdown)
        {
            categoryNames.TryGetValue(item.CategoryId, out var name);
            item.CategoryName = name;
        }

        var roles = await (
                from userRole in _context.UserRoles
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where userRole.UserId == authorId
                select role.Name)
            .ToListAsync(cancellationToken);

        string? avatarPath = null;

        if (author.AvatarImageId.HasValue)
        {
            avatarPath = await _context.Media
                .AsNoTracking()
                .Where(m => m.Id == author.AvatarImageId.Value)
                .Select(m => m.FilePath)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new AuthorStatsData
        {
            AuthorId = author.Id,
            AuthorName =
                string.IsNullOrWhiteSpace(author.FullName)
                    ? "Unknown Author"
                    : author.FullName,
            AuthorEmail = author.Email,
            AuthorSlug = author.Slug,
            Bio = author.Bio,
            AvatarPath = avatarPath,
            IsActive = author.IsActive,
            Roles = roles
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r!)
                .OrderBy(r => r)
                .ToList(),
            Kpis = new AuthorKpiData
            {
                AuthorId = author.Id,
                AuthorName = author.FullName,
                TotalArticles = totalArticles,
                PublishedArticles = publishedCount,
                DraftArticles =
                    GetCount(countsByStatus, ArticleStatus.Draft),
                ReviewPendingArticles =
                    GetCount(countsByStatus, ArticleStatus.ReviewPending),
                ScheduledArticles =
                    GetCount(countsByStatus, ArticleStatus.Scheduled),
                RejectedArticles =
                    GetCount(countsByStatus, ArticleStatus.Rejected),
                TotalViews = totalViews,
                AverageViewsPerPublishedArticle =
                    publishedCount > 0
                        ? (double)totalViews / publishedCount
                        : 0
            },
            ArchivedArticles =
                GetCount(countsByStatus, ArticleStatus.Archived),
            CreatedInPeriod = createdInPeriod,
            PublishedInPeriod = publishedInPeriod,
            Workflow = WorkflowOrder
                .Select(status => new WorkflowStatusData
                {
                    Status = status,
                    Count = GetCount(countsByStatus, status)
                })
                .ToList(),
            TopArticles = topArticles,
            RecentArticles = recentArticles,
            UpcomingPublications = upcoming,
            PublishingTrend = trend,
            CategoryBreakdown = categoryBreakdown
                .OrderByDescending(x => x.TotalArticles)
                .ThenByDescending(x => x.TotalViews)
                .ToList(),
            FirstPublishedAtUtc = firstPublishedAt,
            LastPublishedAtUtc = lastPublishedAt,
            RankByViews = rankByViews > 0 ? rankByViews : null,
            RankByPublished = rankByPublished > 0 ? rankByPublished : null,
            TotalAuthors = allAuthorAggregates.Count,
            SiteTotalViews = allAuthorAggregates.Sum(x => x.TotalViews),
            SitePublishedArticles =
                allAuthorAggregates.Sum(x => x.PublishedArticles),
            DateRange = filter.DateRange,
            PeriodStartUtc = periodStart,
            GeneratedAtUtc = utcNow
        };
    }

    private static List<AuthorTrendPointData> BuildTrend(
        IReadOnlyList<DateTime> publishedTimestampsUtc,
        DateTime periodStartUtc,
        DateTime utcNow)
    {
        var counts = new Dictionary<DateOnly, int>();

        foreach (var timestamp in publishedTimestampsUtc)
        {
            var day = DateOnly.FromDateTime(ToIst(timestamp));

            counts[day] = counts.TryGetValue(day, out var current)
                ? current + 1
                : 1;
        }

        var firstDay = DateOnly.FromDateTime(ToIst(periodStartUtc));
        var lastDay = DateOnly.FromDateTime(ToIst(utcNow));

        var trend = new List<AuthorTrendPointData>();

        for (var day = firstDay; day <= lastDay; day = day.AddDays(1))
        {
            trend.Add(new AuthorTrendPointData
            {
                Day = day,
                PublishedCount =
                    counts.TryGetValue(day, out var count) ? count : 0
            });
        }

        return trend;
    }

    private static DateTime ToIst(DateTime utc)
        => TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(utc, DateTimeKind.Utc),
            IndiaTimeZone);

    private static readonly ArticleStatus[] WorkflowOrder =
    {
        ArticleStatus.Draft,
        ArticleStatus.ReviewPending,
        ArticleStatus.Scheduled,
        ArticleStatus.Published,
        ArticleStatus.Rejected,
        ArticleStatus.Archived
    };

    private static int GetCount(
        IReadOnlyDictionary<ArticleStatus, int> counts,
        ArticleStatus status)
    {
        return counts.TryGetValue(status, out var count)
            ? count
            : 0;
    }

    private static (DateTime TodayStartUtc, DateTime TomorrowStartUtc)
        GetIstDayBoundaries(DateTime utcNow)
    {
        var istNow =
            TimeZoneInfo.ConvertTimeFromUtc(utcNow, IndiaTimeZone);

        var istToday = istNow.Date;
        var istTomorrow = istToday.AddDays(1);

        return (
            TimeZoneInfo.ConvertTimeToUtc(istToday, IndiaTimeZone),
            TimeZoneInfo.ConvertTimeToUtc(istTomorrow, IndiaTimeZone));
    }
}

/// <summary>
/// Lightweight projection used while assembling the activity feed.
/// EF Core materialises this shape straight from SQL (no entity tracking).
/// </summary>
internal sealed class DashboardActivityRow
{
    public DashboardActivityType Type { get; set; }

    public Guid ArticleId { get; set; }

    public string ArticleTitle { get; set; } = default!;

    public Guid? ActorId { get; set; }

    public DateTime OccurredAtUtc { get; set; }
}
