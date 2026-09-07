using TheGameVoice.Domain.Common.Base;
using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Domain.Entities;

public class Category : AuditableEntity
{
    public string Name { get; set; }
        = default!;

    public string Slug { get; set; }
        = default!;

    // Controls the order in navigation, homepage, etc.
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Which site mode(s) this category appears in (header navigation,
    /// homepage sections). Defaults to both so nothing disappears from the
    /// navigation until an editor narrows it down.
    /// </summary>
    public GameSegment Segment { get; set; }
        = GameSegment.All;

    public ICollection<Article> Articles
    {
        get;
        set;
    }
        = new List<Article>();
}