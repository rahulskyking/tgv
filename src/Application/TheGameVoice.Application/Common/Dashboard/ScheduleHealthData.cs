namespace TheGameVoice.Application.Common.Dashboard;

public sealed class ScheduleHealthData
{
    public int ScheduledCount { get; set; }

    /// <summary>Due today according to the publication calendar timezone (IST).</summary>
    public int DueTodayCount { get; set; }

    public int DueTomorrowCount { get; set; }

    /// <summary>
    /// Scheduled articles whose <c>ScheduledPublishAt</c> is already in the past.
    /// Should normally be zero while the background publisher is healthy.
    /// </summary>
    public int OverdueCount { get; set; }

    public DateTime? NextScheduledAtUtc { get; set; }

    public string? NextScheduledTitle { get; set; }
}
