namespace TheGameVoice.Web.Areas.Admin.ViewModels.Dashboard;

public class UpcomingArticleViewModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public DateTime ScheduledPublishAtUtc { get; set; }

    public string? AuthorName { get; set; }

    public string? CategoryName { get; set; }
}
