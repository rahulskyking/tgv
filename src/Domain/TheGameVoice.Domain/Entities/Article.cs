using TheGameVoice.Domain.Common.Base;
using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Domain.Entities;

public class Article : AuditableEntity
{
    public string Title { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public string Summary { get; set; } = default!;

    public string Content { get; set; } = default!;

    public string? SeoTitle { get; set; }

    public string? SeoDescription { get; set; }

    public ArticleStatus Status { get; set; }

    public DateTime? PublishedAt { get; set; }
    public Guid? PublishedById { get; set; }
    public Guid AuthorId { get; set; }

    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = default!;

    public Guid? FeaturedImageId { get; set; }

    public Media? FeaturedImage { get; set; }

    public ICollection<ArticleTag> ArticleTags { get; set; }
    = new List<ArticleTag>();

    public ICollection<ArticleGame> ArticleGames
    { get; set; }
    = new List<ArticleGame>();

    public ICollection<ArticleMedia> ArticleMedia
    {
        get;
        set;
    }
= new List<ArticleMedia>();

    public ICollection<ArticleVideo> ArticleVideos
    {
        get;
        set;
    }
    = new List<ArticleVideo>();
    public int ViewCount { get; set; }

    public ICollection<ArticleView>
        ArticleViews
    {
        get;
        set;
    }
    = new List<ArticleView>();
 
}
