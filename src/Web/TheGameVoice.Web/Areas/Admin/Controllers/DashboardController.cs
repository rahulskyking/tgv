using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Common.Dashboard;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Infrastructure.Identity;
using TheGameVoice.Infrastructure.Identity.Entities;
using TheGameVoice.Web.Areas.Admin.ViewModels.Dashboard;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

public class DashboardController : BaseAdminController
{
    private readonly IDashboardService _dashboardService;

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IDashboardService dashboardService,
        UserManager<ApplicationUser> userManager,
        ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        DashboardDateRange? range = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = new DashboardFilter
            {
                DateRange = range ?? DashboardDateRange.Last30Days
            };

            var currentUser =
                await _userManager.GetUserAsync(User);

            if (User.IsInRole(Roles.Author) && currentUser is not null)
            {
                filter.ScopeToAuthor = true;
                filter.AuthorId = currentUser.Id;
            }

            var data = await _dashboardService
                .GetDashboardAsync(filter, cancellationToken);

            return View(Map(data, currentUser?.FullName));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to load the admin dashboard.");

            TempData["Error"] =
                "The dashboard could not be loaded. Please try again.";

            return View(new DashboardViewModel
            {
                LoadFailed = true,
                SelectedRange =
                    (range ?? DashboardDateRange.Last30Days).ToString()
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Export(
        DashboardDateRange? range = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = new DashboardFilter
            {
                DateRange = range ?? DashboardDateRange.Last30Days
            };

            var currentUser =
                await _userManager.GetUserAsync(User);

            if (User.IsInRole(Roles.Author) && currentUser is not null)
            {
                filter.ScopeToAuthor = true;
                filter.AuthorId = currentUser.Id;
            }

            var data = await _dashboardService
                .GetDashboardAsync(filter, cancellationToken);

            var csv = BuildDashboardCsv(data);

            var content = Encoding.UTF8.GetPreamble()
                .Concat(Encoding.UTF8.GetBytes(csv))
                .ToArray();

            return File(
                content,
                "text/csv",
                $"dashboard-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to export the admin dashboard.");

            TempData["Error"] =
                "The export could not be generated. Please try again.";

            return RedirectToAction(nameof(Index), new { range });
        }
    }

    private static DashboardViewModel Map(
        DashboardData data,
        string? scopedAuthorName)
    {
        var total = data.Kpis.TotalArticles;

        return new DashboardViewModel
        {
            Kpis = new DashboardKpiViewModel
            {
                TotalArticles = total,
                PublishedArticles = data.Kpis.PublishedArticles,
                ScheduledArticles = data.Kpis.ScheduledArticles,
                PendingReviewArticles =
                    data.Kpis.PendingReviewArticles,
                DraftArticles = data.Kpis.DraftArticles,
                RejectedArticles = data.Kpis.RejectedArticles,
                ArchivedArticles = data.Kpis.ArchivedArticles,
                TotalViews = data.Kpis.TotalViews,
                CreatedInPeriod = data.Kpis.CreatedInPeriod,
                PublishedInPeriod = data.Kpis.PublishedInPeriod,
                PublishedPercentage =
                    total > 0
                        ? (data.Kpis.PublishedArticles * 100d) / total
                        : 0,
                NextScheduledTitle = data.Kpis.NextScheduledTitle,
                NextScheduledAtUtc = data.Kpis.NextScheduledAtUtc
            },
            Performance = new ArticlePerformanceViewModel
            {
                TotalViews = data.Performance.TotalViews,
                PublishedArticles = data.Performance.PublishedArticles,
                AverageViewsPerArticle =
                    data.Performance.AverageViewsPerArticle,
                MostReadArticleId =
                    data.Performance.MostReadArticleId,
                MostReadArticleTitle =
                    data.Performance.MostReadArticleTitle,
                MostReadArticleViews =
                    data.Performance.MostReadArticleViews
            },
            Workflow = data.Workflow
                .Select(x => new WorkflowStatusViewModel
                {
                    Status = x.Status,
                    Count = x.Count
                })
                .ToList(),
            ScheduleHealth = new ScheduleHealthViewModel
            {
                ScheduledCount = data.ScheduleHealth.ScheduledCount,
                DueTodayCount = data.ScheduleHealth.DueTodayCount,
                DueTomorrowCount =
                    data.ScheduleHealth.DueTomorrowCount,
                OverdueCount = data.ScheduleHealth.OverdueCount,
                NextScheduledAtUtc =
                    data.ScheduleHealth.NextScheduledAtUtc,
                NextScheduledTitle =
                    data.ScheduleHealth.NextScheduledTitle
            },
            UpcomingPublications = data.UpcomingPublications
                .Select(x => new UpcomingArticleViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Slug = x.Slug,
                    ScheduledPublishAtUtc = x.ScheduledPublishAtUtc,
                    AuthorName = x.AuthorName,
                    CategoryName = x.CategoryName
                })
                .ToList(),
            MostReadArticles = data.MostReadArticles
                .Select(x => new MostReadArticleViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Slug = x.Slug,
                    CategoryName = x.CategoryName,
                    ViewCount = x.ViewCount,
                    PublishedAtUtc = x.PublishedAtUtc
                })
                .ToList(),
            Authors = data.AuthorPerformance
                .Select(x => new AuthorPerformanceViewModel
                {
                    AuthorId = x.AuthorId,
                    AuthorName = x.AuthorName,
                    TotalArticles = x.TotalArticles,
                    PublishedArticles = x.PublishedArticles,
                    DraftArticles = x.DraftArticles,
                    ReviewPendingArticles = x.ReviewPendingArticles,
                    ScheduledArticles = x.ScheduledArticles,
                    RejectedArticles = x.RejectedArticles,
                    TotalViews = x.TotalViews,
                    AverageViewsPerPublishedArticle =
                        x.AverageViewsPerPublishedArticle
                })
                .ToList(),
            Activity = data.RecentActivity
                .Select(x => new DashboardActivityViewModel
                {
                    Type = x.Type,
                    ArticleId = x.ArticleId,
                    ArticleTitle = x.ArticleTitle,
                    ActorName = x.ActorName,
                    OccurredAtUtc = x.OccurredAtUtc
                })
                .ToList(),
            SelectedRange = data.DateRange.ToString(),
            SelectedRangeLabel = RangeLabel(data.DateRange),
            ScopeToAuthor = data.ScopeToAuthor,
            ScopedAuthorName = scopedAuthorName,
            GeneratedAtUtc = data.GeneratedAtUtc
        };
    }

    private static string RangeLabel(DashboardDateRange range)
        => range switch
        {
            DashboardDateRange.Last7Days => "Last 7 Days",
            DashboardDateRange.Last90Days => "Last 90 Days",
            _ => "Last 30 Days"
        };

    private static string BuildDashboardCsv(DashboardData data)
    {
        var sb = new StringBuilder();

        sb.AppendLine("TheGameVoice Admin Dashboard");
        sb.AppendLine(
            $"Generated,{data.GeneratedAtUtc:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"Period,{RangeLabel(data.DateRange)}");
        sb.AppendLine(
            $"Period Start,{data.PeriodStartUtc:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine();

        sb.AppendLine("KPI,Value");
        sb.AppendLine($"Total Articles,{data.Kpis.TotalArticles}");
        sb.AppendLine($"Published,{data.Kpis.PublishedArticles}");
        sb.AppendLine($"Scheduled,{data.Kpis.ScheduledArticles}");
        sb.AppendLine($"Pending Review,{data.Kpis.PendingReviewArticles}");
        sb.AppendLine($"Draft,{data.Kpis.DraftArticles}");
        sb.AppendLine($"Rejected,{data.Kpis.RejectedArticles}");
        sb.AppendLine($"Archived,{data.Kpis.ArchivedArticles}");
        sb.AppendLine($"Total Views,{data.Kpis.TotalViews}");
        sb.AppendLine($"Created In Period,{data.Kpis.CreatedInPeriod}");
        sb.AppendLine($"Published In Period,{data.Kpis.PublishedInPeriod}");
        sb.AppendLine();

        sb.AppendLine("Editorial Workflow,Count");

        foreach (var status in data.Workflow)
        {
            sb.AppendLine($"{status.Status},{status.Count}");
        }

        sb.AppendLine();

        sb.AppendLine(
            "Author,Total Articles,Published,Draft,Pending Review,Scheduled,Rejected,Total Views,Avg Views Per Published");

        foreach (var author in data.AuthorPerformance)
        {
            sb.AppendLine(string.Join(",",
                Csv(author.AuthorName),
                author.TotalArticles,
                author.PublishedArticles,
                author.DraftArticles,
                author.ReviewPendingArticles,
                author.ScheduledArticles,
                author.RejectedArticles,
                author.TotalViews,
                author.AverageViewsPerPublishedArticle.ToString("0.0")));
        }

        sb.AppendLine();

        sb.AppendLine("Upcoming Scheduled,Author,Category,Scheduled At (UTC)");

        foreach (var item in data.UpcomingPublications)
        {
            sb.AppendLine(string.Join(",",
                Csv(item.Title),
                Csv(item.AuthorName ?? string.Empty),
                Csv(item.CategoryName ?? string.Empty),
                item.ScheduledPublishAtUtc.ToString("yyyy-MM-dd HH:mm:ss")));
        }

        sb.AppendLine();

        sb.AppendLine("Most Read,Category,Views,Published At (UTC)");

        foreach (var item in data.MostReadArticles)
        {
            sb.AppendLine(string.Join(",",
                Csv(item.Title),
                Csv(item.CategoryName ?? string.Empty),
                item.ViewCount,
                item.PublishedAtUtc?.ToString("yyyy-MM-dd HH:mm:ss")
                    ?? string.Empty));
        }

        return sb.ToString();
    }

    private static string Csv(string value)
    {
        if (value.Contains(',') ||
            value.Contains('"') ||
            value.Contains('\n'))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        return value;
    }
}
