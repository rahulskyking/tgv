using System.ComponentModel.DataAnnotations;
using TheGameVoice.Web.Areas.Admin.ViewModels.Media;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Events;

public class EventFormViewModel : IValidatableObject
{
    public Guid? Id { get; set; }
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;
    [Required, StringLength(200)]
    [RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Use lowercase letters, numbers, and single hyphens.")]
    public string Slug { get; set; } = string.Empty;
    [StringLength(5000)]
    public string? Description { get; set; }
    [Display(Name = "Banner image")]
    public Guid? BannerImageId { get; set; }
    [StringLength(250), Display(Name = "Banner alternative text")]
    public string? BannerAltText { get; set; }
    [DataType(DataType.Date), Display(Name = "Start date")]
    public DateOnly? StartDate { get; set; }
    [DataType(DataType.Date), Display(Name = "End date")]
    public DateOnly? EndDate { get; set; }
    [StringLength(250)]
    public string? Location { get; set; }
    [StringLength(1000), Display(Name = "Official website")]
    public string? OfficialWebsite { get; set; }
    [Display(Name = "Active event")]
    public bool IsActive { get; set; } = true;
    [Display(Name = "Show event name in navigation")]
    public bool ShowInNavigation { get; set; }
    [Range(0, 9999), Display(Name = "Navigation order")]
    public int DisplayOrder { get; set; }
    public List<MediaPickerItemViewModel> BannerImages { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate.HasValue && !StartDate.HasValue)
            yield return new ValidationResult("Select a start date when setting an end date.", new[] { nameof(StartDate) });

        if (StartDate.HasValue && EndDate.HasValue && EndDate.Value < StartDate.Value)
            yield return new ValidationResult("End date cannot be before the start date.", new[] { nameof(EndDate) });

        if (BannerImageId.HasValue && string.IsNullOrWhiteSpace(BannerAltText))
            yield return new ValidationResult("Describe the selected banner for accessibility.", new[] { nameof(BannerAltText) });

        if (!string.IsNullOrWhiteSpace(OfficialWebsite)
            && (!Uri.TryCreate(OfficialWebsite, UriKind.Absolute, out var uri)
                || (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp)
                || string.IsNullOrWhiteSpace(uri.Host) || !string.IsNullOrEmpty(uri.UserInfo)))
            yield return new ValidationResult("Enter a full HTTP or HTTPS website URL.", new[] { nameof(OfficialWebsite) });
    }
}
