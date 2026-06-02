using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Users;

public class CreateUserViewModel
{
    [Required]
    public string FullName { get; set; } = default!;

    [Required]
    public string UserName { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;

    [Required]
    public string Role { get; set; } = default!;

    public List<SelectListItem> Roles { get; set; }
        = new();
}