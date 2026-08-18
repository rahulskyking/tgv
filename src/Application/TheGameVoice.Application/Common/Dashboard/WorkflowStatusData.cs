using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Application.Common.Dashboard;

public sealed class WorkflowStatusData
{
    public ArticleStatus Status { get; set; }

    public int Count { get; set; }
}
