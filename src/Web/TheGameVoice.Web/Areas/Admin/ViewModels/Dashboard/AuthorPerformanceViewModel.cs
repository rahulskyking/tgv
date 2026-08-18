namespace TheGameVoice.Web.Areas.Admin.ViewModels.Dashboard;

public class AuthorPerformanceViewModel
{
    public Guid AuthorId { get; set; }

    public string AuthorName { get; set; } = default!;

    public int TotalArticles { get; set; }

    public int PublishedArticles { get; set; }

    public int DraftArticles { get; set; }

    public int ReviewPendingArticles { get; set; }

    public int ScheduledArticles { get; set; }

    public int RejectedArticles { get; set; }

    public long TotalViews { get; set; }

    public double AverageViewsPerPublishedArticle { get; set; }

    public string Initials
    {
        get
        {
            if (string.IsNullOrWhiteSpace(AuthorName))
            {
                return "?";
            }

            var parts = AuthorName.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                return parts[0][0].ToString().ToUpperInvariant();
            }

            return (parts[0][0].ToString() + parts[^1][0])
                .ToUpperInvariant();
        }
    }
}
