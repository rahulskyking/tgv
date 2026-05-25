using TheGameVoice.Domain.Common.Base;

namespace TheGameVoice.Domain.Entities;

public class Category : AuditableEntity
{
    public string Name { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public string? Description { get; set; }
}