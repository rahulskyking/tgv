namespace TheGameVoice.Web.Areas.Admin.ViewModels.Dashboard;

public class MostReadArticleViewModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public string? CategoryName { get; set; }

    public int ViewCount { get; set; }

    public DateTime? PublishedAtUtc { get; set; }
}
