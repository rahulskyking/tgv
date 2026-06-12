using TheGameVoice.Domain.Entities;

public class ArticleView
{
    public Guid Id { get; set; }

    public Guid ArticleId { get; set; }

    public Article Article { get; set; }
        = default!;

    public DateTime ViewedAt { get; set; }
}