using TheGameVoice.Domain.Common.Base;

namespace TheGameVoice.Domain.Entities;

public class Game : AuditableEntity
{
    public string Name { get; set; }
        = default!;

    public string Slug { get; set; }
        = default!;

    public string? Summary { get; set; }

    public DateTime? ReleaseDate
    { get; set; }

    public Guid? CoverImageId
    { get; set; }

    public Media? CoverImage
    { get; set; }

    //public ICollection<Article> Articles
    //{ get; set; }
    //    = new List<Article>();
    public ICollection<ArticleGame> ArticleGames
    { get; set; }
        = new List<ArticleGame>();
}