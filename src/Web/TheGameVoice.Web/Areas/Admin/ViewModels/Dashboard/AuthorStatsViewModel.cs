using TheGameVoice.Application.Common.Dashboard;
using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Dashboard;

public class AuthorArticleViewModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public ArticleStatus Status { get; set; }

    public string? CategoryName { get; set; }

    public int ViewCount { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? PublishedAtUtc { get; set; }

    public DateTime? ScheduledPublishAtUtc { get; set; }
}

public class AuthorTrendPointViewModel
{
    public DateOnly Day { get; set; }

    public int PublishedCount { get; set; }
}

public class AuthorCategoryBreakdownViewModel
{
    public Guid CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public int TotalArticles { get; set; }

    public int PublishedArticles { get; set; }

    public long TotalViews { get; set; }

    /// <summary>Share of the author's articles that sit in this category.</summary>
    public double SharePercentage { get; set; }
}

public class AuthorStatsViewModel
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

    public int TotalArticles { get; set; }

    public int PublishedArticles { get; set; }

    public int DraftArticles { get; set; }

    public int ReviewPendingArticles { get; set; }

    public int ScheduledArticles { get; set; }

    public int RejectedArticles { get; set; }

    public int ArchivedArticles { get; set; }

    public long TotalViews { get; set; }

    public double AverageViewsPerPublishedArticle { get; set; }

    public int CreatedInPeriod { get; set; }

    public int PublishedInPeriod { get; set; }

    public double PublishedPercentage { get; set; }

    /// <summary>Share of site-wide views earned by this author.</summary>
    public double ViewSharePercentage { get; set; }

    /// <summary>Share of site-wide published articles written by this author.</summary>
    public double PublishedSharePercentage { get; set; }

    public int? RankByViews { get; set; }

    public int? RankByPublished { get; set; }

    public int TotalAuthors { get; set; }

    public DateTime? FirstPublishedAtUtc { get; set; }

    public DateTime? LastPublishedAtUtc { get; set; }

    public IReadOnlyList<WorkflowStatusViewModel> Workflow { get; set; }
        = Array.Empty<WorkflowStatusViewModel>();

    public IReadOnlyList<MostReadArticleViewModel> TopArticles { get; set; }
        = Array.Empty<MostReadArticleViewModel>();

    public IReadOnlyList<AuthorArticleViewModel> RecentArticles { get; set; }
        = Array.Empty<AuthorArticleViewModel>();

    public IReadOnlyList<UpcomingArticleViewModel> UpcomingPublications { get; set; }
        = Array.Empty<UpcomingArticleViewModel>();

    public IReadOnlyList<AuthorTrendPointViewModel> PublishingTrend { get; set; }
        = Array.Empty<AuthorTrendPointViewModel>();

    public IReadOnlyList<AuthorCategoryBreakdownViewModel> CategoryBreakdown { get; set; }
        = Array.Empty<AuthorCategoryBreakdownViewModel>();

    public string SelectedRange { get; set; }
        = DashboardDateRange.Last30Days.ToString();

    public string SelectedRangeLabel { get; set; } = "Last 30 Days";

    /// <summary>True when the signed-in user is looking at their own page.</summary>
    public bool IsSelf { get; set; }

    public bool CanBrowseOtherAuthors { get; set; }

    public DateTime GeneratedAtUtc { get; set; }

    public string Initials
    {
        get
        {
            if (string.IsNullOrWhiteSpace(AuthorName))
            {
                return "?";
            }

            var parts = AuthorName.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                return parts[0][0].ToString().ToUpperInvariant();
            }

            return (parts[0][0].ToString() + parts[^1][0])
                .ToUpperInvariant();
        }
    }
}
