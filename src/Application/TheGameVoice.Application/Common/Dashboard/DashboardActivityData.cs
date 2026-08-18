namespace TheGameVoice.Application.Common.Dashboard;

public enum DashboardActivityType
{
    Created,

    SubmittedForReview,

    Scheduled,

    Published,

    Rejected,

    Archived
}

public sealed class DashboardActivityData
{
    public DashboardActivityType Type { get; set; }

    public Guid ArticleId { get; set; }

    public string ArticleTitle { get; set; } = default!;

    public string? ActorName { get; set; }

    public DateTime OccurredAtUtc { get; set; }
}
