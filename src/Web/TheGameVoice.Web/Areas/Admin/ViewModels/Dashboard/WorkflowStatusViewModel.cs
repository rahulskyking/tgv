using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Dashboard;

public class WorkflowStatusViewModel
{
    public ArticleStatus Status { get; set; }

    public int Count { get; set; }
}
