namespace TheGameVoice.Application.Common.Dashboard;

public sealed class UpcomingArticleData
{
    public Guid Id { get; set; }

    public string Title { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public DateTime ScheduledPublishAtUtc { get; set; }

    public Guid AuthorId { get; set; }

    public string? AuthorName { get; set; }

    public Guid CategoryId { get; set; }

    public string? CategoryName { get; set; }
}
