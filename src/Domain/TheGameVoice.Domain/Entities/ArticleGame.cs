namespace TheGameVoice.Domain.Entities;

public class ArticleGame
{
    public Guid ArticleId { get; set; }

    public Article Article { get; set; }
        = default!;

    public Guid GameId { get; set; }

    public Game Game { get; set; }
        = default!;
}