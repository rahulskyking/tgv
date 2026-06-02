using System.ComponentModel.DataAnnotations;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Users;

public class ResetPasswordViewModel
{
    public Guid UserId { get; set; }

    public string UserName { get; set; } = default!;

    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = default!;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; set; } = default!;
}