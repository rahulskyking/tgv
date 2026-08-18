using TheGameVoice.Application.Common.Dashboard;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Dashboard;

public class DashboardActivityViewModel
{
    public DashboardActivityType Type { get; set; }

    public Guid ArticleId { get; set; }

    public string ArticleTitle { get; set; } = default!;

    public string? ActorName { get; set; }

    public DateTime OccurredAtUtc { get; set; }
}
