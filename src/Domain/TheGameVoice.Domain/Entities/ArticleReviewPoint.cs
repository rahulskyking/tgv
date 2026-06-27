using TheGameVoice.Domain.Common.Base;
using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Domain.Entities;

public class ArticleReviewPoint : AuditableEntity
{
    public Guid ArticleId { get; set; }

    public Article Article { get; set; } = default!;

    public ReviewPointType Type { get; set; }

    public string Text { get; set; } = default!;

    public int DisplayOrder { get; set; }
}