namespace TheGameVoice.Web.Areas.Admin.ViewModels.Dashboard;

public class DashboardViewModel
{
    public int DraftArticlesCount { get; set; }

    public int ReviewPendingCount { get; set; }

    public int PublishedArticlesCount { get; set; }

    public int ArchivedArticlesCount { get; set; }

    public int UsersCount { get; set; }

    public int MediaCount { get; set; }

    public int CategoriesCount { get; set; }

    public int TagsCount { get; set; }

    public List<DashboardArticleItemViewModel>
        RecentArticles
    {
        get;
        set;
    }
    = new();
}