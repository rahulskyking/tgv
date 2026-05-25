using TheGameVoice.Domain.Common.Base;

public class Game : AuditableEntity
{
    public string Name { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public string Description { get; set; } = default!;

    public DateTime? ReleaseDate { get; set; }

    public string? Developer { get; set; }

    public string? Publisher { get; set; }
}