using TheGameVoice.Domain.Common.Base;
using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Domain.Entities;

public class Media : AuditableEntity
{
    public string FileName { get; set; } = default!;

    public string FilePath { get; set; } = default!;

    public string ContentType { get; set; } = default!;

    public long FileSize { get; set; }

    public MediaType MediaType { get; set; }
}