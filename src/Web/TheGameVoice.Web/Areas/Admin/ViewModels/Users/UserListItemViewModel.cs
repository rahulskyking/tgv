namespace TheGameVoice.Web.Areas.Admin.ViewModels.Users;

public class UserListItemViewModel
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = default!;

    public string UserName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public bool IsActive { get; set; }

    public string Role { get; set; } = default!;

    public string? AvatarImagePath { get; set; }

    /* ---- lifetime publishing stats (from the dashboard aggregation) ---- */

    public int PublishedArticles { get; set; }

    public int DraftArticles { get; set; }

    public int ReviewPendingArticles { get; set; }

    public int ScheduledArticles { get; set; }

    public int RejectedArticles { get; set; }

    public long TotalViews { get; set; }

    /// <summary>Articles still in the pipeline (draft + review + scheduled).</summary>
    public int InProgressArticles =>
        DraftArticles + ReviewPendingArticles + ScheduledArticles;

    public string Initials =>
        string.Join(
            string.Empty,
            (FullName ?? string.Empty)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Take(2)
                .Select(part => char.ToUpperInvariant(part[0])));
}
