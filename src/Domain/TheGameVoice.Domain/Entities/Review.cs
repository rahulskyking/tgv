using TheGameVoice.Domain.Common.Base;
using TheGameVoice.Domain.Entities;

public class Review : AuditableEntity
{
    public string Title { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public decimal Score { get; set; }

    public string Verdict { get; set; } = default!;

    public string Pros { get; set; } = default!;

    public string Cons { get; set; } = default!;

    public Guid GameId { get; set; }

    public Game Game { get; set; } = default!;

}