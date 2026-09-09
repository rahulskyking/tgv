namespace TheGameVoice.Web.Areas.Admin.ViewModels.Users;

/// <summary>
/// Wraps the filtered Users list with the query-string state that drives the
/// search bar, role/status dropdowns and sortable columns, so the view can
/// re-render the active filters and preserve them across clicks.
/// </summary>
public class UserIndexViewModel
{
    public IReadOnlyList<UserListItemViewModel> Items { get; set; }
        = Array.Empty<UserListItemViewModel>();

    /* ---- filter / sort state (normalised + validated in the controller) ---- */

    public string Query { get; set; } = string.Empty;

    /// <summary>Role filter, empty string means "all roles".</summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>"active", "inactive" or empty ("all").</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Active sort column key (see controller switch).</summary>
    public string Sort { get; set; } = "published";

    /// <summary>"asc" or "desc".</summary>
    public string Dir { get; set; } = "desc";

    /// <summary>Roles to list in the filter dropdown.</summary>
    public IReadOnlyList<string> AvailableRoles { get; set; }
        = Array.Empty<string>();

    /// <summary>Total team size before filtering (for "X of Y" labels).</summary>
    public int TotalTeamSize { get; set; }

    /* ---- derived, for the summary cards (reflects the filtered subset) ---- */

    public int ActiveUsers => Items.Count(x => x.IsActive);

    public long TotalDone => Items.Sum(x => (long)x.PublishedArticles);

    public long TotalInProgress => Items.Sum(x => (long)x.InProgressArticles);

    public long TotalViews => Items.Sum(x => x.TotalViews);

    public bool HasFilters =>
        !string.IsNullOrWhiteSpace(Query) ||
        !string.IsNullOrWhiteSpace(Role) ||
        !string.IsNullOrWhiteSpace(Status);
}
