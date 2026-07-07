using TheGameVoice.Domain.Common.Base;

namespace TheGameVoice.Domain.Entities;

public class Category : AuditableEntity
{
    public string Name { get; set; }
        = default!;

    public string Slug { get; set; }
        = default!;

    // Controls the order in navigation, homepage, etc.
    public int DisplayOrder { get; set; }

    public ICollection<Article> Articles
    {
        get;
        set;
    }
        = new List<Article>();
}