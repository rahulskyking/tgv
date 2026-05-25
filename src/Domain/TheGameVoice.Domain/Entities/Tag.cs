using TheGameVoice.Domain.Common.Base;

namespace TheGameVoice.Domain.Entities;

public class Tag : AuditableEntity
{
    public string Name { get; set; } = default!;

    public string Slug { get; set; } = default!;
    public ICollection<ArticleTag> ArticleTags { get; set; }
    = new List<ArticleTag>();

}