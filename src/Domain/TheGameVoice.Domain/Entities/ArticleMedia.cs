namespace TheGameVoice.Domain.Entities;

public class ArticleMedia
{
    public Guid ArticleId { get; set; }

    public Article Article { get; set; } = default!;

    public Guid MediaId { get; set; }

    public Media Media { get; set; } = default!;

    public int SortOrder { get; set; }
}