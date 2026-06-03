namespace TheGameVoice.Domain.Entities;

public class ArticleVideo
{
    public Guid Id { get; set; }

    public Guid ArticleId { get; set; }

    public Article Article { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string VideoUrl { get; set; } = default!;

    public int SortOrder { get; set; }
}