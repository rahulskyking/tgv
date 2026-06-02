namespace TheGameVoice.Web.Areas.Admin.ViewModels.Users;

public class UserListItemViewModel
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = default!;

    public string UserName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public bool IsActive { get; set; }

    public string Role { get; set; } = default!;
}