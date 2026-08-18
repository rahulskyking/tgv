namespace TheGameVoice.Application.Common.Dashboard;

public sealed class MostReadArticleData
{
    public Guid Id { get; set; }

    public string Title { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public Guid CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public int ViewCount { get; set; }

    public DateTime? PublishedAtUtc { get; set; }
}
