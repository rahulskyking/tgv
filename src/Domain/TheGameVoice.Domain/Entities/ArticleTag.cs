using TheGameVoice.Domain.Common.Base;

namespace TheGameVoice.Domain.Entities;

public class ArticleTag : BaseEntity
{
    public Guid ArticleId { get; set; }

    public Article Article { get; set; } = default!;

    public Guid TagId { get; set; }

    public Tag Tag { get; set; } = default!;
}