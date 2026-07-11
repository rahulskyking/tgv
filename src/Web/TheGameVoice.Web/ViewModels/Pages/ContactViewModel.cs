using System.ComponentModel.DataAnnotations;

namespace TheGameVoice.Web.ViewModels.Pages;

public class ContactViewModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [MinLength(20)]
    [MaxLength(2000)]
    public string Message { get; set; } = string.Empty;

    public string ContactEmail { get; set; } = string.Empty;
    // Hidden anti-spam field.
    public string? Website { get; set; }
}