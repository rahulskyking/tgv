namespace TheGameVoice.Web.Areas.Admin.ViewModels.Dashboard;

public class ScheduleHealthViewModel
{
    public int ScheduledCount { get; set; }

    public int DueTodayCount { get; set; }

    public int DueTomorrowCount { get; set; }

    public int OverdueCount { get; set; }

    public DateTime? NextScheduledAtUtc { get; set; }

    public string? NextScheduledTitle { get; set; }
}
