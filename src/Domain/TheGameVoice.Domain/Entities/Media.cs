using TheGameVoice.Domain.Common.Base;

namespace TheGameVoice.Domain.Entities;

public class Media : AuditableEntity
{
    public string FileName { get; set; } = default!;

    public string FilePath { get; set; } = default!;

    public string ContentType { get; set; } = default!;

    public long FileSize { get; set; }

    public bool IsImage { get; set; }

    public string? AltText { get; set; }

    public string? Caption { get; set; }

    public string? Credit { get; set; }
}