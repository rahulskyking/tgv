using TheGameVoice.Application.Common.Pagination;

namespace TheGameVoice.Application.Modules.Tags;

public class TagAdminItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int ArticleCount { get; set; }
}

public class TagAdminPage
{
    public PagedResult<TagAdminItem> Tags { get; set; } = new();
    public int TotalTags { get; set; }
    public int UsedTags { get; set; }
    public int UnusedTags => TotalTags - UsedTags;
}
