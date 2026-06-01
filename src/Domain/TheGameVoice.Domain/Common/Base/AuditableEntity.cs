using TheGameVoice.Domain.Common.Base;
using TheGameVoice.Domain.Common.Interfaces;

public abstract class AuditableEntity
    : BaseEntity, IAuditableEntity, ISoftDelete
{
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? PublishedAt { get; set; }
}