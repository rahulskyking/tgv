namespace TheGameVoice.Web.Areas.Admin.ViewModels.Dashboard;

public class DashboardArticleItemViewModel
{
    public Guid Id { get; set; }

    public string Title { get; set; }
        = string.Empty;

    public string Status { get; set; }
        = string.Empty;

    public DateTime CreatedAt { get; set; }

}